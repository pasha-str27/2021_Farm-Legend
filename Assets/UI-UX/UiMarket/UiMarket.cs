using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UiMarket : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] Text txtName;
    [SerializeField] Text txtDiamond;
    [SerializeField] ItemMarket[] itemMarket;
    [SerializeField] GameObject objTime;
    [SerializeField] Text txtTime;
    [SerializeField] Text txtDesTime;
    [SerializeField] GameObject objEmpty;
    [SerializeField] int diamondSpeedBuy = 5;
    [SerializeField] int diamondSpeedSell = 5;
    [SerializeField] bool isBuy = false;
    [ReadOnly] [SerializeField] List<DataMarket> dataMarkets;

    bool fillNextSlot = false;
    string tempTimeSell = "";
    int diamondSale = 0;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnUnlockSlotMarket, OnUnlockSlotMarketHandle);
        this.RegisterListener((int)EventID.OnClickButtonTab, OnClickButtonTabHandle);
        this.RegisterListener((int)EventID.OnSpeedUpMarket, OnSpeedUpMarketHandle, DispatcherType.Late);
        this.RegisterListener((int)EventID.OnCountDownTimeMarket, OnCountDownTimeMarketHandle);
        this.RegisterListener((int)EventID.OnClaimCoinMarket, OnClaimCoinMarketHandle);
        this.RegisterListener((int)EventID.OnCompliteSale, OnCompliteSaleHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUnlockSlotMarket, OnUnlockSlotMarketHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickButtonTab, OnClickButtonTabHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnSpeedUpMarket, OnSpeedUpMarketHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCountDownTimeMarket, OnCountDownTimeMarketHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClaimCoinMarket, OnClaimCoinMarketHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompliteSale, OnCompliteSaleHandle);
    }

    private void OnCompliteSaleHandle(object obj)
    {
        FillData();
    }

    private void OnClaimCoinMarketHandle(object obj)
    {
        LoadEmpty();
    }

    private void OnCountDownTimeMarketHandle(object obj)
    {
        var msg = (int)obj;
        if (!isBuy)
        {
            diamondSale = DataManager.GameConfig.GetDiamondTime(msg);
            txtDiamond.text = diamondSale + "";
            txtTime.text = Util.ConvertTime(msg);
        }
    }

    private void OnSpeedUpMarketHandle(object obj)
    {
        FillData();
    }

    private void OnClickButtonTabHandle(object obj)
    {
        var msg = (MessagerTab)obj;
        if (msg.tabName == TabName.BuyMarket)
        {
            isBuy = true;
            FillData();
        }
        else
        if (msg.tabName == TabName.SaleMarket)
        {
            isBuy = false;
            FillData();
        }
    }

    private void OnUnlockSlotMarketHandle(object obj)
    {
        isBuy = (bool)obj;
        FillData();
    }

    public void Show(TabName tabName)
    {

        uIAnimation.Show(() =>
        {
            this.PostEvent((int)EventID.OnClickButtonTab, new MessagerTab { tabName = tabName });
        });
        //FillData();
    }
    public void Hide()
    {
        uIAnimation.Hide();
    }
    void FillData()
    {
        DisableAllItem();
        objTime.SetActive(false);
        objEmpty.SetActive(false);
        txtName.text = isBuy ? DataManager.LanguegesAsset.GetName("Table Buy") : DataManager.LanguegesAsset.GetName("Table Sell");
        txtDiamond.text = (isBuy ? diamondSpeedBuy : diamondSpeedSell) + "";
        dataMarkets = isBuy ? DataManager.MarketAsset.listBuy : DataManager.MarketAsset.listSell;
        fillNextSlot = false;
        for (int i = 0; i < dataMarkets.Count; i++)
        {
            if (dataMarkets[i].unlocked)
            {
                itemMarket[i].gameObject.SetActive(true);
                itemMarket[i].FillData(dataMarkets[i], isBuy);
            }
            else
            if (!fillNextSlot)
            {
                fillNextSlot = true;
                itemMarket[i].gameObject.SetActive(true);
                itemMarket[i].FillData(dataMarkets[i], isBuy);
            }
        }
        LoadEmpty();
    }
    void LoadEmpty()
    {
        if (isBuy)
        {
            tempTimeSell = Util.ConvertTime24H(MarketManager.TimeNextSell);
            txtDesTime.text = DataManager.LanguegesAsset.GetName("New goods will be available at");
            txtTime.text = tempTimeSell == "" ? DataManager.LanguegesAsset.GetName("Open next day") : tempTimeSell;
            objTime.SetActive(true);
            objEmpty.SetActive(false);
        }
        else
        {
            txtDesTime.text = DataManager.LanguegesAsset.GetName("Time to sell");
            if (MarketManager.IsNotEmptySale && !MarketManager.IsCompliteSale)
            {
                objTime.SetActive(true);
                objEmpty.SetActive(false);
            }
            else
            {
                objTime.SetActive(false);
                objEmpty.SetActive(true);
            }
        }
    }
    void DisableAllItem()
    {
        for (int i = 0; i < itemMarket.Length; i++)
        {
            itemMarket[i].gameObject.SetActive(false);
        }
    }
    public void Btn_SpeedUp_Click()
    {
        if (CoinManager.totalDiamond >= (isBuy ? diamondSpeedBuy : diamondSale))
        {
            CoinManager.AddDiamond(-(isBuy ? diamondSpeedBuy : diamondSale), null, null, "nosound");
            //this.PostEvent((int)EventID.OnFxPutDiamond, txtDiamond.transform.position);
        }
        else
        {
            UIToast.Show("Not enough diamond!", null, ToastType.Notification, 1.5f);
            this.PostEvent((int)EventID.OnShowVideoReward);
            return;
        }

        this.PostEvent((int)EventID.OnSpeedUpMarket, isBuy);
    }
}
