using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#pragma warning disable 0618

public class LLAPI_NetworkAdapter : MonoBehaviour, NetworkAdapter
{
    public int myTCP;
    public int myHostID;
    public int[] myConnections;
    public int connectionCount;

    NetworkListener listener;

    public bool init()
    {
        ConnectionConfig config = new ConnectionConfig();
        myTCP = config.AddChannel(QosType.Reliable);
        NetworkTransport.Init();
        HostTopology topology = new HostTopology(config, 10);
        myHostID = NetworkTransport.AddHost(topology, 8888);
        myConnections = new int[10];
        connectionCount = 0;
        return true;
    }

    public bool host()
    {
        return true;
    }

    public bool join(string ip, int port)
    {
        byte error;

        myConnections[0] = NetworkTransport.Connect(myHostID, ip, port, 0, out error);

        if ((NetworkError)error == NetworkError.Ok)
            return true;
        else
        {
            Debug.LogError("ConnectionError: " + (NetworkError)error);
            return false;
        }
    }

    public int[] clients()
    {
        int[] tmp = new int[connectionCount];
        for (int i = 0; (i < connectionCount); i++)
        {
            tmp[i] = myConnections[i];
        }
        return tmp;
    }

    public bool disconnect()
    {
        byte error;

        NetworkTransport.Disconnect(myHostID, myConnections[0], out error);

        if ((NetworkError)error == NetworkError.Ok)
            return true;
        else
        {
            Debug.LogError("DisconnectError: " + (NetworkError)error);
            return false;
        }
    }

    public bool send(byte[] msg, int client)
    {
        byte error;
        NetworkTransport.Send(myHostID, myConnections[client], myTCP, msg, msg.Length, out error);

        if ((NetworkError)error == NetworkError.Ok)
            return true;
        else
        {
            Debug.LogError("SendError: " + (NetworkError)error);
            return false;
        }
    }

    public void listen(NetworkListener listener)
    {
        this.listener = listener;
    }

    void Update()
    {
        byte error;

        bool received = true;

        while (received)
        {
            int sender;
            int senderConnection;
            int senderChannel;

            byte[] rmsg = new byte[1024];
            int rsize;

            NetworkEventType recData = NetworkTransport.Receive(out sender, out senderConnection, out senderChannel, rmsg, rmsg.Length, out rsize, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogError("ReceiveError: " + (NetworkError)error);
                listener.OnReceiveError();
                return;
            }

            switch (recData)
            {
                case NetworkEventType.Nothing:
                    received = false;
                    break;
                case NetworkEventType.ConnectEvent:
                    if (myConnections[0] == senderConnection)
                    {
                        listener.OnConnection();
                    }
                    else
                    {
                        connectionCount++;
                        myConnections[connectionCount] = senderConnection;
                        listener.OnRemoteConnection();
                    }
                    break;
                case NetworkEventType.DataEvent:
                    byte[] tmp = new byte[rsize];
                    for (int i = 0; (i < rsize); i++)
                    {
                        tmp[i] = rmsg[i];
                    }
                    for (int i = 0; (i < myConnections.Length); i++)
                    {
                        if (senderConnection == myConnections[i])
                        {
                            listener.OnReceive(tmp, i);
                        }
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    for (int i = 0; (i < myConnections.Length); i++)
                    {
                        if (senderConnection == myConnections[i])
                        {
                            listener.OnDisconnect(i);
                        }
                    }
                    break;
                case NetworkEventType.BroadcastEvent: break;
            }
        }
    }
}
