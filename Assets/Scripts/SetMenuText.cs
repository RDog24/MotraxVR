using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetMenuText : MonoBehaviour
{
    public TMP_Text text;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void setMode()
    {
        if (text.text == "Mode 1")
        {
            text.text = "Mode 2";
        }
        else
        {
            text.text = "Mode 1";
        }

    }

}
