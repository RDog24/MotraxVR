using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Real_time_other : MonoBehaviour
{
    public int number;
    [HideInInspector] public float convertX;
    [HideInInspector] public float convertY;
    [HideInInspector] public float convertZ;
    private float previous = 0f;
    private Queue<Quaternion> waiting = new Queue<Quaternion>();
    private Queue<float> waiting_time = new Queue<float>();
    private bool end = true;
    private Quaternion setting;

    Main_Node main_Node;

    private void Awake()
    {
        GameObject Reference = GameObject.Find("Write");
        if (Reference != null)
        {
            main_Node = Reference.GetComponent<Main_Node>();
        }
        else
        {
            Debug.LogWarning("GameObject with Write not found.");
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        setting = transform.rotation;
        previous = Time.time;
    }

    // Update is called once per frame
    private void Update()
    {
        Moving();
    }

    private void Moving()
    {
        if (main_Node.Feather13 != null && main_Node.Feather13.angle != null && main_Node.Feather13.angle.Count >= number * 3)
        {
            Calculation();

            Quaternion change = Quaternion.Euler(convertX, convertY, convertZ);

            float current = Time.time;
            float interval = current - previous;
            previous = current;

            if (end)
            {
                Quaternion newRotation = setting * change;

                end = false;

                StartCoroutine(Smooth_Rotate(transform.rotation, newRotation, interval));
            }
            else
            {
                waiting.Enqueue(change);
                waiting_time.Enqueue(interval);
                //Debug.Log("Number of element: " + waiting.Count);
            }
        }
    }

    private void Calculation()
    {
        string Ax = main_Node.Feather13.angle[(number - 1) * 3];
        string Ay = main_Node.Feather13.angle[((number - 1) * 3) + 1];
        string Az = main_Node.Feather13.angle[(number * 3) - 1];

        float delta_x = float.Parse(Ax);
        float delta_y = float.Parse(Ay);
        float delta_z = float.Parse(Az);

        if (number is 1 or 4)
        {
            //foot
            convertX = delta_y;
            convertY = delta_z;
            convertZ = delta_x * -1;
        }
        else if (number is < 7)
        {
            //leg except foot
            convertX = delta_x;
            convertY = delta_z;
            convertZ = delta_y * -1;
        }
        else if (number is > 6 and < 13)
        {
            //left and right arm
            convertX = delta_y * -1;
            convertY = delta_z;
            convertZ = delta_x;
        }
        else
        {
            //spine
            convertX = delta_x * -1;
            convertY = delta_z * -1;
            convertZ = delta_y * -1;
        }
    }

    private IEnumerator Smooth_Rotate(Quaternion currentRotation, Quaternion newRotation, float duration)
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
                transform.rotation = Quaternion.Slerp(currentRotation, newRotation, progress);
                yield return null;
            }
        }
        transform.rotation = newRotation;

        if (waiting.Count > 0)
        {
            Quaternion nextChange = waiting.Dequeue();
            Quaternion nextRotation = setting * nextChange;
            float nextDuration = waiting_time.Dequeue();
            StartCoroutine(Smooth_Rotate(transform.rotation, nextRotation, nextDuration));
        }
        else
        {
            end = true;
        }
    }
}