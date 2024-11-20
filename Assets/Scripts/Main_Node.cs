using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class SensorData
{
    public IList<string> angle { get; set; }
}

public class Main_Node : MonoBehaviour
{
    public SensorData Feather13 { get; private set; }
    private GameObject UDP;
    private UDP_Other_Node listener;
    private CancellationTokenSource cancellationTokenSource;
    private Task networkTask;

    GameObject saveObj = null;
    FileHandler fileHandler = null;

    public bool readFile = false;
    public bool disableUDP = false;
    public int speed = 1;

    GameObject scoreObj = null;
    Score_Counter counter = null;

    void Start()
    {
        Debug.Log("Start");

        scoreObj = GameObject.Find("ScoreScript");
        if (scoreObj != null)
        {
            counter = scoreObj.GetComponent<Score_Counter>();
        }
        else
        {
            Debug.LogWarning("GameObject with ScoreScript not found.");
        }

        saveObj = GameObject.Find("Save");
        if (saveObj != null)
        {
            fileHandler = saveObj.GetComponent<FileHandler>();
        }
        else
        {
            Debug.LogWarning("GameObject with Save not found.");
        }

        UDP = GameObject.Find("UDP");
        listener = UDP.GetComponent<UDP_Other_Node>();
        if (listener == null)
        {
            Debug.LogError("UDP_Other_Node component not found on UDP object!");
        }

        cancellationTokenSource = new CancellationTokenSource();
        networkTask = Task.Run(() => NetworkListener(cancellationTokenSource.Token), cancellationTokenSource.Token);
    }

    private void NetworkListener(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            string message = null;

            if (readFile == false)
            {
                if (disableUDP == false)
                {
                    message = listener.dataReceived;
                }
            }
            else { 
                message = fileHandler.GetLine();
                if (message == null) { 
                    fileHandler.ResetReader();
                    message = fileHandler.GetLine();
                    counter.increment();
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                Feather13 = JsonConvert.DeserializeObject<SensorData>(message);
                //Debug.Log("Data received and processed in background thread");
            }

            // Introduce a slight delay to avoid hammering the CPU
            Thread.Sleep(speed);
        }
    }

    void OnDestroy()
    {
        // Ensure the cancellation token is triggered when this object is destroyed
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
        }

        // Optionally wait for the task to complete
        if (networkTask != null)
        {
            networkTask.Wait();
        }
    }
}
