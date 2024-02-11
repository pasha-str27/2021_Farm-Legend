﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class UserData : UserAnalysic
{
    [Header("Data")]
    public int level = 0;
    public int rank = 0;
    public float health = 100;
    public float power = 20;
    public int income = 0;

    public float stepTutorial = 0;



    private string lastTimeUpdate = new DateTime(1999, 1, 1).ToString();
    public DateTime LastTimeUpdate
    {
        get => DateTimeConverter.ToDateTime(lastTimeUpdate);
        set => lastTimeUpdate = value.ToString();
    }
    private string fistTimeOpen = DateTime.Now.ToString();
    public DateTime FistTimeOpen
    {
        get => DateTimeConverter.ToDateTime(fistTimeOpen);
        set => fistTimeOpen = value.ToString();
    }
    private bool _isRemovedAds;
    public bool isRemovedAds
    {
        get => _isRemovedAds;
        set
        {
            if (value != _isRemovedAds)
            {
                _isRemovedAds = value;
#if USE_IRON || USE_MAX || USE_ADMOB
               // AdsManager.SetArea();
#endif
            }
        }
    }

    private float _scaleCoin = 1;
    public float scaleCoin
    {
        get
        {
            if (_scaleCoin < 1 || _scaleCoin >= 2)
                _scaleCoin = 1;
            return _scaleCoin;
        }
        set
        {
            if (value != _scaleCoin)
            {
                _scaleCoin = value;
            }
        }
    }

    private long _limitedDateTime;

    public double limitedPassTimeCountDown;

    public long limitedDateTime
    {
        get
        {
#if !UNITY_EDITOR
            if (_limitedDateTime > DateTime.Now.AddDays(30).Ticks)
#else
            if (_limitedDateTime > DateTime.Now.AddDays(6).Ticks)
#endif
                _limitedDateTime = DateTime.Now.AddMinutes(1).Ticks;
            return _limitedDateTime;
        }
        set
        {
            if (_limitedDateTime != value && value > 0)
            {
                _limitedDateTime = value;
#if USE_FIREBASE
                FirebaseHelper.SetUser("Limited", _limitedDateTime);
#endif
#if USE_IRON || USE_MAX || USE_ADMOB
                //AdsManager.SetArea();
#endif
            }
        }
    }

    [Header("Money")]
    [SerializeField]
    private int coin = 0;
    public int totalCoin
    {
        get => coin;
        set
        {
#if USE_ADMOB
Debug.LogError("Admob enable");
#elif !USE_ADMOB
            Debug.LogError("Admob disabled");
#endif

            if (coin < 2000000000)
            {
                if (coin != value)
                {
                    int changed = 0;
                    if (coin > value)
                    {
                        changed = coin - value;
                        totalCoinSpend += changed;
                    }
                    else
                    {
                        changed = value - coin;
                        totalCoinEarn += changed;
                    }

                    coin = value;
                    OnCoinChanged?.Invoke(changed, coin);
                }
            }
            else
            {
                UIToast.ShowError("Don't do that!");
                coin = 100;
                totalCoinEarn = 0;
                totalCoinSpend = 0;
            }
        }
    }
    public int totalCoinEarn = 0;
    public int totalCoinSpend = 0;

    [SerializeField]
    private int diamond;
    public int totalDiamond
    {
        get => diamond;
        set
        {
            if (diamond != value)
            {
                int changed = 0;
                if (diamond > value)
                {
                    changed = diamond - value;
                    totalDiamondSpend += changed;
                }
                else
                {
                    changed = value - diamond;
                    totalDiamondEarn += changed;
                }

                diamond = value;
                OnDiamondChanged?.Invoke(changed, diamond);
            }
        }
    }
    public int totalDiamondEarn = 0;
    public int totalDiamondSpend = 0;

    [SerializeField]
    private int star;
    public int totalStar
    {
        get
        {
            if (star == 0)
                star = DataManager.StagesAsset.list.Sum(x => x.star);
            return star;
        }
        set
        {
            if (star != value)
            {
                int changed = 0;
                if (star > value)
                {
                    changed = star - value;
                    totalStarSpend += changed;
                }
                else
                {
                    changed = value - star;
                    totalStarEarn += changed;
                }

                star = value;
                OnStarChanged?.Invoke(changed, star);
            }
        }
    }
    public int totalStarEarn = 0;
    public int totalStarSpend = 0;


    private int totalPurchased = 0;
    public int TotalPurchased
    {
        get => totalPurchased;
        set
        {
            if (totalPurchased != value && value > 0)
            {
                totalPurchased = value;
            }
        }
    }

    public delegate void MoneyChangedDelegate(int change, int current);
    public static event MoneyChangedDelegate OnCoinChanged;
    public static event MoneyChangedDelegate OnDiamondChanged;
    public static event MoneyChangedDelegate OnStarChanged;
}

[Serializable]
public class UserAnalysic : UserBase
{
    [Header("Analysic")]
    private int versionInstall;
    public int VersionInstall
    {
        get => PlayerPrefs.GetInt("VersionInstall",0);
        set
        {
            PlayerPrefs.SetInt("VersionInstall", value);
        }
    }

    private int versionCurrent;
    public int VersionCurrent
    {
        get => versionCurrent;
        set
        {
            if (versionCurrent != value)
            {
                versionCurrent = value;
            }
        }
    }

    private int session = 0;
    public int Session
    {
        get => session;
        set
        {
            if (session != value && value > 0)
            {
                session = value;
            }
        }
    }

    private long totalPlay = 0;
    public long TotalPlay
    {
        get => totalPlay;
        set
        {
            if (totalPlay != value && value > 0)
            {
                totalPlay = value;
            }
        }
    }

    private int totalWin = 0;
    public int TotalWin
    {
        get => totalWin;
        set
        {
            if (totalWin != value && value > 0)
            {
                totalWin = value;
            }
        }
    }
    private int winStreak = 0;
    public int WinStreak
    {
        get => winStreak;
        set
        {
            if (winStreak != value)
            {
                winStreak = value;
            }
        }
    }
    private int loseStreak = 0;
    public int LoseStreak
    {
        get => loseStreak;
        set
        {
            if (loseStreak != value)
            {
                loseStreak = value;
            }
        }
    }

    public long timeStart = 0;
    public long totalTimePlay = 0;
    public long TotalTimePlay
    {
        get => Util.timeNow - timeStart;
        set
        {
            if (totalTimePlay != value && value > 0)
            {
                totalTimePlay = value;
            }
        }
    }

    [Header("Ads")]
    private long totalAdInterstitial = 0;
    public long TotalAdInterstitial
    {
        get => totalAdInterstitial;
        set
        {
            if (totalAdInterstitial != value && value > 0)
            {
                totalAdInterstitial = value;
            }
        }
    }

    private long totalAdRewarded = 0;
    public long TotalAdRewarded
    {
        get => totalAdRewarded;
        set
        {
            if (totalAdRewarded != value && value > 0)
            {
                totalAdRewarded = value;
            }
        }
    }


    private string abTesting;
    public string ABTesting
    {
        get
        {
            if (string.IsNullOrEmpty(abTesting))
            {
                int randonAB = UnityEngine.Random.Range(0, 3);
                if (randonAB == 0)
                    abTesting = "A";
                else if (randonAB == 1)
                    abTesting = "B";
                else
                    abTesting = "C";
            }
            return abTesting;
        }
        set
        {
            if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(abTesting))
            {
                abTesting = value;
            }
        }
    }

    private string source;
    public string Source
    {
        get => source;
        set
        {
            if (!string.IsNullOrEmpty(value) && source != value)
            {
                source = value;
            }
        }
    }
}

[Serializable]
public class UserBase
{
    [Header("Base")]
    public string id;
    public string email;
    public string name;
}
