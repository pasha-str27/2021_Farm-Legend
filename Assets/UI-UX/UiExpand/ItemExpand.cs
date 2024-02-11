using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemExpand : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text txtCount;
    [SerializeField] Text txtDiamond;
    [SerializeField] GameObject tick;
    [SerializeField] GameObject btnDiamod;
    [SerializeField] Sprite spCoin;
    [SerializeField] Color colorNot;
    [SerializeField] Color color;
    public bool isNough = false;
    [ReadOnly] [SerializeField] ProductData data;
    int need = 0;
    int coin = 0;
    int diamond = 0;
    public void FillData(ProductData productData, int need)
    {
        this.data = productData;
        this.need = need;
        diamond = need - data.total;
        txtCount.color = color;
        tick.SetActive(false);
        icon.sprite = productData.icon;
        icon.SetNativeSize();
        txtCount.text = productData.total + "/" + need;
        txtDiamond.text = (diamond > 0 ? diamond : 0) + "";
        isNough = productData.total >= need;
        btnDiamod.SetActive(productData.total < need);
        if (productData.total < need)
        {
            txtCount.color = colorNot;
        }
        tick.SetActive(isNough);
        coin = 0;
    }
    public void FillDataCoin(int coin)
    {
        tick?.SetActive(false);
        icon.sprite = spCoin;
        icon.SetNativeSize();
        
        txtCount.text = PlayerPrefSave.Coin + "/" + coin;
        isNough = PlayerPrefSave.Coin >= coin;
        btnDiamod.SetActive(false);
        tick.SetActive(isNough);
        txtCount.color = isNough? color : colorNot;
        this.coin = coin;
    }

    public void MinusPoints()
    {
        if (data != null)
            data.total -= need;

        if (coin > 0)
            CoinManager.AddCoin(-coin);
    }

    public void Btn_Diamond_Click()
    {
        if (PlayerPrefSave.Diamond >= diamond)
        {
            CoinManager.AddDiamond(-diamond);
            //this.PostEvent((int)EventID.OnFxPutDiamond, txtDiamond.transform.position);
            data.total += diamond;
            FillData(data, need);
        }
        else
        {
            this.PostEvent((int)EventID.OnShowVideoReward);
            UIToast.Show("Not enought diamond!", null, ToastType.Notification, 1.5f);
        }
    }
}
