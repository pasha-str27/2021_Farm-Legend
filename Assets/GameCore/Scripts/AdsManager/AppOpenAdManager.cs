using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ADMOB
using GoogleMobileAds.Api;
#endif
public class AppOpenAdManager : MonoBehaviour
{
    [SerializeField] string AD_UNIT_ID_Android = "ca-app-pub-3940256099942544/3419835294";
    [SerializeField] string AD_UNIT_ID_Ios = "ca-app-pub-3940256099942544/5662855259";
    string AD_UNIT_ID = "unexpected_platform";
    private static AppOpenAdManager instance;
#if USE_ADMOB
    private AppOpenAd ad;
#endif
    private bool isShowingAd = false;
    private DateTime loadTime;

    public static AppOpenAdManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AppOpenAdManager();
            }

            return instance;
        }
    }

    private bool IsAdAvailable
    {
        get
        {
#if USE_ADMOB
            return ad != null && (System.DateTime.UtcNow - loadTime).TotalHours < 4;
#elif !USE_ADMOB
            return false;
#endif
        }
    }
    public void Start()
    {
        LoadAd();
    }

    public void LoadAd()
    {
#if USE_ADMOB
        AdRequest request = new AdRequest.Builder().Build();
#if UNITY_ANDROID
        AD_UNIT_ID = AD_UNIT_ID_Android;
#elif UNITY_IOS
    AD_UNIT_ID = AD_UNIT_ID_Ios;
#endif

        MobileAds.Initialize(initStatus => { Debug.Log(initStatus.ToString()); });

        // Load an app open ad for portrait orientation
        //AppOpenAd.LoadAd(AD_UNIT_ID, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
        //{
        //    if (error != null)
        //    {
        //        // Handle the error.
        //        Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
        //        return;
        //    }

        //    // App open ad is loaded.
        //    ad = appOpenAd;
        //    loadTime = DateTime.UtcNow;
        //}));
#endif
    }

//    public void ShowAdIfAvailable()
//    {
//#if UNITY_EDITOR
//        return;
//#endif
//#if USE_ADMOB
//        if (!IsAdAvailable || isShowingAd)
//        {
//            return;
//        }
//        ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
//        ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
//        ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
//        ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
//        ad.OnPaidEvent += HandlePaidEvent;
//        ad.Show();
//#endif
//    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
#if USE_ADMOB
        ad = null;
#endif
        isShowingAd = false;
        LoadAd();
    }
#if USE_ADMOB
    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.

        ad = null;

        LoadAd();
    }


    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Displayed app open ad");
        isShowingAd = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                args.AdValue.CurrencyCode, args.AdValue.Value);
    }
#endif
    public void OnApplicationPause(bool paused)
    {
        if (!paused && instance != null)
        {
            //ShowAdIfAvailable();
        }
    }
}