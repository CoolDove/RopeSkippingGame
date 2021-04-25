using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UICStatic : UIController
{
    public override string UICName { get { return "UICStatic"; } }
    public override float closeWait { get { return 0.1f; } }

    [SerializeField]
    private Text ScoreText;
    [SerializeField]
    private Vector2 trtOutPosition;

    private Vector2 trtBackPosition;

    private RectTransform RTScoreText;

    private bool showScore;


    public override void GetData()
    {
        if (showScore)
        {
            ScoreText.text = GlobalVar.score.ToString();
        }
        else
        {
            ScoreText.text = "";
        }
    }

    private void ScoreTextOut()
    {
        RTScoreText.DOAnchorPos(trtOutPosition, 0.3f);
    }
    private void ScoreTextBack()
    {
        RTScoreText.DOAnchorPos(trtBackPosition, 0.3f);
    }

    public override void Init()
    {
        EventManager.Instance.SubscribeEvent("GameStart", () => showScore = true);
        EventManager.Instance.SubscribeEvent("BackToMenu", () => showScore = false);
        EventManager.Instance.SubscribeEvent("ScoreTextOut", ScoreTextOut);
        EventManager.Instance.SubscribeEvent("ScoreTextBack", ScoreTextBack);

        EventManager.Instance.SubscribeEvent("DifficultyIncrease", OnDifficultyIncrease);
        EventManager.Instance.SubscribeEvent("ScoreGet", OnScoreGet);

        RTScoreText = ScoreText.GetComponent<RectTransform>();
        trtBackPosition = RTScoreText.anchoredPosition;
    }

    public override void OnClose()
    {

    }

    public override void OnOpen()
    {

    }
    private void OnScoreGet()
    {
        RTScoreText.DORewind();
        RTScoreText.DOJumpAnchorPos(RTScoreText.anchoredPosition, 12f, 1, 0.3f);
    }
    private void OnDifficultyIncrease()
    {
        RTScoreText.DORewind();
        RTScoreText.DOJumpAnchorPos(RTScoreText.anchoredPosition, 16f, 1, 0.45f);
        RTScoreText.DOPunchScale(Vector3.one * 0.5f, 0.3f);

    }
    public void ExitGame()
    {
        Application.Quit();
    }

}
