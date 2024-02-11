using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    [SerializeField]  Transform Target;
    [SerializeField] float distancePosWin;
    [SerializeField] float offsetX = -1;
    Vector3 tempPos;
    Vector3 posWin;
    private void Awake()
    {
        instance = this;
        tempPos = transform.position;
    }
    public void RestPosition()
    {
        transform.position = tempPos;
    }

    public void SetTarget(Transform tf, Vector3 posWin)
    {
        Target = tf;
        this.posWin = posWin;
    }
    private void LateUpdate()
    {
        if (Target)
        {
            transform.position = new Vector3(Target.position.x + offsetX, Target.position.y, -10);
            if (Vector3.Distance(new Vector3(Target.position.x + offsetX, Target.position.y, 0), posWin) <= distancePosWin)
                Target = null;
        }
    }
}
