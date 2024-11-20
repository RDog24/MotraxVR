using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;


public class Main_Node2 : MonoBehaviour
{
    public static SensorData Feather13 { get; private set; }
    public float total_time;
    private float speed = 12.0f;
    //private SubscriberSocket _socket;
    private int number = 13;
    private int maxQueueSize = 153;
    private bool end = true;
    private Queue<float> waiting;
    //private Queue<float> waiting_time;

    Quaternion change;
    Quaternion setting;

    private GameObject UDP;
    private UDP_Other_Node listener;

    public string message;

    void Start()
    {
        //_socket = new SubscriberSocket();
        //_socket.Connect("tcp://localhost:5555");
        //_socket.Subscribe("");
        Debug.Log("Start");
        waiting = new Queue<float>();
        setting = transform.rotation;
        //setting = transform.localRotation;

        UDP = GameObject.Find("UDP");
        listener = UDP.GetComponent<UDP_Other_Node>();
    }

    void Update()
    {
        //string message = _socket.ReceiveFrameString();
        //float message_in = Time.time;

        message = listener.dataReceived;

        if (!string.IsNullOrEmpty(message))
        {
            //float message_get = Time.time;
            Feather13 = JsonConvert.DeserializeObject<SensorData>(message);

            if (Feather13 != null && Feather13.angle != null && Feather13.angle.Count >= number * 3)
            {
                //total_time = message_get - message_in;

                float delta_x = float.Parse(Feather13.angle[(number - 1) * 3]);
                float delta_y = float.Parse(Feather13.angle[(number - 1) * 3 + 1]);
                float delta_z = float.Parse(Feather13.angle[(number * 3) - 1]);

                float convertX = delta_x;
                float convertY = delta_z;
                float convertZ = delta_y * -1;


                //Quaternion currentRotation = transform.rotation;
                //change = Quaternion.Euler(delta_x, delta_y, delta_z);
                //Quaternion newRotation = currentRotation * change;
                //transform.rotation = Quaternion.Slerp(currentRotation, newRotation, Time.deltaTime * speed);

                //Debug.Log("delta_x: " + delta_x);
                //Debug.Log("delta_y: " + delta_y);
                //Debug.Log("delta_z: " + delta_z);

                if (end)
                {
                    Quaternion currentRotation = transform.rotation;

                    if (waiting.Count > 0)
                    {
                        change = Quaternion.Euler(waiting.Dequeue(), waiting.Dequeue(), waiting.Dequeue());
                    }
                    else
                    {
                        change = Quaternion.Euler(convertX, convertY, convertZ);
                    }

                    Quaternion newRotation = setting * change;

                    //float angleDiff = Quaternion.Angle(currentRotation, newRotation);
                    //float duration = angleDiff / 180f;

                    end = false;

                    StartCoroutine(Smooth_Rotate(currentRotation, newRotation, 1f / speed)); //// 1f / speed can change to duration
                }
                else
                {
                    if (waiting.Count <= maxQueueSize)
                    {
                        if (waiting.Count != 0)
                        {
                            int index = waiting.Count / 3;

                            float x = GetElement(waiting, (index - 1) * 3);
                            float y = GetElement(waiting, ((index - 1) * 3) + 1);
                            float z = GetElement(waiting, (index * 3) - 1);

                            //Debug.Log("abs x: " + Mathf.Abs(delta_x - x));
                            //Debug.Log("abs y: " + Mathf.Abs(delta_y - y));
                            //Debug.Log("abs z: " + Mathf.Abs(delta_z - z));

                            if (Mathf.Abs(convertX - x) > 5 || Mathf.Abs(convertY - y) > 5 || Mathf.Abs(convertZ - z) > 5)
                            {
                                waiting.Enqueue(convertX);
                                waiting.Enqueue(convertY);
                                waiting.Enqueue(convertZ);
                            }
                        }
                        else
                        {
                            waiting.Enqueue(convertX);
                            waiting.Enqueue(convertY);
                            waiting.Enqueue(convertZ);
                        }
                    }
                    //Debug.Log("Number of element: " + waiting.Count);
                }
            }
        }
        if (Input.GetKey("a")){
            OnDestroy();
        }
            
    }

    IEnumerator Smooth_Rotate(Quaternion currentRotation, Quaternion newRotation, float duration)
    {
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            transform.rotation = currentRotation;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;
                // progress will equal 0 at startTime, 1 at endTime.
                transform.rotation = Quaternion.Slerp(currentRotation, newRotation, progress);
                yield return null;
            }
        }
        transform.rotation = newRotation;
        end = true;
    }

    private float GetElement(Queue<float> queue, int index)
    {
        if (index < 0 || index > queue.Count)
        {
            Debug.LogError("Index Out of range");
            return -1;
        }
        float[] array = queue.ToArray();

        return array[index];
    }

    void OnDestroy()
    {
        //if (_socket != null)
        //{
        //    _socket.Close();
        //    NetMQConfig.Cleanup();
        //    Debug.Log("END!!!");
        //}
    }
}
