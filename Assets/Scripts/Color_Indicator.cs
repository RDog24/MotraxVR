using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_Indicator : MonoBehaviour
{

    public bool active;
    private Color objectColor;

    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        active = false;

        if (renderer != null && renderer.material.HasProperty("_Color"))
        {
            // Access the color property of the material
            objectColor = renderer.material.color;
            // Debug.Log("Object color: " + objectColor);
        }
        else
        {
            Debug.LogWarning("No Renderer or color property found on this object.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active) {
            set_color();
        }
        else
        {
            reset_color();
        }
    }

    public void set_color() 
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void reset_color()
    {
        GetComponent<Renderer>().material.color = objectColor;
    }
}
