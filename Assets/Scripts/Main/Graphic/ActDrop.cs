using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ActDrop : MonoBehaviour
{
    public bool In { get; private set; }

    private bool clear = true;

    private static ActDrop _instance;
    public static ActDrop Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private float inTime;
    [SerializeField]
    private float outTime;
    [SerializeField]
    private float blurTime;
    [SerializeField]
    private float clearTime;

    private CamCtrl camEft;

    private WaitForSeconds inWaiter;
    private WaitForSeconds outWaiter;
    private WaitForSeconds blurWaiter;
    private WaitForSeconds clearWaiter;
    private WaitForSeconds smallWaiter;

    private void Awake()
    {
        _instance = this;
        camEft = Camera.main.GetComponent<CamCtrl>();
        inWaiter = new WaitForSeconds(inTime);
        outWaiter = new WaitForSeconds(outTime);
        blurWaiter = new WaitForSeconds(blurTime);
        clearWaiter = new WaitForSeconds(clearTime);
        smallWaiter = new WaitForSeconds(0.1f);

    }

    public void ActIn()
    {
        if (!In) StartCoroutine(ActInCo());
    }
    private IEnumerator ActInCo()
    {
        DOTween.To(() => camEft.BlurLevel, x => camEft.BlurLevel = x, 1, inTime);
        yield return inWaiter;
        EventManager.Instance.SendEvent("StartPrepare");
        yield return smallWaiter;
        In = true;
    }
    public void ActOut()
    {
        if (In) StartCoroutine(ActOutCo());
    }
    private IEnumerator ActOutCo()
    {
        DOTween.To(() => camEft.BlurLevel, x => camEft.BlurLevel = x, 0, outTime);
        yield return outWaiter;
        In = false;
    }
    public void Blur()
    {
        if (clear) StartCoroutine(BlurCo());
    }
    private IEnumerator BlurCo()
    {
        DOTween.To(() => camEft.BlurLevel, x => camEft.BlurLevel = x, 0.5f, inTime).SetEase(Ease.InBack);
        yield return blurWaiter;
        clear = false;
    }
    public void Clear()
    {
        if (!clear) StartCoroutine(ClearCo());
    }
    private IEnumerator ClearCo()
    {
        DOTween.To(() => camEft.BlurLevel, x => camEft.BlurLevel = x, 0, inTime);
        yield return inWaiter;
        clear = true;
    }
}
