using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void zoomIn()
    {
        Camera myCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        myCam.orthographicSize = Mathf.Max(1, myCam.orthographicSize - 0.5f);
    }

    public void zoomOut()
    {
        Camera myCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        myCam.orthographicSize = Mathf.Min(20, myCam.orthographicSize + 0.5f);
    }
}
