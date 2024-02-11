using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHarbor : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] Transform contentOrder;
    [SerializeField] ItemOrderHarbor orderHarbor;


    [SerializeField] GameObject objActive;
    [SerializeField] ItemNeedOrder itemNeed;
    [SerializeField] Transform contentItemNeed;
    [SerializeField] Text txtCoin, txtExp;
    [SerializeField] GameObject objTimeSend;
    [SerializeField] Text txtTimeSend;
    [SerializeField] Text txtTime;
    [SerializeField] Text txtDiamond;

    [SerializeField] GameObject objDelete;
    [SerializeField] Text txtTimeDelete;
    [SerializeField] Text txtDiamondDelete;

    [ReadOnly] [SerializeField] List<OrderHarborData> orderDatas;
    ItemOrderHarbor itemOrderHarbor;
    int diamondSend = 0;
    int diamondDelete = 0;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnItemOrderHarborClick, OnItemOrderHarborClickHandle);
        this.RegisterListener((int)EventID.OnUpdateTimeHarbor, OnUpdateTimeHarborHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnItemOrderHarborClick, OnItemOrderHarborClickHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUpdateTimeHarbor, OnUpdateTimeHarborHandle);
    }

    private void OnUpdateTimeHarborHandle(object obj)
    {
        var time = (int)obj;
        if (time > 0)
        {
            objTimeSend.SetActive(true);
            txtTime.text = Util.ConvertTime(time);
            diamondSend = DataManager.GameConfig.GetDiamondTime(time);
            txtDiamond.text = diamondSend + "";
        }
        else
        {
            objTimeSend.SetActive(false);
        }
    }

    private void OnItemOrderHarborClickHandle(object obj)
    {
        var msg = (ItemOrderHarbor)obj;
        itemOrderHarbor = msg;
        //show info
        contentItemNeed.RecycleChild();
        objDelete.SetActive(msg.timeCount > 0);
        objActive.SetActive(msg.timeCount <= 0);

        if (msg.timeCount > 0)
            return;

        txtCoin.text = Util.Convert(msg.order.coin);
        txtExp.text = Util.Convert(msg.order.exp);
        txtTimeSend.text = Util.ConvertTime(msg.order.timeSend);
        for (int i = 0; i < msg.order.productNeeds.Count; i++)
        {
            var item = itemNeed.Spawn(contentItemNeed);
            item.FillData(msg.order.productNeeds[i]);
        }
    }

    public void Show()
    {
        uIAnimation.Show();
        objTimeSend.SetActive(false);
        objActive.SetActive(true);
        objDelete.SetActive(false);
        if (orderDatas.Count == 0)
        {
            contentOrder.RecycleChild();
            orderDatas = DataManager.OrderHarborAsset.list;
            for (int i = 0; i < orderDatas.Count; i++)
            {
                var item = orderHarbor.Spawn(contentOrder);
                item.FillData(orderDatas[i]);
            }
        }
        else
        {
            for (int i = 0; i < orderDatas.Count; i++)
            {
                contentOrder.GetChild(i).GetComponent<ItemOrderHarbor>().FillData(orderDatas[i]);
            }
        }
        

        int tempTime = HarborManager.Instance.timeLife;
        if (tempTime > 0)
        {
            objTimeSend.SetActive(true);
            txtTime.text = Util.ConvertTime(tempTime);
            diamondSend = DataManager.GameConfig.GetDiamondTime(tempTime);
            txtDiamond.text = diamondSend + "";
        }
    }

    public void Hide()
    {
        uIAnimation.Hide();
    }
    private void Update()
    {
        if (objDelete.activeInHierarchy && itemOrderHarbor!= null)
        {
            diamondDelete = DataManager.GameConfig.GetDiamondTime(itemOrderHarbor.timeCount);
            txtDiamondDelete.text = diamondDelete + "";
            txtTimeDelete.text = Util.ConvertTime(itemOrderHarbor.timeCount);
        }
    }
    public void Btn_Send_Click()
    {
        if (GameUIManager.IsTest)
        {
            this.PostEvent((int)EventID.OnShipStar, itemOrderHarbor.order);
            Hide();
            return;
        }
        if (itemOrderHarbor.order.checkCompleteOrder)
        {
            this.PostEvent((int)EventID.OnShipStar, itemOrderHarbor.order);
            //AnalyticsManager.LogEvent("Send_order_Click", new Dictionary<string, object> {
            //{ "level", PlayerPrefSave.Level },
            //{ "time", DataManager.UserData.TotalTimePlay } });
            Hide();
        }
        else
        {
            UIToast.Show("Please complete the order!", null, ToastType.Notification, 1.5f);
        }
    }

    public void Btn_DeleteOrder_Click()
    {
        DataManager.OrderHarborAsset.ResetOrder(itemOrderHarbor.order.index);
        this.PostEvent((int)EventID.OnResetOrderHarbor, itemOrderHarbor.order);

        //AnalyticsManager.LogEvent("DeleteOrder_Click", new Dictionary<string, object> {
        //    { "level", PlayerPrefSave.Level },
        //    { "time", DataManager.UserData.TotalTimePlay } });
    }

    public void Btn_SpeedUp_Click()
    {
        if (CoinManager.totalDiamond >= diamondSend)
        {
            CoinManager.AddDiamond(-diamondSend, null, null, "nosound");
        }
        else
        {
            UIToast.Show("Not enough diamond!", null, ToastType.Notification, 1.5f);
            this.PostEvent((int)EventID.OnShowVideoReward);
            return;
        }
        this.PostEvent((int)EventID.OnSpeedUpHarbor);
        Hide();
    }
    public void Btn_SpeedUpDelete_Click()
    {
        if (CoinManager.totalDiamond >= diamondDelete)
        {
            CoinManager.AddDiamond(-diamondDelete, null, null, "nosound");
        }
        else
        {
            UIToast.Show("Not enough diamond!", null, ToastType.Notification, 1.5f);
            this.PostEvent((int)EventID.OnShowVideoReward);
            return;
        }
        this.PostEvent((int)EventID.OnSpeedUpDeledeHarbor, itemOrderHarbor);
    }
}
