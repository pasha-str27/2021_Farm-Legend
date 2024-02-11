using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemVideoReward : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text txtCoin;
    [SerializeField] Text txtCountAds;
    [SerializeField] Text txtTime;
    [SerializeField] GameObject btnAds;
    [SerializeField] GameObject bgTime;
    [SerializeField] Animator anim;
    ItemVideo itemVideo;
    Coroutine coroutine;
    private void OnDisable()
    {
        if (itemVideo != null)
            timeOutReward = Util.timeNow;
    }
    public void FillData(ItemVideo itemVideo)
    {
        this.itemVideo = itemVideo;
        icon.sprite = itemVideo.icon;
        icon.SetNativeSize();
        txtCoin.text = itemVideo.GetCoin + "";
        txtCountAds.text = countAdsView + "/" + itemVideo.countAds;
        txtCountAds.gameObject.SetActive(itemVideo.countAds > 0);
        btnAds.SetActive(timeLife <= 0);
        bgTime.SetActive(timeLife > 0);
        txtTime.text = Util.ConvertTime2(timeLife);
        Invoke("DelayAnim", (itemVideo.id + 1) * .1f);
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
        AdsManager.ShowVideoAds(() =>
        {
            Oncomplete();
        }, null,
        () => {
            AnalyticsManager.LogEvent("ads_reward_video_fail", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "coin_itemVideo", itemVideo.coin },
            { "countAds_itemVideo", itemVideo.countAds }
            });
        });
    }

    void Oncomplete()
    {
        if (itemVideo.countAds > 0)
        {
            countAdsView++;
            if (countAdsView >= itemVideo.countAds)
            {
                countAdsView = 0;
                switch (itemVideo.typeAds)
                {
                    case TypeAds.Coin:
                        CoinManager.AddCoin(itemVideo.GetCoin, transform, null, "ads");
                        AnalyticsManager.LogEvent("reward_ads", new Dictionary<string, object> {
            { "name_reward", "video_reward_coin_x5" },
            { "reward", itemVideo.GetCoin },
            { "level", PlayerPrefSave.Level },
            { "total_coin", DataManager.UserData.totalCoin },
            { "total_diamond", DataManager.UserData.totalDiamond } });
                        break;
                    case TypeAds.Diamond:
                        CoinManager.AddDiamond(itemVideo.GetCoin, transform, null, "ads");
                        AnalyticsManager.LogEvent("reward_ads", new Dictionary<string, object> {
            { "name_reward", "video_reward_diamond_x5" },
            { "reward", itemVideo.GetCoin },
            { "level", PlayerPrefSave.Level },
            { "total_coin", DataManager.UserData.totalCoin },
            { "total_diamond", DataManager.UserData.totalDiamond } });
                        break;
                }
            }
            FillData(itemVideo);
        }
        else
        {
            switch (itemVideo.typeAds)
            {
                case TypeAds.Coin:
                    CoinManager.AddCoin(itemVideo.GetCoin, transform, null, "ads");
                    AnalyticsManager.LogEvent("reward_ads", new Dictionary<string, object> {
            { "name_reward", "video_reward_coin_x1" },
            { "level", PlayerPrefSave.Level },
            { "reward", itemVideo.GetCoin },
            { "total_coin", DataManager.UserData.totalCoin },
            { "total_diamond", DataManager.UserData.totalDiamond } });
                    break;
                case TypeAds.Diamond:
                    CoinManager.AddDiamond(itemVideo.GetCoin, transform, null, "ads");
                    AnalyticsManager.LogEvent("reward_ads", new Dictionary<string, object> {
            { "name_reward", "video_reward_diamond_x1" },
            { "reward", itemVideo.GetCoin },
            { "level", PlayerPrefSave.Level },
            { "total_coin", DataManager.UserData.totalCoin },
            { "total_diamond", DataManager.UserData.totalDiamond } });
                    break;
            }
            timeLife = itemVideo.time;
            timeOutReward = Util.timeNow;
            FillData(itemVideo);
        }
    }
    int timeLife
    {
        get { return PlayerPrefs.GetInt("timeLife" + itemVideo.id, 0); }
        set { PlayerPrefs.SetInt("timeLife" + itemVideo.id, value); }
    }
    int timeOutReward
    {
        get { return PlayerPrefs.GetInt("timeOutVideoReward" + itemVideo.id, 0); }
        set { PlayerPrefs.SetInt("timeOutVideoReward" + itemVideo.id, value); }
    }

    public int countAdsView
    {
        get { return PlayerPrefs.GetInt("countAdsVideoReward" + itemVideo.id, 0); }
        set { PlayerPrefs.SetInt("countAdsVideoReward" + itemVideo.id, value); }
    }
}
