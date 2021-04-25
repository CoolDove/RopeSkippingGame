using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIController : MonoBehaviour
{
    public abstract string UICName { get; }
    public abstract float closeWait { get; }

    private WaitForSeconds closeWaiter;

    private void Awake()
    {
        UIManager.RegisterUI(this);
        closeWaiter = new WaitForSeconds(closeWait);
        Init();
    }

    private void Update()
    {
        GetData();
    }
    public void Remove()
    {
        Destroy(gameObject);
    }

    public abstract void GetData();

    public void OnEnable()
    {
        OnOpen();
    }

    public void CloseUI()
    {
        Debug.Log("Close");
        StartCoroutine(CloseWait());
    }

    private IEnumerator CloseWait()
    {
        OnClose();
        yield return closeWaiter;
        gameObject.SetActive(false);
    }

    public abstract void Init();
    public abstract void OnClose();
    public abstract void OnOpen();
}
