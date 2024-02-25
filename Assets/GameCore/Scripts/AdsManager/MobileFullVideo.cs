using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ADMOB
using GoogleMobileAds.Api;
#endif
using System;
public class MobileFullVideo : MonoBehaviour
{
    [SerializeField] float timeToShowAdvertisement = 300;
    [SerializeField] float liveTimeRemoveAdsButton = 15;
    [SerializeField] GameObject removeAdsButton;

    Action onComplite;
    public string adUnitIdAndroid = "ca-app-pub-2860978374448480/4042200052";
    public string adUnitIdIos = "ca-app-pub-3940256099942544/1033173712";
    bool isTest = false;
#if USE_ADMOB
    //full Video Ad
    private static InterstitialAd interstitial;
    string idTest = "ca-app-pub-2860978374448480/4042200052";
#endif

    void Awake()
    {
        Debug.Log(name);

#if USE_ADMOB
        MobileAds.SetiOSAppPauseOnBackground(true);

        MobileAds.Initialize(initStatus => { });

        RequestInterstitial();
#endif
    }

    IEnumerator Start()
    {
        var waiter = new WaitForSeconds(timeToShowAdvertisement);

        while (true)
        {
            yield return waiter;

            if(PlayerPrefs.GetInt("no_ads", 0) == 1)
                yield break;

            ShowFullNormal(() => StartCoroutine(ShowRemoveAdsButton()), null);
        }
    }

    IEnumerator ShowRemoveAdsButton()
    {
        removeAdsButton.SetActive(true);
        yield return new WaitForSeconds(liveTimeRemoveAdsButton);
        removeAdsButton.SetActive(false);
    }

#if USE_ADMOB
    public void RequestInterstitial()
    {
        try
        {
#if UNITY_ANDROID
            string adUnitId = adUnitIdAndroid;
#elif UNITY_IPHONE
        string adUnitId = adUnitIdIos;
#endif
            if (isTest)
                adUnitId = idTest;


            // Clean up the old ad before loading a new one.
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }

            Debug.Log("Loading the interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            InterstitialAd.Load(adUnitId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);

                        Invoke(nameof(RequestInterstitial), 5f);

                        return;
                    }

                    Debug.Log("Interstitial ad loaded with response : "
                              + ad.GetResponseInfo());

                    RegisterEventHandlers(ad);

                    interstitial = ad;
                });

            //if (interstitial != null)
            //{
            //    interstitial.Destroy();
            //}
            //// Initialize an InterstitialAd.
            //interstitial = new InterstitialAd(adUnitId);

            //interstitial.OnAdLoaded += HandleOnAdLoaded;
            //interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            //interstitial.OnAdOpening += HandleOnAdOpened;
            //interstitial.OnAdClosed += HandleOnAdClosed;

            //AdRequest request = new AdRequest.Builder().Build();
            //// Load the interstitial with the request.
            //interstitial.LoadAd(request);
            Debug.Log("[Ad - interstitial] RequestInterstitial");
        }
        catch
        {

        }
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            HandleOnAdClosed();
            onComplite?.Invoke();
            Debug.Log("Interstitial ad full screen content closed.");
            RequestInterstitial();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            RequestInterstitial();
        };
    }

    public void HandleOnAdClosed()
    {
        //interstitial.OnAdLoaded -= HandleOnAdLoaded;
        //interstitial.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
        //interstitial.OnAdOpening -= HandleOnAdOpened;
        //interstitial.OnAdClosed -= HandleOnAdClosed;
        //RequestInterstitial();
        //onComplite?.Invoke();
        Debug.Log("[Ad - interstitial] HandleOnAdClosed");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {

    }

#endif

    public void ShowFullNormal(Action onComplite, Action onFail)
    {
        this.onComplite = onComplite;
//#if UNITY_EDITOR || !USE_ADMOB
//        if(onComplite != null)
//        onComplite.Invoke();
//        return;
//#endif
#if USE_ADMOB
        try
        {
            if(PlayerPrefs.GetInt("no_ads", 0) == 1)
            {
                this.onComplite?.Invoke();
                return;
            }

            if (interstitial.CanShowAd())
            {
                interstitial.Show();
                Debug.Log("[Ad - interstitial] ShowVideoAds true");
            }
            else
            {
        if(onFail != null)
                onFail?.Invoke();
        if(onComplite != null)
                this.onComplite?.Invoke();
                Debug.Log("[Ad - interstitial] ShowVideoAds false");
            }
        }
        catch
        {
        if(onComplite != null)
            this.onComplite?.Invoke();
        }
#endif
    }
}
