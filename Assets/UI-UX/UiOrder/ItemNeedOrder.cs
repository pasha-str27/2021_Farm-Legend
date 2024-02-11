using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNeedOrder : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text txtCount;
    [SerializeField] GameObject tick;
    [SerializeField] GameObject btnSug;
    public bool isNough = false;
    [SerializeField] Color colorNot;
    [SerializeField] Color color;
    [ReadOnly] [SerializeField] ProductData data;
    DataItemOrder dataItemOrder;
    DataItemOrderHarbor dataItemOrderHarbor;

    public void FillData(DataItemOrder dataItemOrder)
    {
        this.dataItemOrder = dataItemOrder;
        data = DataManager.ProductAsset.GetProductByName(dataItemOrder.name);
        tick.SetActive(false);
        if (btnSug != null)
            btnSug.SetActive(false);
        icon.sprite = data.icon;
        icon.SetNativeSize();
        txtCount.text = data.total + "/" + dataItemOrder.need;
        isNough = data.total >= dataItemOrder.need;
        txtCount.color = data.total >= dataItemOrder.need? color: colorNot;
        tick.SetActive(isNough);

    }
    public void FillData(DataItemOrderHarbor dataItemOrderHarbor)
    {
        this.dataItemOrderHarbor = dataItemOrderHarbor;
        data = DataManager.ProductAsset.GetProductByName(dataItemOrderHarbor.name);
        tick.SetActive(false);
        if (btnSug != null)
            btnSug.SetActive(false);
        icon.sprite = data.icon;
        icon.SetNativeSize();
        txtCount.text = data.total + "/" + dataItemOrderHarbor.need;
        isNough = data.total >= dataItemOrderHarbor.need;
        txtCount.color = data.total >= dataItemOrderHarbor.need ? color : colorNot;
        tick.SetActive(isNough);

    }
    public void MinusPoints()
    {
        if (data != null)
            data.total -= dataItemOrder.need;
    }
}
