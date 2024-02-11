using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemOrder : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Text txtCoin;
    [SerializeField] Text txtExp;
    [SerializeField] GameObject tick;
    [ReadOnly][SerializeField] OrderData orderNeed;
    bool isComplete = true;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnItemOrderClick, OnItemOrderClickHandle, DispatcherType.Late);
        this.RegisterListener((int)EventID.OnResetOrder, OnResetOrderHandle, DispatcherType.Late);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnItemOrderClick, OnItemOrderClickHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnResetOrder, OnResetOrderHandle);
    }

    private void OnResetOrderHandle(object obj)
    {
        var msg = (OrderData)obj;
        if (msg.name.Equals(orderNeed.name))
        {
            FillData(DataManager.OrderAsset.list.FirstOrDefault(x=>x.name.Equals(msg.name)));
            On_Item_Click();
        }
    }

    private void OnItemOrderClickHandle(object obj)
    {
        var msg = (OrderData)obj;
        if (!msg.name.Equals(orderNeed.name))
        {
            anim.SetBool("click", false);
        }
        else
        {
            anim.SetBool("click", true);
        }
    }

    public void FillData(OrderData orderNeed)
    {
        if (orderNeed.coin < 1)
            orderNeed.coin = 1;
        this.orderNeed = orderNeed;
        txtCoin.text = Util.Convert(orderNeed.coin);
        txtExp.text = Util.Convert(orderNeed.exp);
        isComplete = true;
        for (int i = 0; i < orderNeed.productNeeds.Count; i++)
        {
            if(DataManager.ProductAsset.GetProductByName(orderNeed.productNeeds[i].name).total < orderNeed.productNeeds[i].need)
                isComplete = false;
        }
        tick.SetActive(isComplete);

        if (orderNeed.index == 0)
        {
            On_Item_Click();
        }
    }

    public void On_Item_Click()
    {
        this.PostEvent((int)EventID.OnItemOrderClick, orderNeed);
    }
}
