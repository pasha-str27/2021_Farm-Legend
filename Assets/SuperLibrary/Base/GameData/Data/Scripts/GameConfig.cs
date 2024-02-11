using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameConfig
{
    [Header("ADs")]
    public float timePlayToShowAds = 30;
    public float timePlayReduceToShowAds = 15;
    public RebornType rebornType = RebornType.Continue;
    public RebornBy rebornBy = RebornBy.Ads;
    public int suggestUpdateVersion = 0;
    public int adRewardNotToInter = 1;
    public int adInterNotToReward = 1;
    public int adInterViewToReward = 5;

    public float timeActiveMove = 1.5f;



    #region MONEY
    [Header("Money")]
    [SerializeField]
    public int _coinByAds = 100;
    public int coinByAds
    {
        get
        {
            return _coinByAds + (int)(_coinByAds * (PlayerPrefSave.Level - 1) * mutil);
        }
    }

    public int CoinBase = 100;
    public float mutil = .2f;
    public int DiamondBase = 50;
    public int coinUnlockMap = 100;
    public int countMaterialBase = 3;
    public int BaseDiamondTime = 120;

    public int coinLevelUp
    {
        get
        {
            return CoinBase + (int)(CoinBase * PlayerPrefSave.Level * mutil);
        }
    }
    public int diamondLevelUp
    {
        get
        {
            return 1 + (int)(PlayerPrefSave.Level * mutil);
        }
    }
    public int expLevel
    {
        get
        {
            return baseExp + (int)(baseExp * PlayerPrefSave.Level * mutil);
        }
    }
    public int coinMap
    {
        get
        {
            return coinUnlockMap + (int)(coinUnlockMap * PlayerPrefSave.LevelMap * mutil);
        }
    }
    public int countMaterialMap
    {
        get
        {
            return countMaterialBase + (int)(PlayerPrefSave.LevelMap * mutil);
        }
    }

    public int GetDiamondTime(int time)
    {
        return time / BaseDiamondTime <= 0 ? 1 : (time / BaseDiamondTime>=100?100: time / BaseDiamondTime);
    }
    public int GetExpByTime(int time)
    {

        return time / numTimePersent <= 0 ? 1 : time / numTimePersent;
    }
    #endregion

    #region LEVEL DESIGN
    [Header("Base")]
    public int BaseStoreUpgrade = 10;
    public int BaseStoreSlilo = 30;
    public int BaseStoreStorage = 30;
    public int maxOrder = 9;
    public float mutilUpgradeStore = .5f;
    public int MaxLand = 100;
    public int NumHarvestOldTree = 3;
    public int NumUnlockLand = 3;
    public int percentRandomMaterial = 10;
    public int timeSaleMarket = 60;
    public int numTimePersent = 60;

    [Header("Level unlock")]
    public int LevelUnlockAchie = 3;
    public int LevelUnlockMarket = 3;
    public int LevelUnlockOrder = 3;
    [Header("minigame")]
    public float numPercentMinigame = .2f;
    public int LevelUnlockNextMinigame = 6;
    public int coinMiniGameBase = 200;
    public int diamondMiniGameBase = 5;
    public int coinMiniGame {
        get { return (int)(coinMiniGameBase /*+ coinMiniGameBase * PlayerPrefSave.Level * numPercentMinigame*/); }
    }
    public int diamondMiniGame
    {
        get { return (int)(diamondMiniGameBase /*+ diamondMiniGameBase * PlayerPrefSave.Level * numPercentMinigame*/); }
    }

    [Header("Harbor")]
    public int LevelUnlockOrderHarbor = 10;
    public int timeDeleteOrderHarbor = 300;

    [Header("Gold mine")]
    public int LevelUnlockGoldMine = 12;
    public int numMaterialGoldmine = 2;
    public int goldMine = 100;
    public int timeGoldMine = 600;

    [Header("Exp")]
    public int baseExp = 10;
    public float rateExp = 1.2f;
    public float mutilOrder = 0.2f;
    public int BaseCountBonusOder = 2;
    public int expGarbage = 1;
    public int GetCountBonusOder
    {
        get { return BaseCountBonusOder + (int)(PlayerPrefSave.Level * mutil); }
    }

    public int GetStoreUpgrade(ObjectMouseDown mouseDown)
    {
        return BaseStoreUpgrade + (int)(PlayerPrefSave.GetLevelStore(mouseDown) * BaseStoreUpgrade * mutilUpgradeStore);
    }
    #endregion
}

[Serializable]
public enum RebornType
{
    Continue,
    Checkpoint
}

[Serializable]
public enum RebornBy
{
    Free,
    Gold,
    Gem,
    Ads
}

[SerializeField]
public class BossData
{
    public int level;
    public int health;
    public int damage;
}