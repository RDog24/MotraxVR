using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.IO.LowLevel.Unsafe;

public class Posture_Indicator : MonoBehaviour
{
    string[] part_name = new string[8] {"Left-ankle", "Left-arm", "Left-forearm", "Left-leg", 
        "Right-ankle", "Right-arm", "Right-forearm", "Right-leg"};

    string[] joint_name = new string[8] {"mixamorig7:RightLeg", "mixamorig7:RightArm", "mixamorig7:RightForeArm", "mixamorig7:RightUpLeg",
        "mixamorig7:LeftLeg", "mixamorig7:LeftArm", "mixamorig7:LeftForeArm", "mixamorig7:LeftUpLeg"};

    GameObject body;

    public void set_color(string body_part)
    {
        string body_part_name;
        string game_object_name;
        
        int index = Array.IndexOf(joint_name, body_part);

        if (index != -1)
        {
            body_part_name = part_name[index];
            game_object_name = "/User/" + body_part_name;
            GameObject body_obj = GameObject.Find(game_object_name);
            if (body_obj != null)
            {
                Color_Indicator color_change = body_obj.GetComponent<Color_Indicator>();
                //color_change.set_color();
                color_change.active = true;
                Debug.Log($"{body_part} found, {body_part_name} color set.");
            }
            else
            {
                Debug.LogWarning("GameObject not found.");
            }
        }
        else
        {
            Debug.Log($"{body_part} not found.");
        }
    }

    public void reset_color(string body_part)
    {
        string body_part_name;
        string game_object_name;

        int index = Array.IndexOf(joint_name, body_part);

        if (index != -1)
        {
            body_part_name = part_name[index];
            game_object_name = "/User/" + body_part_name;
            GameObject body_obj = GameObject.Find(game_object_name);
            if (body_obj != null)
            {
                Color_Indicator color_change = body_obj.GetComponent<Color_Indicator>();
                //color_change.reset_color();
                color_change.active = false;
                //Debug.Log($"{body_part} found, {body_part_name} color reset.");
            }
            else
            {
                Debug.LogWarning("GameObject not found.");
            }
        }
        else
        {
            Debug.Log($"{body_part} not found.");
        }
    }
}
