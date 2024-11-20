using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable_Tracker : MonoBehaviour
{
    public int mode = 1;

    public int ticks = 0;

    public void flipMode()
    {
        if (mode == 1)
        {
            mode = 2;
        }
        else
        {
            mode = 1;
        }
    }

}
