using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiVideoReward : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] ItemVideo[] itemVideos;
    [SerializeField] ItemVideoReward[] itemVideo;
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
public class ItemVideo
{
    public int id;
    public TypeAds typeAds;
    public Sprite icon;
    public int coin;
    public int countAds;
    public int time;
    
    public int GetCoin
    {
        get { 
            if(typeAds == TypeAds.Diamond)
            {
                return coin;
            }
            return coin + (int)(coin * (PlayerPrefSave.Level-1) * DataManager.GameConfig.mutil); ; }
    }
}
