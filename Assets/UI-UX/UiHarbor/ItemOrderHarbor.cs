using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemOrderHarbor : MonoBehaviour
{
    [SerializeField] Image bgItem;
    [SerializeField] Image icon;
    [SerializeField] Text txtCoin;
    [SerializeField] Text txtExp;
    [SerializeField] GameObject objTick;
    [SerializeField] GameObject objActive;
    [SerializeField] GameObject objTime;
    [SerializeField] Text txtTime;
    [SerializeField] Sprite spActive, spSelect;
    [ReadOnly] public OrderHarborData order;
    Coroutine coroutine;
    bool isComplete;

    void StartCountdown()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(CountdownDelete());
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnItemOrderHarborClick, OnItemOrderHarborClickHandle);
        this.RegisterListener((int)EventID.OnResetOrderHarbor, OnResetOrderHarborHandle, DispatcherType.Late);
        this.RegisterListener((int)EventID.OnSpeedUpDeledeHarbor, OnSpeedUpDeledeHarborHandle, DispatcherType.Late);

        
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnItemOrderHarborClick, OnItemOrderHarborClickHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnResetOrderHarbor, OnResetOrderHarborHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnSpeedUpDeledeHarbor, OnSpeedUpDeledeHarborHandle);

        if (timeCount > 0)
        {
            timeOut = Util.timeNow;
            //Debug.Log("=> timeOut OnDisable:" + timeOut);
        }
    }

    private void OnSpeedUpDeledeHarborHandle(object obj)
    {
        var msg = (ItemOrderHarbor)obj;
        if (msg != this)
            return;
        timeCount = 0;
        StartCountdown();
        Item_Click();
    }

    private void OnResetOrderHarborHandle(object obj)
    {
        var msg = (OrderHarborData)obj;
        if (msg.name != order.name)
            return;

        FillData(DataManager.OrderHarborAsset.GetOrder(order.index));
        timeCount = DataManager.GameConfig.timeDeleteOrderHarbor;
        StartCountdown();
        Item_Click();
    }

    private void OnItemOrderHarborClickHandle(object obj)
    {
        var msg = (ItemOrderHarbor)obj;
        if (msg.order.index == order.index)
            SelectOrder(true);
        else
            SelectOrder(false);
    }

    public void FillData(OrderHarborData order)
    {
        this.order = order;
        icon.sprite = order.spIcon;
        icon.SetNativeSize();
        txtCoin.text = Util.Convert(order.coin);
        txtExp.text = Util.Convert(order.exp);
        objTick.SetActive(false);
        objTime.SetActive(false);

        isComplete = true;
        if (PlayerPrefSave.IsNewHarbor && order.index == 0)
        {
            PlayerPrefSave.IsNewHarbor = false;
            for (int i = 0; i < order.productNeeds.Count; i++)
            {
                ProductData data = DataManager.ProductAsset.GetProductByName(order.productNeeds[i].name);
                if (data.total < order.productNeeds[i].need)
                    data.total += order.productNeeds[i].need - data.total;
            }
        }
        else
        {
            for (int i = 0; i < order.productNeeds.Count; i++)
            {
                if (DataManager.ProductAsset.GetProductByName(order.productNeeds[i].name).total < order.productNeeds[i].need)
                    isComplete = false;
            }
        }
        
        objTick.SetActive(isComplete);
        if (order.index == Util.IndexOrderHarbor)
            Item_Click();

        if (timeCount > 0)
        {
            timeCount -= (Util.timeNow - timeOut);
            //Debug.Log("=> timeOut load: " + (Util.timeNow - timeOut));
            StartCountdown();
        }
    }
    public void Item_Click()
    {
        this.PostEvent((int)EventID.OnItemOrderHarborClick, this);
        Util.IndexOrderHarbor = order.index;
    }
    void SelectOrder(bool isSelect)
    {
        bgItem.sprite = isSelect ? spSelect : spActive;
    }

    #region count delete
    IEnumerator CountdownDelete()
    {
        objTime.SetActive(timeCount > 0);
        objActive.SetActive(timeCount <= 0);
        txtTime.text = Util.ConvertTime(timeCount);
        if(timeCount <= 0)
        {
            Item_Click();
        }
        yield return new WaitForSeconds(1);
        if (timeCount > 0)
        {
            timeCount--;
            coroutine = StartCoroutine(CountdownDelete());
        }

    }
    public int timeCount
    {
        get { return PlayerPrefs.GetInt("timeCount_harbor" + order.id, 0); }
        set { PlayerPrefs.SetInt("timeCount_harbor" + order.id, value); }
    }
    int timeOut
    {
        get { return PlayerPrefs.GetInt("timeOut_harbor" + order.id, 0); }
        set { PlayerPrefs.SetInt("timeOut_harbor" + order.id, value); }
    }
    #endregion
}
