using UnityEngine;
using System.Collections;
#if USE_ADMOB
using GoogleMobileAds.Api;
#endif
using System;

public class MobileRewardVideoAd : MonoBehaviour
{
    string idTest = "ca-app-pub-3940256099942544/5224354917";
    public string IdRewardedVideoAndroid = "ca-app-pub-2860978374448480/7612374431"; // Ban goc
    public string IdRewardedVideoIOs = "ca-app-pub-3940256099942544/5224354917"; // Ban goc
    bool isTest = false;
#if USE_ADMOB
    private RewardedAd rewardAd;
#endif
    string adUnitId = "";

    Action onComplite, onClose, onFailed;

    private void Start()
    {
        RequestRewardedVideo();
    }

    void RequestRewardedVideo()
    {
#if USE_ADMOB
#if UNITY_ANDROID
        adUnitId = IdRewardedVideoAndroid;
#elif UNITY_IPHONE
        adUnitId = IdRewardedVideoIOs;
#endif
        if (isTest)
            adUnitId = idTest;

        // Clean up the old ad before loading a new one.
        if (rewardAd != null)
        {
            rewardAd.Destroy();
            rewardAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);

                    //OnAdFailedToLoad();

                    Invoke(nameof(RequestRewardedVideo), 10);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                OnAdLoaded();

                RegisterEventHandlers(ad);

                rewardAd = ad;
            });

        //rewardAd = new RewardedAd(adUnitId);
        //AdRequest request = new AdRequest.Builder().Build();
        //rewardAd.LoadAd(request);
        Debug.Log("[Ad - reward] RequestRewardedVideo");
#endif
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Rewarded ad Paid");
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            OnAdClosed();
            Debug.Log("Rewarded ad full screen content closed.");
            RequestRewardedVideo();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    void OnAdLoaded()
    {
        //Debug.Log("[Ad - reward] OnAdLoaded");
    }

    void OnAdFailedToLoad(object sender, System.EventArgs eventArgs)
    {
        //if (onFailed != null)
        //    onFailed.Invoke();
        Debug.Log("[Ad - reward] OnAdFailedToLoad");
    }

    void OnAdStarted(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("[Ad - reward] OnAdStarted");
    }

    void OnAdClosed()
    {
        //tắt video
        if(onClose != null)
            onClose.Invoke();
        //RemovEvent();
        //RequestRewardedVideo();
        Debug.Log("[Ad - reward] OnAdClosed");
    }

    //void OnAdRewarded(object sender, System.EventArgs eventArgs)
    //{
    //    //trả thưởng
    //    //Debug.Log("On xem xong Video !");
    //    if (onComplite != null)
    //        onComplite.Invoke();
    //    Debug.Log("[Ad - reward] OnAdRewarded");
    //}

    public void ShowVideoAds(Action onComplite, Action onClose, Action onFailed)
    {
        this.onComplite = onComplite;
        this.onClose = onClose;
        this.onFailed = onFailed;
//#if UNITY_EDITOR || !USE_ADMOB
//        Debug.Log("[Ad - reward] ShowVideoAds EDITOR");
//        onComplite.Invoke();
//        return;
//#endif
#if USE_ADMOB
        Debug.Log("[Ad - reward] ShowVideoAds ADMOB");
        if (rewardAd.CanShowAd() == false)
        {
            onFailed?.Invoke();

            UIToast.Show(DataManager.LanguegesAsset.GetName("Video Reward not ready, please try again...!"), null, ToastType.Notification, 1.5f);
            RequestRewardedVideo();
            Debug.Log("[Ad - reward] ShowVideoAds failed");
        }
        else
        {
            rewardAd.Show((_) => onComplite.Invoke());
            //rewardAd.Show();
            Debug.Log("[Ad - reward] ShowVideoAds true");
        }
#endif
    }
}
