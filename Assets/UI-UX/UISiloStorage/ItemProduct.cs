using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemProduct : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text txtCount;
    ProductData productData;
   public void FillData(ProductData productData)
    {
        this.productData = productData;
        icon.sprite = productData.icon;
        icon.SetNativeSize();
        txtCount.text = productData.total+"";
    }

    public void On_Item_Click()
    {
        this.PostEvent((int)EventID.OnItemStoreClick, productData);
    }
}
