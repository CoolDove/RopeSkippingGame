using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyInput : MonoBehaviour
{
    private static MyInput _instance;
    public static MyInput Instance {
        get
        {
            return _instance;
        }
    }


    [SerializeField]
    private DoButton clickPad;

    public bool CommandDown { get { return clickPad.ButtonDown; } }
    public bool CommandHold { get { return clickPad.ButtonHold; } }
    public bool CommandRelease { get { return clickPad.ButtonRelease; } }

    private void Awake()
    {
        if (clickPad == null) 
        {
            throw new System.Exception("cannot find clickPad!");
        }
        _instance = this;
    }

}
