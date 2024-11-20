using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;
using System.Security.Cryptography.Xml;

public class UDP_Other_Node : MonoBehaviour
{
    Thread thread;
    public int connectionPort = 5011;
    string multicastAddress = "224.1.1.1";

    UdpClient listener;
    IPEndPoint groupEP;

    public string dataReceived;

    GameObject saveObj = null;
    FileHandler fileHandler = null;

    public bool write = false;

    void Start()
    {
        saveObj = GameObject.Find("Save");
        if (saveObj != null)
        {
            fileHandler = saveObj.GetComponent<FileHandler>();
        }
        else
        {
            Debug.LogWarning("GameObject with Save not found.");
        }

        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.IsBackground = true;
        thread.Start();
    }

    void GetData()
    {
        listener = new UdpClient(connectionPort);
        //listener.ExclusiveAddressUse = false;
        groupEP = new IPEndPoint(IPAddress.Any, connectionPort);

        //listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        //listener.Client.Bind(groupEP);

        // Join the multicast group
        IPAddress multicastIP = IPAddress.Parse(multicastAddress);
        listener.JoinMulticastGroup(multicastIP);

        // Start listening
        while (true)
        {
            Connection();
        }
        //listener.Close();

    }

    void Connection()
    {

        //print("Waiting for data");

        byte[] buffer = listener.Receive(ref groupEP);

        int bytesRead = Buffer.ByteLength(buffer);

        // Decode the bytes into a string
        dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        if(fileHandler != null && write == true)
        {
            fileHandler.WriteLines(dataReceived);
        }

        // print(dataReceived);
    }




    void Update()
    {


    }

    void OnDisable()
    {
        listener.Close();
        print("Kill Threads");
        if (thread != null)
        {
            thread.Abort();
        }
    }

    public string getString()
    {
        return dataReceived;
    }

}