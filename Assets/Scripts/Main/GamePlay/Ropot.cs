using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dove.Core;

public class Ropot : MonoBehaviour
{
    [SerializeField]
    private Rope rope;
    [SerializeField]
    private Transform armA;
    [SerializeField]
    private Transform armB;

    private float progress { get { return GlobalVar.ropeProgress; } }

    protected void LateUpdate()
    {
        Quaternion q1 = Quaternion.Euler(0, 0, 360 * progress);
        Quaternion q2 = Quaternion.Euler(0, 0, -360 * progress);

        if (armA != null) 
        {
            armA.transform.localRotation = q1;
        }
        if (armB != null) 
        {
            armB.transform.localRotation = q2;
        }
    }

}
