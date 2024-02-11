using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;
    [SerializeField] string app_id_Adnroid = "";
    [SerializeField] string app_id_Ios = "";
    [SerializeField] MobileFullVideo mobileFull;
    [SerializeField] MobileRewardVideoAd mobileReward;
    [SerializeField] AppOpenAdManager appOpenAd;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static void ShowVideoAds(Action onComplite, Action onClose, Action onFailed)
    {
        AnalyticsManager.LogEvent("video_reward_call", null);
        instance.mobileReward.ShowVideoAds(onComplite, onClose, onFailed);
    }
    public static void ShowFullNormal(Action onComplite, Action onFailed)
    {
        AnalyticsManager.LogEvent("interstitial_call", null);
        instance.mobileFull.ShowFullNormal(onComplite, onFailed);
    }
    public static void ShowAdOpening()
    {
        AnalyticsManager.LogEvent("ad_opening_call", null);
        //instance.appOpenAd.ShowAdIfAvailable();
    }
}
