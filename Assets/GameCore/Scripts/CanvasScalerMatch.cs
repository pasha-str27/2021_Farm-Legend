using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerMatch : MonoBehaviour
{
    [SerializeField] Camera UiCamera;
    [SerializeField] CanvasScaler[] arAanvasScaler;
    [Tooltip("man may tinh bang")] public float ipad = .3f;
    [Tooltip("man dien thoai dài")] public float phoneNomal = .65f;
    [Tooltip("man dien thoai dài")] public float phoneHight = .5f;
    [Tooltip("frame fps")] public int fps = 60;
    private void OnEnable()
    {
        Debug.Log("=> CanvasScalerMatch = " + UiCamera.aspect);
        //foreach (CanvasScaler scaler in arAanvasScaler)
        //    scaler.matchWidthOrHeight = UiCamera.aspect;
        //return;
        if (UiCamera.aspect <= 1.4f)
        {
            Debug.Log("man may tinh bang");
            foreach (CanvasScaler scaler in arAanvasScaler)
                scaler.matchWidthOrHeight = ipad;
        }
        else
        {
            if (UiCamera.aspect < 1.7f)
            {
                foreach (CanvasScaler scaler in arAanvasScaler)
                    scaler.matchWidthOrHeight = phoneHight;
            }
            else
            {
                Debug.Log("man dien thoai");
                foreach (CanvasScaler scaler in arAanvasScaler)
                    scaler.matchWidthOrHeight = phoneNomal;
            }
        }

        Application.targetFrameRate = fps;
    }
}
