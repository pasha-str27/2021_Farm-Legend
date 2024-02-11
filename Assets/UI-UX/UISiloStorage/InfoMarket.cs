using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMarket : MonoBehaviour
{
    [SerializeField] Text txtName;
    [SerializeField] Text txtCount;
    [SerializeField] Text txtCoin;
    [SerializeField] Image icon;
    [SerializeField] GameObject objActive;
    [SerializeField] GameObject objEmpty;
    int count = 1;
    DataMarket dataMarket;
    ProductData data;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnItemStoreClick, OnItemStoreClickHanlde);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnItemStoreClick, OnItemStoreClickHanlde);
    }

    private void OnItemStoreClickHanlde(object obj)
    {
        var msg = (ProductData)obj;
        FillData(msg);
    }
    public void Show(DataMarket dataMarket)
    {
        this.dataMarket = dataMarket;
        txtName.text = DataManager.LanguegesAsset.GetName("Pick an item");
        objEmpty.SetActive(true);
        objActive.SetActive(false);
    }
    void FillData(ProductData data)
    {
        this.data = data;
        objActive.SetActive(true);
        objEmpty.SetActive(false);
        txtName.text = data.GetName;
        icon.sprite = data.icon;
        icon.SetNativeSize();
        count = 1;
        ReLoad();
    }
    void ReLoad()
    {
        txtCount.text = count + "/" + data.total;
        txtCoin.text = Util.Convert(count * data.cell);
    }
    public void SetDataMarketCurrent(DataMarket dataMarket)
    {
        this.dataMarket = dataMarket;
    }
    public void Btn_Add_Click()
    {
        if (count < data.total)
        {
            count++;
            ReLoad();
        }
    }
    public void Btn_Subtract_Click()
    {
        if (count > 1)
        {
            count--;
            ReLoad();
        }
    }
    public void Btn_Max_Click()
    {
        count = data.total;
        ReLoad();
    }
    public void Btn_Load_Click()
    {
        dataMarket.nameProduct = data.name;
        dataMarket.countProduct = count;
        data.total -= count;
        this.PostEvent((int)EventID.OnLoadProductSale, data);
    }
}
