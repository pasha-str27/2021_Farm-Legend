using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiUpgradeSiloStorage : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] Text txtTitle;
    [SerializeField] Text txtCountUpgrade;
    [SerializeField] Image fillProgress;
    [SerializeField] ItemExpand itemNeed;
    [SerializeField] Transform tfConten;
    [SerializeField] List<Requirement> listMaterialSilo;
    [SerializeField] List<Requirement> listMaterialStorage;
    List<Requirement> listMaterial;
    ObjectMouseDown objectMouseDown;
    List<ItemExpand> tempList = new List<ItemExpand>();
    public void Show(ObjectMouseDown objectMouseDown)
    {
        uIAnimation.Show();
        this.objectMouseDown = objectMouseDown;
        txtTitle.text = DataManager.LanguegesAsset.GetName("Upgrade")+" "+ DataManager.LanguegesAsset.GetName(objectMouseDown.ToString());
        txtCountUpgrade.text = PlayerPrefSave.GetMaxStore(objectMouseDown) + "+" + DataManager.GameConfig.GetStoreUpgrade(objectMouseDown);
        fillProgress.fillAmount = (float)PlayerPrefSave.GetMaxStore(objectMouseDown) / (PlayerPrefSave.GetMaxStore(objectMouseDown) + DataManager.GameConfig.GetStoreUpgrade(objectMouseDown));
        listMaterial = objectMouseDown == ObjectMouseDown.Silo ? listMaterialSilo : listMaterialStorage;
        tfConten.RecycleChild();
        FillData();
    }

    void FillData()
    {
        tempList = new List<ItemExpand>();
        for (int i = 0; i < listMaterial.Count; i++)
        {
            var item = itemNeed.Spawn(tfConten);
            item.FillData(DataManager.ProductAsset.GetProductByName(listMaterial[i].name), listMaterial[i].count);
            tempList.Add(item);
        }
    }
   
    public void Btn_Upgrade_Click()
    {
        bool isNough = true;
        for (int i = 0; i < tempList.Count; i++)
        {
            if (!tempList[i].isNough)
                isNough = false;
        }
        if (isNough)
        {
            for (int i = 0; i < tempList.Count; i++)
            {
                tempList[i].MinusPoints();
            }
            PlayerPrefSave.SetMaxStore(objectMouseDown, DataManager.GameConfig.GetStoreUpgrade(objectMouseDown));
            PlayerPrefSave.UpLevelStore(objectMouseDown);
            uIAnimation.Hide();
            this.PostEvent((int)EventID.OnUpgradeComplite, objectMouseDown);

            AnalyticsManager.LogEvent("upgrade_"+ objectMouseDown, new Dictionary<string, object> {
            { "level", PlayerPrefSave.GetLevelStore(objectMouseDown) },
            { "time", DataManager.UserData.TotalTimePlay } });
        }
        else
        {
            UIToast.Show("Not enought materal", null, ToastType.Notification, 1f);
        }
    }
}
