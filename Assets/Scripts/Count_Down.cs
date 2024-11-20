using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Count_Down : MonoBehaviour
{
    public float countdownTime = 3f; // Set the countdown time in seconds
    private float currentTime;
    public TMP_Text countdownText; // Reference to a UI Text component (optional)
    private bool start_count;

    Main_Node main;

    void Start()
    {
        countdownText.text = "";
        GameObject reference = GameObject.Find("WriteFile");
        if (reference != null) { 
            main = reference.GetComponent<Main_Node>();
        }
        else
        {
            Debug.LogWarning("GameObject with Write not found.");
        }

        // Initialize the timer
        currentTime = countdownTime;
        start_count = false;
    }

    void Update()
    {
        countdownText.text = "";
        if (start_count == true) { 
            // Decrease the timer over time
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;

                // Optional: Update UI text if available
                if (countdownText != null)
                {
                    countdownText.text = "Starting in: " + Mathf.Ceil(currentTime).ToString(); // Display as integer
                }
            }
            else
            {
                // Timer has reached zero
                currentTime = 0;
                OnCountdownEnd();
            }
        }
    }   

    void OnCountdownEnd()
    {
        // Add any actions you want to happen when the countdown reaches zero
        //Debug.Log("Countdown Finished!");

        main.setActive();


        reset_countdown();
    }

    public void reset_countdown() {
        currentTime = countdownTime;
        stop_countdown();
    }

    public void start_countdown() { 
        start_count= true;
    }

    public void stop_countdown() {
        start_count = false;
    }
}
