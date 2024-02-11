using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarborManager : MonoBehaviour
{
    public static HarborManager Instance;
    [SerializeField] public Transform harbor;
    [SerializeField] ShipController shipController;
    [ReadOnly] [SerializeField] int timeCount = 0;
    [SerializeField] GameObject[] partical;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ActivePartical(false);
        Invoke("DelayLoadOrder", 3f);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnStartCountDownHarbor, OnStartCountDownHarborHandle);
        this.RegisterListener((int)EventID.OnSpeedUpHarbor, OnSpeedUpHarborHandle);
        this.RegisterListener((int)EventID.OnClickObject, OnClickObjectHandle);
        this.RegisterListener((int)EventID.OnHidePopupLevelUp, OnHidePopupLevelUpHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnStartCountDownHarbor, OnStartCountDownHarborHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnSpeedUpHarbor, OnSpeedUpHarborHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickObject, OnClickObjectHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnHidePopupLevelUp, OnHidePopupLevelUpHandle);
    }

    private void OnHidePopupLevelUpHandle(object obj)
    {
        if (GameUIManager.BuildMarketing == BuildMarketing.Farm)
            DelayLoadOrder();
    }

    private void OnClickObjectHandle(object obj)
    {
        var msg = (MessageObject)obj;
        if (msg.type == ObjectMouseDown.Harbor)
            this.PostEvent((int)EventID.OnShowHandTutorial, false);
    }

    private void OnSpeedUpHarborHandle(object obj)
    {
        timeLife = 0;
        StartCoroutine(CountDownTime());
    }

    private void OnStartCountDownHarborHandle(object obj)
    {
        var time = (int)obj;
        timeLife = time;
        if(GameUIManager.IsTest)
            timeLife = 30;
        StartCoroutine(CountDownTime());
    }
    void ActivePartical(bool isActive)
    {
        for (int i = 0; i < partical.Length; i++)
        {
            partical[i].SetActive(isActive);
        }
    }
    void DelayLoadOrder()
    {
        if (PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockOrderHarbor)
        {
            DataManager.OrderHarborAsset.LoadOrder();
            if (DataManager.OrderHarborAsset.list.Count == 0)
            {
                DataManager.OrderHarborAsset.CreatOrder();
            }

            if (isNewUnlock)
            {
                isNewUnlock = false;
                ActivePartical(true);
                
                this.PostEvent((int)EventID.OnClickObject, new MessageObject
                {
                    pos = harbor.position,
                    callBack = () => { this.PostEvent((int)EventID.OnShowHandTutorial, true); }
                });
            }
        }

        if (shipController.isRun)
        {
            if (timeLife > 0)
            {
                timeLife -= Util.timeOffline;
            }
            if (timeLife <= 0)
            {
                shipController.GiftReady();
            }
            else
            {
                shipController.Shiping();
                StartCoroutine(CountDownTime());
            }
        }
    }

    IEnumerator CountDownTime()
    {
        if (timeLife <= 0)
        {
            this.PostEvent((int)EventID.OnShipBack);
        }
        timeCount = timeLife;
        this.PostEvent((int)EventID.OnUpdateTimeHarbor, timeLife);
        yield return new WaitForSeconds(1);
        if (timeLife > 0)
        {
            timeLife--;
            StartCoroutine(CountDownTime());
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.B))
        {
            this.PostEvent((int)EventID.OnShipBack);
        }
#endif
    }

    public int timeLife
    {
        set
        {
            PlayerPrefs.SetInt("time_sendOrder_harbor", value);
        }
        get
        {
            return PlayerPrefs.GetInt("time_sendOrder_harbor", 0);
        }
    }
    bool isNewUnlock
    {
        get { return PlayerPrefs.GetInt("isNewUnlock_harbor", 0) == 0; }
        set { PlayerPrefs.SetInt("isNewUnlock_harbor", value == true ? 0 : 1); }
    }
}
