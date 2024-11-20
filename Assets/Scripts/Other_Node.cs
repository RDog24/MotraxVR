using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Other_Node : MonoBehaviour
{
    public int number;
    [HideInInspector] public float delta_x;
    [HideInInspector] public float delta_y;
    [HideInInspector] public float delta_z;
    private const float speed = 20.0f;
    private Queue<float> waiting;
    private const int maxQueueSize = 153;
    private float convertX;
    private float convertY;
    private float convertZ;
    private bool end = true;

    private Quaternion setting;
    private Quaternion xRotation;
    private Quaternion yRotation;
    private Quaternion zRotation;

    public string objName = "Write";

    Main_Node main_Node;

    private void Awake()
    {
        GameObject reference = GameObject.Find(objName);
        if (reference != null)
        {
            main_Node = reference.GetComponent<Main_Node>();
        }
        else
        {
            Debug.LogWarning("GameObject with Write not found.");
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        waiting = new Queue<float>();
        setting = transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        if (main_Node.Feather13 != null && main_Node.Feather13.angle != null && main_Node.Feather13.angle.Count >= number * 3)
        {
            Moving();
        }
    }

    private async void Moving()
    {
        await Task.Run(() => Calculation());

        if (end)
        {
            Quaternion currentRotation = transform.rotation;

            if (waiting.Count > 0)
            {
                DequeueValues();
            }

            xRotation = Quaternion.AngleAxis(convertX, Vector3.right);
            yRotation = Quaternion.AngleAxis(convertY, Vector3.up);
            zRotation = Quaternion.AngleAxis(convertZ, Vector3.forward);

            Quaternion newRotation = setting * zRotation * xRotation * yRotation;

            end = false;

            StartCoroutine(Smooth_Rotate(currentRotation, newRotation, 1f / speed));
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

                    if (Mathf.Abs(convertX - x) >= 5 || Mathf.Abs(convertY - y) >= 5 || Mathf.Abs(convertZ - z) >= 5)
                    {
                        EnqueueValues();
                    }
                }
                else
                {
                    EnqueueValues();
                }
            }
        }
    }


    private void Calculation()
    {
        string Ax = main_Node.Feather13.angle[(number - 1) * 3];
        string Ay = main_Node.Feather13.angle[((number - 1) * 3) + 1];
        string Az = main_Node.Feather13.angle[(number * 3) - 1];

        delta_x = float.Parse(Ax);
        delta_y = float.Parse(Ay);
        delta_z = float.Parse(Az);

        switch (number)
        {
            case 1 or 4:
                convertX = delta_y;
                convertY = delta_z;
                convertZ = delta_x * -1;
                break;
            case < 7:
                //leg except foot
                convertX = delta_x;
                convertY = delta_z;
                convertZ = delta_y * -1;
                break;
            case > 6 and < 10:
                //left arm
                convertX = delta_x * -1;
                convertY = delta_z;
                convertZ = delta_y * -1;
                break;
            case > 9 and < 13:
                //right arm
                convertX = delta_x * -1;
                convertY = delta_z * -1;
                convertZ = delta_y;
                break;
        }
    }

    private void EnqueueValues()
    {
        waiting.Enqueue(convertX);
        waiting.Enqueue(convertY);
        waiting.Enqueue(convertZ);
    }

    private void DequeueValues()
    {
        convertX = waiting.Dequeue();
        convertY = waiting.Dequeue();
        convertZ = waiting.Dequeue();
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
}


