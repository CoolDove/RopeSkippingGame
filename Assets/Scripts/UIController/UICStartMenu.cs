using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UICStartMenu : UIController
{
    public override string UICName { get { return "UICStartMenu"; } }
    public override float closeWait { get { return 0.4f; } }

    [SerializeField]
    private RectTransform btnStartRct;

    [SerializeField]
    private Text rt;
    [SerializeField]
    private Text rc1;
    [SerializeField]
    private Text rc2;
    [SerializeField]
    private Text rc3;


    private CanvasGroup cgp;

    public override void GetData()
    {

    }

    public void StartGame()
    {
        EventManager.Instance.SendEvent("StartGame");
    }
    public void EndGame()
    {
        //EventManager.Instance.SendEvent("EndGame");
        Application.Quit();
    }

    public override void OnClose()
    {
        btnStartRct.DOScale(3.5f, closeWait).SetEase(Ease.InOutBack);
        cgp.DOFade(0f, 0.36f);
    }

    public override void OnOpen()
    {
        if (GlobalVar.records[0] == 0 && GlobalVar.records[1] == 0 && GlobalVar.records[2] == 0) 
        {
            rt.text = "Welcome!";
        }
        else
        {
            rt.text = "YouRecords:";
        }
        rc1.text = GlobalVar.records[0] == 0 ? "" : GlobalVar.records[0].ToString();
        rc2.text = GlobalVar.records[1] == 0 ? "" : GlobalVar.records[1].ToString();
        rc3.text = GlobalVar.records[2] == 0 ? "" : GlobalVar.records[2].ToString();

        btnStartRct.DOScale(1, 0.5f).SetEase(Ease.InOutBack);
        cgp.DOFade(0.93f, 0.6f);
    }

    public override void Init()
    {
        cgp = btnStartRct.GetComponent<CanvasGroup>();
    }
}
