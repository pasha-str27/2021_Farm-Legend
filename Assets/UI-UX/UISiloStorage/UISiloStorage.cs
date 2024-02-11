using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UISiloStorage : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] ItemProduct prItemProduct;
    [SerializeField] Transform content;
    [SerializeField] GameObject buttonSilo, buttonStorage;
    [SerializeField] GameObject buttUpgrade;
    [SerializeField] InfoMarket infoMarket;
    [SerializeField] Text txtTitle;
    [SerializeField] FillProgressBar fillProgress;
    ObjectMouseDown objectMouseDownCurrent;

    DataMarket dataMarket = null;
    List<ProductData> productDatas;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnClickButtonTab, OnClickButtonTabHanlde);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickButtonTab, OnClickButtonTabHanlde);
    }
    private void OnClickButtonTabHanlde(object obj)
    {
        var msg = (MessagerTab)obj;
        if (msg.typeTab != TypeTab.TabSilo && msg.typeTab != TypeTab.TabStorage)
            return;
        FillData(msg.tabName);
    }

    public void Show(ObjectMouseDown objectMouseDown)
    {
        uIAnimation.Show();
        dataMarket = null;
        buttUpgrade.SetActive(true);
        infoMarket.gameObject.SetActive(false);
        txtTitle.text = DataManager.LanguegesAsset.GetName(objectMouseDown.ToString());
        fillProgress.UpdateFillBar(objectMouseDown);
        objectMouseDownCurrent = objectMouseDown;
        switch (objectMouseDown)
        {
            case ObjectMouseDown.Silo:
                buttonSilo.SetActive(true);
                buttonStorage.SetActive(false);
                this.PostEvent((int)EventID.OnClickButtonTab, new MessagerTab { typeTab = TypeTab.TabSilo, tabName = TabName.Crops });
                break;
            case ObjectMouseDown.Storage:
                buttonSilo.SetActive(false);
                buttonStorage.SetActive(true);
                this.PostEvent((int)EventID.OnClickButtonTab, new MessagerTab { typeTab = TypeTab.TabStorage, tabName = TabName.Food });
                break;
        }
    }
    public void Show(ObjectMouseDown objectMouseDown, DataMarket dataMarket)
    {
        Show(objectMouseDown);
        this.dataMarket = dataMarket;
        buttUpgrade.SetActive(false);
        infoMarket.gameObject.SetActive(true);
        infoMarket.Show(dataMarket);
    }
    void FillData(TabName tabName)
    {
        content.RecycleChild();
        productDatas = DataManager.ProductAsset.list.Where(x => x.tabName == tabName).ToList();
        for (int i = 0; i < productDatas.Count; i++)
        {
            if (productDatas[i].total > 0)
            {
                var item = prItemProduct.Spawn(content);
                item.FillData(productDatas[i]);
            }
        }
        //Debug.Log("=> FillData " + tabName+":"+ productDatas.Count);
    }
    
    public void Hide()
    {
        uIAnimation.Hide();
    }

    public void Btn_NextStore_Click()
    {
        if(objectMouseDownCurrent == ObjectMouseDown.Storage)
        {
            objectMouseDownCurrent = ObjectMouseDown.Silo;
        }
        else
        {
            objectMouseDownCurrent = ObjectMouseDown.Storage;
        }
        if(dataMarket != null)
        {
            Show(objectMouseDownCurrent, dataMarket);
        }
        else
            Show(objectMouseDownCurrent);
    }

    public void Btn_Upgrade_Click()
    {
        this.PostEvent((int)EventID.OnShowPopupUpgrade, objectMouseDownCurrent);
        Hide();
    }
}
