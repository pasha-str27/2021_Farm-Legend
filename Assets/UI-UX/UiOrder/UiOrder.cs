using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using MyBox;
using DG.Tweening;
public class UiOrder : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] ItemOrder itemOrder;
    [SerializeField] ItemNeedOrder itemNeed;
    [SerializeField] Transform contentOrder;
    [SerializeField] Transform contentItemNeed;
    [SerializeField] Text txtNameOrder;
    [SerializeField] Text txtCoin, txtExp;
    [SerializeField] Image fillBonus;
    [SerializeField] Text txtBonus;
    [SerializeField] GameObject btnFaild;

    [ReadOnly] [SerializeField] List<OrderData> orderDatas;
    OrderData orderView;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnItemOrderClick, OnItemOrderClickHandle);
        
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnItemOrderClick, OnItemOrderClickHandle);
    }
    private void OnItemOrderClickHandle(object obj)
    {
        var msg = (OrderData)obj;
        contentItemNeed.RecycleChild();
        orderView = msg;
        //txtNameOrder.text = DataManager.LanguegesAsset.GetName(msg.name);
        txtNameOrder.text = DataManager.LanguegesAsset.GetName(msg.name);
        txtCoin.text = Util.Convert(msg.coin);
        txtExp.text = Util.Convert(msg.exp);
        for (int i = 0; i < msg.productNeeds.Count; i++)
        {
            var item = itemNeed.Spawn(contentItemNeed);
            item.FillData(msg.productNeeds[i]);
        }
        if (!orderView.checkCompleteOrder)
        {
            btnFaild.SetActive(true);
        }
        else
        {
            btnFaild.SetActive(false);
        }

        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 9 && msg.index == 0)
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 1:
                    PlayerPrefSave.stepTutorialCurrent = 2;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
    }

    public void Show()
    {
        uIAnimation.Show();
        contentOrder.RecycleChild();
        orderDatas = DataManager.OrderAsset.list;
        ItemOrder tempItemTutorial = null;
        for (int i = 0; i < orderDatas.Count; i++)
        {
            var item = itemOrder.Spawn(contentOrder);
            item.FillData(orderDatas[i]);
            if(i==0)
                tempItemTutorial= item;
        }
        LoadFillBonus();

        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 9)
        {
            for (int i = 0; i < orderDatas[0].productNeeds.Count; i++)
            {
                DataManager.ProductAsset.GetProductByName(orderDatas[0].productNeeds[i].name).total += orderDatas[0].productNeeds[i].need;
            }
            tempItemTutorial.FillData(orderDatas[0]);

            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 0:
                    PlayerPrefSave.stepTutorialCurrent = 1;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }

        AnalyticsManager.LogEvent("show_order", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay } });
    }
    void LoadFillBonus()
    {
        txtBonus.text = DataManager.LanguegesAsset.GetName("Bonus")+" " + PlayerPrefSave.bonusOrder + "/" + DataManager.GameConfig.GetCountBonusOder;
        fillBonus.DOFillAmount((float)PlayerPrefSave.bonusOrder / DataManager.GameConfig.GetCountBonusOder, .2f);
    }
    public void Hide()
    {
        uIAnimation.Hide();
    }
   
    public void Btn_Close_Click()
    {
        if (PlayerPrefSave.IsTutorial)
            return;
        Hide();
    }
    public void Btn_Send_Click()
    {
        if (PlayerPrefSave.stepTutorial == 9 && PlayerPrefSave.stepTutorialCurrent == 1)
        {
            return;
        }
        if (orderView.checkCompleteOrder)
        {
            this.PostEvent((int)EventID.CarStart, orderView);
            LoadFillBonus();
            AnalyticsManager.LogEvent("Send_order_Click", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay } });
            Hide();
        }
        else
        {
            UIToast.Show("Please complete the order!", null, ToastType.Notification, 1.5f);
        }
    }

    public void Btn_DeleteOrder_Click()
    {
        if (PlayerPrefSave.IsTutorial)
            return;
        DataManager.OrderAsset.ResetOrder(orderView.name);
        this.PostEvent((int)EventID.OnResetOrder, orderView);
        this.PostEvent((int)EventID.OnLoadUiOrderMap);

        AnalyticsManager.LogEvent("DeleteOrder_Click", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay } });
    }
}

