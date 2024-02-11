using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiIAPReward : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] ItemIAP[] itemVideos;
    [SerializeField] ItemIAPReward[] itemVideo;
    [SerializeField] Text txtName;
    public void Show()
    {
        uIAnimation.Show();
        //txtName.text = DataManager.LanguegesAsset.GetName(txtName.text);
        //txtName.font = GameUIManager.FontVietnamese;
        for (int i = 0; i < itemVideo.Length; i++)
        {
            itemVideo[i].FillData(itemVideos[i]);
        }
    }
    public void Hide()
    {
        uIAnimation.Hide();
    }
}
[System.Serializable]
public class ItemIAP
{
    public int id;
    public string IAPID;
    public TypeAds typeAds;
    public Sprite icon;
    public int coin;
    public int time;
    
    public int GetCoin
    {
        get { 
            if(typeAds == TypeAds.Diamond)
            {
                return coin;
            }
            return coin /*+ (int)(coin * (PlayerPrefSave.Level-1) * DataManager.GameConfig.mutil);*/ ; }
    }
}

public enum TypeAds
{
    Coin, Diamond
}