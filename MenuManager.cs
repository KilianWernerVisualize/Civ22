using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public InputField myIPAddress;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHostButton()
    {
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetManager>().host();
    }

    public void OnJoinButton()
    {
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetManager>().join(myIPAddress.text, 8888);
    }

    public void OnStart()
    {
        SceneManager.LoadScene("game");
    }
}
