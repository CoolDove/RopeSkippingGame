using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class CamCtrl : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float _blurLevel;

    public float BlurLevel
    {
        get
        {
            return _blurLevel;
        }
        set
        {
            value = Mathf.Clamp01(value);
            _blurLevel = value;
        }
    }
    
    private Camera cam;

    public bool bluring { get { return _blurLevel > 0f; } }

    private void OnEnable()
    {
        cam = Camera.main;
        EventManager.Instance.SubscribeEvent("ShakeCamera", Shake);
    }
    private void Update()
    {
        cam.orthographicSize = Mathf.Lerp(1.5f, 7.1f, 1 - BlurLevel);
    }
    
    public void Shake()
    {
        cam.DOShakePosition(0.15f, 1f, 9, 90, true);
    }

}