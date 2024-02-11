using DG.Tweening;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLevelUp : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] Text txtLevel;
    [SerializeField] Text txtCoin;
    [SerializeField] Text txtDiamond;
    [SerializeField] ItemLevelUp itemLevel;
    [SerializeField] Text txtDes3;
    [SerializeField] GameObject scrollView;
    [SerializeField] Transform content;
    [ReadOnly] DataLevel dataLevel;
    int tempCount = 0;
    int tempCountShop = 0;
    public void Show()
    {
        uIAnimation.Show();
        this.PostEvent((int)EventID.OnHideToggleSuggestion);
        dataLevel = DataManager.LevelAsset.CurrentDataLevel;
        SoundManager.Play("sfxLevelUp");
        content.RecycleChild();
        txtLevel.text = DataManager.LanguegesAsset.GetName("You'ra reached level") + " " + PlayerPrefSave.Level;
        txtCoin.text = dataLevel.coin + "";
        txtDiamond.text = dataLevel.gem + "";
        tempCount = 0;
        tempCountShop = 0;
        //check unlock harbor
        if (PlayerPrefSave.Level == DataManager.GameConfig.LevelUnlockOrderHarbor)
        {
            DataManager.OrderHarborAsset.CreatOrder();
        }

        ProductData data = null;
        for (int i = 0; i < DataManager.ProductAsset.list.Count; i++)
        {
            data = DataManager.ProductAsset.list[i];
            if (data.levelUnlock == PlayerPrefSave.Level && data.icon != null)
            {
                SpawItem(data.icon, .5f);
                tempCount++;
            }
        }

        ShopData shopData = null;
        for (int i = 0; i < DataManager.ShopAsset.list.Count; i++)
        {
            shopData = DataManager.ShopAsset.list[i];
            if (shopData.name.Equals("Land"))
            {
                if (PlayerPrefSave.Level > 2)
                {
                    SpawItem(shopData.spIcon, .3f);
                    tempCount++;
                    tempCountShop++;
                    continue;
                }

            }
            if (shopData.levelUnlock == PlayerPrefSave.Level)
            {
                SpawItem(shopData.spIcon, .3f);
                tempCount++;
                tempCountShop++;
            }
        }

        if (tempCount == 0)
        {
            txtDes3.text = DataManager.LanguegesAsset.GetName("Congratulations!");
            scrollView.SetActive(false);
        }
        else
        {
            txtDes3.text = DataManager.LanguegesAsset.GetName("Now available:");
            scrollView.SetActive(true);
        }
        SoundManager.Play("firework");

        AnalyticsManager.LogEvent("Levelup", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay },
         { "coin",CoinManager.totalCoin },
            { "diamond", CoinManager.totalDiamond },
        });

    }
    void SpawItem(Sprite icon, float scale)
    {
        var item = itemLevel.Spawn(content);
        item.FillData(icon, scale);
    }
    public void Hide(TweenCallback callBack)
    {
        if(callBack != null)
        {
            uIAnimation.Hide(callBack);
        }
        else
        {
            uIAnimation.Hide();
            this.PostEvent((int)EventID.OnHidePopupLevelUp, tempCountShop);
        }

        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 0)
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 6:
                    PlayerPrefSave.stepTutorial = 1;
                    PlayerPrefSave.stepTutorialCurrent = 0;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
    }


    public void Btn_Take_Click()
    {
        CoinManager.AddCoin(dataLevel.coin, transform);
        CoinManager.AddDiamond(dataLevel.gem, transform);
        if (GameUIManager.BuildMarketing == BuildMarketing.Farm)
        {
            Hide(null);
            return;
        }
        if (!PlayerPrefSave.IsTutorial)
            LevelUpLoadMiniGame();
        else Hide(null);
    }
    public void Btn_TakeX2_Click()
    {
        AdsManager.ShowVideoAds(() =>
        {
            CoinManager.AddCoin(dataLevel.coin * 2, transform);
            CoinManager.AddDiamond(dataLevel.gem * 2, transform);
            AnalyticsManager.LogEvent("reward_Levelup", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay },
            { "coin",CoinManager.totalCoin },
            { "diamond", CoinManager.totalDiamond },
            });
            //Hide();
            
            if(GameUIManager.BuildMarketing == BuildMarketing.Farm)
            {
                Hide(null);
                return;
            }
            if (!PlayerPrefSave.IsTutorial)
                LevelUpLoadMiniGame();
            else
                Hide(null);
        }, null,
        () =>
        {
            AnalyticsManager.LogEvent("ads_reward_Levelup_false", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay },
            { "coin",CoinManager.totalCoin },
            { "diamond", CoinManager.totalDiamond }, });

        });
    }

    void LevelUpLoadMiniGame()
    {
        Hide(()=> {
            GameStateManager.Idle(null);
        });
    }
}
