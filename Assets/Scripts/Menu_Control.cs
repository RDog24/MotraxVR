using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Control : MonoBehaviour
{

    private GameObject UDP;
    public bool active = false;

    void Start()
    {
        UDP = GameObject.Find("UDP");
        UDP.SetActive(active);
    }

    void Stop_UDP_Reading()
    {
        active = false;

    }

    void Start_UDP_Reading()
    {
        active = true;
    }

    void UDP_Reset()
    {

    }

    
    void Update()
    {
        UDP.SetActive(active);
    }
}
