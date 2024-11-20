using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score_Counter : MonoBehaviour
{
    public int score;
    public TMP_Text score_text;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    public void increment(){
        score++;
    }
    public void decrement() {
        score--;
    }

    public void reset_score() { 
        score = 0;
    }

    void Update()
    {
        if (score > 0)
        {
            score_text.text = "Score: " + score.ToString();
        }
        else
        {
            score_text.text = "";
        }
    }
}
