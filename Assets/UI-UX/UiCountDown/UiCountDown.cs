using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiCountDown : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] Image icon;
    [SerializeField] Image fill;
    [SerializeField] Text txtName;
    [SerializeField] Text txtTime;
    [SerializeField] Text txtDiamond;
    [SerializeField] Sprite spGoldMine;
    [ReadOnly] [SerializeField] ProductData data;
    string key;
    int diamond = 0;
    int timeLife = 0;
    int time = 0;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnUpdateProgress, OnUpdateProgressHanlde);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUpdateProgress, OnUpdateProgressHanlde);
    }
    private void OnUpdateProgressHanlde(object obj)
    {
        var msg = (MessagerCountDown)obj;
        if (!msg.keyId.Equals(key))
            return;
        timeLife = msg.timeLife;
        fill.DOFillAmount((float)(time - msg.timeLife) / time, .1f);
        txtTime.text = Util.ConvertTime(msg.timeLife);
        diamond = DataManager.GameConfig.GetDiamondTime(msg.timeLife);
        txtDiamond.text = diamond + "";
        if (msg.timeLife <= 1)
            Hide();
    }

    public void Show(string key, ProductData data, int timecount, string nameCage = null)
    {
        this.key = key;
        this.data = data;
        uIAnimation.Show();
        time = data.time;
        txtTime.text = Util.ConvertTime(timecount);
        txtName.text = data.GetName;
        txtDiamond.text = DataManager.GameConfig.GetDiamondTime(timecount) + "";
        if (data.tabName == TabName.Pet)
        {
            ShopData shopData = DataManager.ShopAsset.GetAnimalsByProduct(nameCage);
            icon.sprite = shopData.spIcon;
            //txtName.text = shopData.name;
        }
        else
            icon.sprite = data.icon;

        icon.SetNativeSize();
        fill.DOFillAmount((float)(time - timecount) / time, .1f);
    }
    public void Show(string name,string key, int timecount)
    {
        this.key = key;
        time = DataManager.GameConfig.timeGoldMine;
        uIAnimation.Show();
        txtTime.text = Util.ConvertTime(timecount);
        txtName.text = name;
        txtDiamond.text = DataManager.GameConfig.GetDiamondTime(timecount) + "";
        icon.sprite = spGoldMine;
        icon.SetNativeSize();
        fill.DOFillAmount((float)(time - timecount) / time, .1f);
    }

    public void Hide()
    {
        this.PostEvent((int)EventID.OnZoomCamera, false);
        uIAnimation.Hide();
    }
    public void Btn_SpeedUp_Click()
    {
        if (PlayerPrefSave.Diamond > diamond)
        {
            CoinManager.AddDiamond(-diamond);
            this.PostEvent((int)EventID.OnSpeedUp, new MessagerCountDown { keyId = key });
            Hide();
            if (!PlayerPrefSave.IsTutorial)
                return;
            if (PlayerPrefSave.stepTutorial == 3)
            {
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 4:
                        PlayerPrefSave.stepTutorialCurrent = 5;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
            }
        }
        else
        {
            this.PostEvent((int)EventID.OnShowVideoReward);
            UIToast.Show("Not enought diamond!", null, ToastType.Notification, 1.5f);
        }
    }
    public void Btn_AdsSpeedUp_Click()
    {
        AdsManager.ShowVideoAds(() =>
        {
            this.PostEvent((int)EventID.OnSpeedUp, new MessagerCountDown { keyId = key });
            Hide();
        }, null,
        () =>
        {
            AnalyticsManager.LogEvent("ads_SpeedUp_crops_fail", new Dictionary<string, object> {
            { "name_product", data.name },
            { "timeLife", timeLife } });
        });
    }
}


