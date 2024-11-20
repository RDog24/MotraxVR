//using UnityEngine;
//using NetMQ;
//using NetMQ.Sockets;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Collections;

//public class SensorData
//{
//    public IList<string> angle { get; set; }
//}


//public class Main_Node : MonoBehaviour
//{
//    public static SensorData Feather13 { get; private set; }
//    //private SubscriberSocket _socket;

//    private GameObject UDP;
//    private UDP_Other_Node listener;

//    void Start()
//    {
//        Debug.Log("Start");

//        UDP = GameObject.Find("UDP");
//        listener = UDP.GetComponent<UDP_Other_Node>();
//        if (listener == null)
//        {
//            Debug.LogError("UDP_Other_Node component not found on UDP object!");
//        }
//    }

//    void Update()
//    {
//        string message = listener.dataReceived;

//        if (!string.IsNullOrEmpty(message))
//        {
//            Feather13 = JsonConvert.DeserializeObject<SensorData>(message);
//        }
//    }
//}
