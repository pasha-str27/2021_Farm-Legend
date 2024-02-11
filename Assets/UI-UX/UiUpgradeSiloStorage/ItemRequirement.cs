using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemRequirement : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text txtName;
    [SerializeField] Text txtDiamond;
    [SerializeField] Text txtCount;
    [SerializeField] GameObject objTick;

    public void Init(ProductData material)
    {
        icon.sprite = material.icon;
        txtName.text = material.GetName;
        //txtCount.text = PlayerPrefSave.GetCountProduct(material.tabName, material.index) + "/" + material.need;
        txtDiamond.text = material.price+"";
    }
}
