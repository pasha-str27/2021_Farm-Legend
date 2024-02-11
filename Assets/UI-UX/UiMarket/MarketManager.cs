using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance;
    public static bool IsCompliteSale => Instance.isCompliteSale;
    public static bool IsNotEmptySale => Instance.isNotEmptySale;
    public static int TimeNextSell => Instance.GetTimeNextSell;
    [SerializeField] CountDownTime countDown;
    [SerializeField] int[] timeOpenSell;
    [SerializeField] GameObject objUnlock;
    [SerializeField] GameObject objGranma;
    [SerializeField] GameObject[] particleUnlock;

    public Vector3 position { get { return transform.position; } }
    [ReadOnly] [SerializeField] bool isCompliteSale;
    [SerializeField] bool isNotEmptySale { get { return DataManager.MarketAsset.listSell.FirstOrDefault(x => x.nameProduct != "") != null; } }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (PlayerPrefSave.IsFirtOpen)
        {
            DataManager.MarketAsset.LoadNewProductBuy();
        }
        for (int i = 0; i < particleUnlock.Length; i++)
            particleUnlock[i].SetActive(false);

        LoadMarketSell();
        objUnlock.SetActive(PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockMarket);
        objGranma.SetActive(PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockMarket);
        //count down time sale
        if (isNotEmptySale)
            countDown.Init("market", DataManager.GameConfig.timeSaleMarket);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnSpeedUpMarket, OnSpeedUpMarketHandle);
        this.RegisterListener((int)EventID.OnLoadProductSale, OnLoadProductSaleHandle);
        this.RegisterListener((int)EventID.OnClaimCoinMarket, OnClaimCoinMarketHandle);
        this.RegisterListener((int)EventID.OnLevelUp, OnLevelUpHandle);
        this.RegisterListener((int)EventID.OnHidePopupLevelUp, OnHidePopupLevelUpHandle);
        this.RegisterListener((int)EventID.OnClickObject, OnClickObjectHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnSpeedUpMarket, OnSpeedUpMarketHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLoadProductSale, OnLoadProductSaleHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClaimCoinMarket, OnClaimCoinMarketHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLevelUp, OnLevelUpHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnHidePopupLevelUp, OnHidePopupLevelUpHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickObject, OnClickObjectHandle);
    }

    private void OnClickObjectHandle(object obj)
    {
        var msg = (MessageObject)obj;
        if (msg.type != ObjectMouseDown.Market)
            return;
        LoadMarketSell();
    }

    private void OnHidePopupLevelUpHandle(object obj)
    {
        if (PlayerPrefSave.Level == 3 && !PlayerPrefSave.IsTutorial)
        {
            this.PostEvent((int)EventID.OnClickObject, new MessageObject { pos = transform.position });
            for (int i = 0; i < particleUnlock.Length; i++)
                particleUnlock[i].SetActive(true);
        }
    }

    private void OnLevelUpHandle(object obj)
    {
        objUnlock.SetActive(PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockMarket);
        objGranma.SetActive(PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockMarket);
    }

    private void OnClaimCoinMarketHandle(object obj)
    {
        if (!isNotEmptySale)
        {
            isCompliteSale = false;
            countDown.timeLife = 0;
            countDown.isComplete = 0;
        }
    }

    void LoadMarketSell()
    {
        DateTime dateTime = DateTime.Now;
        //new day
        if (CurrentDay < dateTime.Day)
        {
            CurrentDay = dateTime.Day;
            for (int i = 0; i < timeOpenSell.Length; i++)
            {
                SetNewHour(timeOpenSell[i], 0);
            }
        }
        //load sell
        for (int i = 0; i < timeOpenSell.Length; i++)
        {
            if (dateTime.Hour >= timeOpenSell[i] && isNewHour(timeOpenSell[i]))
            {
                DataManager.MarketAsset.LoadNewProductBuy();
                SetNewHour(timeOpenSell[i], 1);
            }
        }
    }
    public int GetTimeNextSell
    {
        get
        {
            for (int i = 0; i < timeOpenSell.Length; i++)
            {
                if (isNewHour(timeOpenSell[i]))
                {
                    return timeOpenSell[i];
                }
            }
            return 0;
        }
    }
    private void OnLoadProductSaleHandle(object obj)
    {
        var msg = (ProductData)obj;
        if (countDown.timeLife == 0)
        {
            countDown.Init("market", DataManager.GameConfig.timeSaleMarket);
        }
        else
        {
            countDown.timeLife += msg.time / 3;
        }
    }

    private void OnSpeedUpMarketHandle(object obj)
    {
        var msg = (bool)obj;
        if (msg)
        {
            DataManager.MarketAsset.LoadNewProductBuy();
        }
        else
        {
            isCompliteSale = true;
            countDown.SpeedUp();
        }
    }
    public void EventCountDownTimeHandle()
    {
        //Debug.Log("=> EventCountDownTimeHandle " + countDown.timeLife);
        if (countDown.timeLife <= 0)
        {
            isCompliteSale = true;
            this.PostEvent((int)EventID.OnCompliteSale);
        }
        else
        {
            isCompliteSale = false;
        }
        this.PostEvent((int)EventID.OnCountDownTimeMarket, countDown.timeLife);
    }

    bool isNewHour(int Hour)
    {
        return PlayerPrefs.GetInt("isNewHour" + Hour, 0) == 0;
    }
    void SetNewHour(int Hour, int vl)
    {
        PlayerPrefs.SetInt("isNewHour" + Hour, vl);
    }
    int CurrentDay
    {
        set { PlayerPrefs.SetInt("CurrentDay", value); }
        get { return PlayerPrefs.GetInt("CurrentDay", 0); }
    }
}
