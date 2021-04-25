using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CameraArm : MonoBehaviour
{
    [SerializeField]
    float armLength = 1;


    Camera cam;




    void Update()
    {
        if (cam == null) 
        {
            cam = GetComponentInChildren<Camera>();
            return;
        }

        cam.transform.localPosition = new Vector3(0, 0, -armLength);
        Debug.DrawLine(transform.position, cam.transform.position,Color.red);

    }
}
