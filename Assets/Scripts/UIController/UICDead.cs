using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UICDead : UIController
{
    public override string UICName { get { return "UICDead"; } }

    public override float closeWait { get { return 0.3f; } }


    private Button btn_a;
    private Button btn_b;

    private RectTransform btnARct;
    private RectTransform btnBRct;

    private Image btnAImg;
    private Image btnBImg;

    public override void GetData()
    {

    }

    public void Restart()
    {
        EventManager.Instance.SendEvent("RestartGame");
    }
    public void BackToMenu()
    {
        EventManager.Instance.SendEvent("BackToMenu");
    }

    public override void OnClose()
    {
        btnARct.DOScale(0.2f, 0.25f).SetEase(Ease.InBack);
        btnBRct.DOScale(0.2f, 0.25f).SetEase(Ease.InBack);

        btnAImg.DOFade(0f, 0.25f);
        btnBImg.DOFade(0f, 0.25f);
    }

    public override void OnOpen()
    {
        btnARct.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        btnBRct.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

        btnAImg.DOFade(1f, 0.25f);
        btnBImg.DOFade(1f, 0.25f);
    }

    public override void Init()
    {
        btn_a = GetComponentsInChildren<Button>()[0];
        btn_b = GetComponentsInChildren<Button>()[1];

        btnARct = btn_a.GetComponent<RectTransform>();
        btnBRct = btn_b.GetComponent<RectTransform>();

        btnAImg = btn_a.GetComponent<Image>();
        btnBImg = btn_b.GetComponent<Image>();

        btnARct.localScale = new Vector3(0.1f, 0.1f, 1);
        btnBRct.localScale = new Vector3(0.1f, 0.1f, 1);
        btnAImg.color = new Color(btnAImg.color.r, btnAImg.color.g, btnAImg.color.b, 0f);
        btnBImg.color = new Color(btnBImg.color.r, btnBImg.color.g, btnBImg.color.b, 0f);

    }
}
