using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoButton : Button
{
    public bool ButtonDown;
    public bool ButtonHold;
    public bool ButtonRelease;

    void Update()
    {
        if (!ButtonHold&& IsPressed())
        {
            ButtonDown = true;
        }
        else
        {
            ButtonDown = false;
        }
        if (ButtonHold && !IsPressed()) 
        {
            ButtonRelease = true;
        }
        else
        {
            ButtonRelease = false;
        }
        ButtonHold = IsPressed();
    }
}
