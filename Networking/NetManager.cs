using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.IO;

public class NetManager : MonoBehaviour, NetworkListener
{

    NetworkAdapter adapter;

    public bool isHost = true;
    public static int id;
    public int[] clients;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        adapter = GetComponent<LLAPI_NetworkAdapter>();
        adapter.init();
        adapter.join("127.0.0.1", 8888);
        adapter.listen(this);
    }

    public void join(string ip, int port)
    {
        isHost = false;
        adapter.join(ip, port);
    }

    public void host()
    {
        isHost = true;
        adapter.host();
    }

    public void netSyncAction(Action action)
    {
        action.local = true;
        action.host = isHost;
        action.perform();
        if (isHost)
        {
            for (int current = 1; (current < clients.Length); current++)
            {
                adapter.send(action.Serialize(), clients[current]);
            }
        }
        else
        {
            adapter.send(action.Serialize(), 0);
        }
    }

    public void OnReceive(byte[] msg, int client)
    {
        Action action = SerializerExtension.Deserialize<Action>(msg);

        action.local = false;
        action.host = isHost;
        action.perform();
        
        if (isHost)
        {
            for (int current = 1; (current < clients.Length); current++)
            {
                if (clients[current] != client)
                    adapter.send(action.Serialize(), clients[current]);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnRemoteConnection()
    {
        clients = adapter.clients();
        if (isHost)
        {
            if (clients.Length > 1)
            {
                JoinGameAction join = new JoinGameAction();
                join.player = clients.Length-1;
                join.host = isHost;
                join.perform();
                adapter.send(join.Serialize(), clients[clients.Length - 1]);
            }
        }
    }

    public void OnConnection()
    {
        clients = adapter.clients();
    }

    public void OnReceiveError()
    {

    }

    public void OnDisconnect(int client)
    {
        clients = adapter.clients();
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().players.Remove(client);
        Debug.Assert(clients.Length == GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().players.Count);
    }
}

public static class SerializerExtension
{
    public static byte[] Serialize(this object obj)
    {
        if (obj == null)
        {
            return null;
        }
        var bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static T Deserialize<T>(this byte[] byteArray) where T : class
    {
        if (byteArray == null)
        {
            return null;
        }
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(byteArray, 0, byteArray.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = (T)binForm.Deserialize(memStream);
            return obj;
        }
    }
}


public interface NetworkListener
{
    void OnRemoteConnection();
    void OnConnection();
    void OnReceive(byte[] msg, int client);
    void OnReceiveError();
    void OnDisconnect(int client);
}

public interface NetworkAdapter
{
    bool init();
    bool host();
    bool join(string ip, int port);
    int[] clients();
    bool disconnect();
    bool send(byte[] msg, int client);
    void listen(NetworkListener listener);
}

[System.Serializable]
public class Action
{
    public bool local;
    public bool host;

    protected GameManager manager()
    {
        return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public virtual void perform()
    {

    }
}

[System.Serializable]
public class JoinGameAction : Action
{
    public int player;

    public override void perform()
    {
        NetManager.id = player;
    }
}
