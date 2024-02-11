using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoreManager : MonoBehaviour
{
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnAddProduct, OnAddProductHandle);
    }
    private void OnDisable() 
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnAddProduct, OnAddProductHandle);
    }

    private void OnAddProductHandle(object obj)
    {
        var msg = (MessagerAddProduct)obj;
        bool limit = DataManager.ProductAsset.GetTotal(GetTypeStore(msg.data)) + msg.cropYields <= PlayerPrefSave.GetMaxStore(GetTypeStore(msg.data));
        if (limit)
        {
            msg.onDone?.Invoke();
        }
        else
        {
            msg.onFail?.Invoke();
            this.PostEvent((int)EventID.OnShowPopupUpgrade, GetTypeStore(msg.data));
        }
    }

    ObjectMouseDown GetTypeStore(ProductData data)
    {
        if (data.tabName == TabName.Crops
        || data.tabName == TabName.Flower || data.tabName == TabName.OldTree || data.tabName == TabName.Pet)
        {
            return ObjectMouseDown.Silo;
        }
        return ObjectMouseDown.Storage;
    }
}

public class MessagerAddProduct
{
    public ProductData data;
    public int cropYields=1;
    public Action onDone;
    public Action onFail;
}
