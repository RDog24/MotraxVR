using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int number;
    public float speed = 1f;
    Quaternion rotation1;
    Quaternion setting;
    private Quaternion change;

    private void Start()
    {
        setting = transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //print(transform.rotation.eulerAngles.x);
            rotation1 = transform.rotation;
           
            if (number == 9)
            {
                change = Quaternion.Euler(0, 0, 0);
            }
            else if (number == 7)
            {
                change = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                change = Quaternion.Euler(0, 0, 90);
            }

            Quaternion unityRotation = new Quaternion(change.x, change.y, -change.z, -change.w);
            Quaternion rotation2 = rotation1 * unityRotation;
            StartCoroutine(RotateOverTime(rotation1, rotation2, 1f / speed));
            transform.rotation = setting;
        }
    }

    IEnumerator RotateOverTime(Quaternion originalRotation, Quaternion finalRotation, float duration)
    {
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            transform.rotation = originalRotation;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;
                // progress will equal 0 at startTime, 1 at endTime.
                transform.rotation = Quaternion.Slerp(originalRotation, finalRotation, progress);
                yield return null;
            }
        }
        transform.rotation = finalRotation;
    }
}