using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UICPlaying : UIController
{
    public override string UICName { get { return "UICPlaying"; } }
    public override float closeWait { get { return 0.3f; } }

    [SerializeField]
    private RectTransform btnPauReRct;
    [SerializeField]
    private RectTransform btnRestartRct;
    [SerializeField]
    private RectTransform btnBackRct;

    [SerializeField]
    private Sprite pauseSpt;
    [SerializeField]
    private Sprite resumeSpt;


    private Image btnPauReImg;
    private Image btnRestartImg;
    private Image btnBackImg;

    [Header("PositionSlot")]
    [SerializeField]
    private Vector2 sinkRootPos;
    [SerializeField]
    private Vector2 waitingRootPos;
    [SerializeField]
    private Vector2 showingRootPos;
    [SerializeField]
    private Vector2 unfoldOffsetRight;





    private Vector2 unfoldOffsetLeft;

    private float timeToSwitch = 0.25f;


    private WaitForSeconds waiter;

    private bool pausing;
    private bool moving;



    public override void GetData()
    {
    }

    public void ClickOnPauRe()
    {
        if (!moving)
        {
            if (pausing)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void Restart()
    {
        EventManager.Instance.SendEvent("RestartPrepare");
        StartCoroutine(ToRestart());
    }
    private IEnumerator ToRestart()
    {
        moving = true;
        btnBackRct.DOAnchorPos(Vector2.zero, timeToSwitch);
        btnRestartRct.DOAnchorPos(Vector2.zero, timeToSwitch);
        btnPauReRct.DOAnchorPos(waitingRootPos, timeToSwitch);

        btnBackImg.DOFade(0f, timeToSwitch);
        btnRestartImg.DOFade(0f, timeToSwitch);
        btnPauReImg.DOFade(0.6f, 0.4f);

        yield return waiter;

        btnPauReImg.sprite = pauseSpt;
        EventManager.Instance.SendEvent("RestartGame");

        btnBackRct.gameObject.SetActive(false);
        btnRestartRct.gameObject.SetActive(false);
        moving = false;
        pausing = false;
    }

    public void BackToMenu()
    {
        EventManager.Instance.SendEvent("BackToMenu");
    }

    private void PauseGame()
    {
        EventManager.Instance.SendEvent("PauseGame");
        btnBackRct.gameObject.SetActive(true);
        btnRestartRct.gameObject.SetActive(true);

        btnBackRct.anchoredPosition = btnRestartRct.anchoredPosition = Vector2.zero;

        btnBackRct.DOAnchorPos(unfoldOffsetLeft, timeToSwitch);
        btnRestartRct.DOAnchorPos(unfoldOffsetRight, timeToSwitch);
        btnPauReRct.DOAnchorPos(showingRootPos, 0.2f).SetEase(Ease.OutBack);

        btnBackImg.DOFade(1f, timeToSwitch);
        btnRestartImg.DOFade(1f, timeToSwitch);
        btnPauReImg.DOFade(1f, 0.4f);

        moving = true;
        pausing = true;

        btnPauReImg.sprite = resumeSpt;

        StartCoroutine(ToPause());
    }
    private IEnumerator ToPause()
    {
        yield return waiter;
        moving = false;
    }

    private void ResumeGame()
    {
        btnBackRct.DOAnchorPos(Vector2.zero, timeToSwitch);
        btnRestartRct.DOAnchorPos(Vector2.zero, timeToSwitch);
        btnPauReRct.DOAnchorPos(waitingRootPos, timeToSwitch);

        btnBackImg.DOFade(0f, timeToSwitch);
        btnRestartImg.DOFade(0f, timeToSwitch);
        btnPauReImg.DOFade(0.6f, 0.4f);

        btnPauReImg.sprite = pauseSpt;

        StartCoroutine(ToResume());
    }
    private IEnumerator ToResume()
    {
        moving = true;
        yield return waiter;
        btnBackRct.gameObject.SetActive(false);
        btnRestartRct.gameObject.SetActive(false);
        moving = false;
        pausing = false;
        EventManager.Instance.SendEvent("ResumeGame");
    }

    public override void OnClose()
    {
        moving = true;
        btnBackRct.gameObject.SetActive(false);
        btnRestartRct.gameObject.SetActive(false);
        btnPauReRct.DOAnchorPos(sinkRootPos, closeWait).SetEase(Ease.InOutElastic);

        btnPauReImg.DOFade(0, closeWait);
    }

    public override void OnOpen()
    {
        btnPauReImg.sprite = pauseSpt;
        pausing = false;
        moving = false;
        btnPauReRct.anchoredPosition = sinkRootPos;
        btnBackRct.gameObject.SetActive(false);
        btnRestartRct.gameObject.SetActive(false);
        btnPauReRct.DOAnchorPos(waitingRootPos, 0.4f).SetEase(Ease.InOutElastic);
        btnPauReImg.DOFade(0.6f, 0.4f);
    }

    public override void Init()
    {
        unfoldOffsetLeft = new Vector2(-unfoldOffsetRight.x, unfoldOffsetRight.y);
        waiter = new WaitForSeconds(timeToSwitch);

        btnPauReImg = btnPauReRct.GetComponent<Image>();
        btnRestartImg = btnRestartRct.GetComponent<Image>();
        btnBackImg = btnBackRct.GetComponent<Image>();

    }
}
