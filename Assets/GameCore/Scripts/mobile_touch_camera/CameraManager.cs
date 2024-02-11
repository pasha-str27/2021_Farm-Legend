using BitBenderGames;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] MobileTouchCamera mobileTouchCamera;
    [SerializeField] Transform cameraFarm;
    [SerializeField] Ease ease = Ease.OutQuad;
    bool isZoomCam = false;
    public float sizeZoom = 3;
    public float speedZoom = 5;
    float tempSpeedZoom = 0;
    float tempTimeZoom = .5f;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnClickObject, OnOjectClickHanlde);
        this.RegisterListener((int)EventID.OnLockCamera, OnLockCameraHanlde);
        this.RegisterListener((int)EventID.OnZoomCamera, OnZoomCameraHanlde);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickObject, OnOjectClickHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLockCamera, OnLockCameraHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnZoomCamera, OnZoomCameraHanlde);
    }
    private void OnZoomCameraHanlde(object obj)
    {
        var msg = (bool)obj;
        tempSpeedZoom = speedZoom;
        isZoomCam = msg;
        MobileTouchCamera.isZoomCam = msg;
    }

    private void OnLockCameraHanlde(object obj)
    {
        var msg = (bool)obj;
        bool isTutorialLock = false;
        if (PlayerPrefSave.stepTutorial < 3)
            isTutorialLock = true;
        if (!isTutorialLock)
            MobileTouchCamera.LockCam = msg;
        else MobileTouchCamera.LockCam = true;
        //Debug.Log("=>  OnLockCamera " + msg);
    }

    private void Update()
    {
        if (isZoomCam)
        {
            if (cameraFarm.GetComponent<Camera>().orthographicSize > sizeZoom)
                cameraFarm.GetComponent<Camera>().orthographicSize -= Time.deltaTime * tempSpeedZoom;
            else tempSpeedZoom = 0;
        }
    }
    private void OnOjectClickHanlde(object obj)
    {
        var msg = (MessageObject)obj;
        Vector3 pos = msg.pos;
        pos.z = -10;
        mobileTouchCamera.SetTargetPosition(pos);
        cameraFarm.DOMove(pos, tempTimeZoom).SetEase(ease).OnComplete(() =>
       {
           msg.callBack?.Invoke();
       });
    }
}
