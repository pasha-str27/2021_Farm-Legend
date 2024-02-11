using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UiGiftMain : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] Text txtCoin;
    [SerializeField] Image icon;
    [SerializeField] Sprite spCoinSmall;
    [SerializeField] Sprite spCoinBig;
    
    int coinReward = 0;
    public void Show()
    {
        uIAnimation.Show();
        coinReward = DataManager.GameConfig.coinByAds;
        if (coinReward <= 500)
            icon.sprite = spCoinSmall;
        else icon.sprite = spCoinBig;
        icon.SetNativeSize();

        txtCoin.text = "+"+coinReward;
    }
    public void Hide()
    {
        uIAnimation.Hide();
    }
    public void Btn_AdsCoin_Click()
    {
        AdsManager.ShowVideoAds(() =>
        {
            CoinManager.AddCoin(coinReward, transform);
            uIAnimation.Hide();
            AnalyticsManager.LogEvent("reward_ads", new Dictionary<string, object> {
            { "name_reward", "gift_main" },
            { "reward", coinReward },
            { "level", PlayerPrefSave.Level },
            { "total_coin", DataManager.UserData.totalCoin },
            { "total_diamond", DataManager.UserData.totalDiamond } });
        }, null,
        () => {
            AnalyticsManager.LogEvent("ads_coin_main_reward_fail");
        });
    }
}
