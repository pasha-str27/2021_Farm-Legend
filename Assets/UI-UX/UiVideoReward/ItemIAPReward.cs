using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ItemIAPReward : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text txtCoin;
    [SerializeField] Text txtCountAds;
    [SerializeField] Text txtTime;
    [SerializeField] GameObject btnAds;
    [SerializeField] GameObject bgTime;
    [SerializeField] Animator anim;
    ItemIAP itemIAP;
    Coroutine coroutine;

    private void OnDisable()
    {
        if (itemIAP != null)
            timeOutReward = Util.timeNow;
    }

    public void FillData(ItemIAP itemIAP)
    {
        this.itemIAP = itemIAP;
        icon.sprite = itemIAP.icon;
        icon.SetNativeSize();
        txtCoin.text = itemIAP.GetCoin + "";
        txtCountAds.text = IAPManager.Instance.GetItemPrice(itemIAP.IAPID);
        //txtCountAds.gameObject.SetActive(itemIAP.countAds > 0);
        btnAds.SetActive(timeLife <= 0);
        bgTime.SetActive(timeLife > 0);
        txtTime.text = Util.ConvertTime2(timeLife);
        Invoke("DelayAnim", (itemIAP.id + 1) * .1f);
        ShowCountTime();
    }

    void DelayAnim()
    {
        if (anim != null)
            anim.SetTrigger("show");
    }
    void ShowCountTime()
    {
        if (timeLife > 0)
        {
            timeLife -= (Util.timeNow - timeOutReward);
        }

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(CountTimeReward());
    }
    IEnumerator CountTimeReward()
    {
        yield return new WaitForSecondsRealtime(1);

        txtTime.text = Util.ConvertTime2(timeLife);
        btnAds.SetActive(timeLife <= 0);
        bgTime.SetActive(timeLife > 0);
        yield return new WaitForSecondsRealtime(1);
        if (timeLife > 0)
        {
            timeLife--;
            coroutine = StartCoroutine(CountTimeReward());
        }
    }

    public void On_Click()
    {
        if (timeLife > 0)
            return;

        IAPManager.Instance.BuyConsumable(itemIAP.IAPID, Oncomplete);
    }

    void Oncomplete(string productID, bool result, PurchaseFailureReason failureReason)
    {
        if (!result)
            return;

        switch (itemIAP.typeAds)
        {
            case TypeAds.Coin:
                CoinManager.AddCoin(itemIAP.GetCoin, transform, null, "ads");
                AnalyticsManager.LogEvent("reward_ads", new Dictionary<string, object> {
        { "name_reward", "video_reward_coin_x1" },
        { "level", PlayerPrefSave.Level },
        { "reward", itemIAP.GetCoin },
        { "total_coin", DataManager.UserData.totalCoin },
        { "total_diamond", DataManager.UserData.totalDiamond } });
                break;
            case TypeAds.Diamond:
                CoinManager.AddDiamond(itemIAP.GetCoin, transform, null, "ads");
                AnalyticsManager.LogEvent("reward_ads", new Dictionary<string, object> {
        { "name_reward", "video_reward_diamond_x1" },
        { "reward", itemIAP.GetCoin },
        { "level", PlayerPrefSave.Level },
        { "total_coin", DataManager.UserData.totalCoin },
        { "total_diamond", DataManager.UserData.totalDiamond } });
                break;
        }
        timeLife = itemIAP.time;
        timeOutReward = Util.timeNow;
        
        Invoke(nameof(UpdateInfo), 1);
    }

    void UpdateInfo() => FillData(itemIAP);

    int timeLife
    {
        get { return PlayerPrefs.GetInt("timeLife" + itemIAP.id, 0); }
        set { PlayerPrefs.SetInt("timeLife" + itemIAP.id, value); }
    }
    int timeOutReward
    {
        get { return PlayerPrefs.GetInt("timeOutVideoReward" + itemIAP.id, 0); }
        set { PlayerPrefs.SetInt("timeOutVideoReward" + itemIAP.id, value); }
    }

    public int countAdsView
    {
        get { return PlayerPrefs.GetInt("countAdsVideoReward" + itemIAP.id, 0); }
        set { PlayerPrefs.SetInt("countAdsVideoReward" + itemIAP.id, value); }
    }
}
