using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShowRequirement : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text txtCount;
    [SerializeField] Color colorNot;
    [SerializeField] Color color;
    [SerializeField] Sprite spCoin;
    public void FillData(Requirement requirement)
    {
        ProductData data = DataManager.ProductAsset.GetProductByName(requirement.name);
        if(data != null) {
            txtCount.color = color;
            icon.sprite = data.icon;
            icon.SetNativeSize();
            txtCount.text = data.total + "/" + requirement.count;
            if (data.total < requirement.count)
            {
                txtCount.color = colorNot;
            }
        }
        else{
            gameObject.Recycle();
        }
    }
    public void FillData(int coin)
    {
        txtCount.color = color;
        icon.sprite = spCoin;
        txtCount.text = coin + "";
        icon.SetNativeSize();
    }
}
