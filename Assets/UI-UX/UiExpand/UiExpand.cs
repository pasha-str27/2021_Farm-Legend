using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TypePopup
{
    Expand, GoldMine,
}
public class UiExpand : MonoBehaviour
{
    [SerializeField] TypePopup typePopup;
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] ItemExpand[] itemNeeds;
    [SerializeField] string[] nameItem;
    MessageObject msg;
    int coin = 0, numMaterial = 0;
    int tempIndex = 0;
    public void Show(MessageObject msg)
    {
        this.msg = msg;
        uIAnimation.Show();

        coin = typePopup == TypePopup.Expand ? DataManager.GameConfig.coinMap : DataManager.GameConfig.goldMine;
        numMaterial = typePopup == TypePopup.Expand ? DataManager.GameConfig.countMaterialMap : DataManager.GameConfig.numMaterialGoldmine;
        tempIndex = 0;
        itemNeeds[0].FillDataCoin(coin);
        for (int i = 1; i < itemNeeds.Length; i++)
        {
            itemNeeds[i].FillData(DataManager.ProductAsset.GetProductByName(nameItem[tempIndex]), numMaterial);
            tempIndex++;
        }
    }
    public void Hide()
    {
        uIAnimation.Hide();
    }

    public void Btn_Expand_Click()
    {
        bool isNough = true;
        for (int i = 0; i < itemNeeds.Length; i++)
        {
            if (!itemNeeds[i].isNough)
                isNough = false;
        }
        if (isNough)
        {
            for (int i = 0; i < itemNeeds.Length; i++)
            {
                itemNeeds[i].MinusPoints();
            }
            if (typePopup == TypePopup.Expand)
            {
                this.PostEvent((int)EventID.OnExpandMap, msg.idMap);
                PlayerPrefSave.LevelMap++;
            }
            else
            {
                this.PostEvent((int)EventID.OnExploitGoldMine);
            }
            Hide();
        }
        else
        {
            this.PostEvent((int)EventID.OnShowVideoReward);
            UIToast.Show("Not enought materal!", null, ToastType.Notification, 1f);
        }
    }
}
