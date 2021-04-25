using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager _instance;
    public static UIManager Instance {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    Dictionary<string, UIController> UIControllers = new Dictionary<string, UIController>();

    public static void CloseUI(string name)
    {
        if (Instance.UIControllers.ContainsKey(name))
        {
            Instance.UIControllers[name].CloseUI();
        }
    }
    public static void OpenUI(string name)
    {
        if (Instance.UIControllers.ContainsKey(name))
        {
            Instance.UIControllers[name].gameObject.SetActive(true);
        }
    }
    public static void RegisterUI(UIController UIC)
    {
        if (!Instance.UIControllers.ContainsKey(UIC.UICName))
        {
            Instance.UIControllers.Add(UIC.UICName, UIC);
            Debug.Log("RegisterUI:" + UIC.name);
            UIC.gameObject.SetActive(false);
        }
        else
        {
            throw new System.Exception("重复创建同一UI！！");
        }
    }
}
