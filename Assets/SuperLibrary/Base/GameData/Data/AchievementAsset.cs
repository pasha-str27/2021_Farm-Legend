using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "AchievementAsset", menuName = "DataAsset/AchievementAsset")]
public class AchievementAsset : BaseAsset<AchievementData>
{
    [Header("Level design")]
    [SerializeField] public float numMax = .5f;
    [SerializeField] public float numExp = .5f;
    [SerializeField] public float numDiamond = .2f;

    [Header("Data json")]
    [SerializeField] string data_json;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<DataAchievement> lisJson;

    [Header("Assets")]
    [SerializeField] Sprite[] spIcon;
    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        lisJson = JsonUtility.FromJson<ListDataAchievement>(data_json).dataAchievement;
        AddData();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    public override void ResetData()
    {
        base.ResetData();
        AddAllData();
    }
    private void AddData()
    {
        for (int i = 0; i < lisJson.Count; i++)
        {
            try
            {
                AchievementData stage = new AchievementData();
                stage.id = list.Count + "";
                stage.index = list.Count;
                stage.name = lisJson[i].name;
                stage.nameV = lisJson[i].nameV;
                stage.icon = spIcon[i];
                stage.exp = lisJson[i].exp;
                stage.count = lisJson[i].count;
                stage.diamond = lisJson[i].diamond;
                stage.description = lisJson[i].des;
                stage.descriptionV = lisJson[i].desV;

                list.Add(stage);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AddAllExercises: " + i + " " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
#region object data
[System.Serializable]
public class AchievementData : SaveData
{
    [Header("AchievementData")]
    public string nameV;
    public string descriptionV;
    public int exp;
    public int diamond;
    public string GetName
    {
        get
        {
            if (Util.isVietnamese)
                return nameV;
            return name;
        }
    }
    public string GetDes
    {
        get
        {
            if (Util.isVietnamese)
                return descriptionV;
            return description;
        }
    }
    public int maxAchire
    {
        get { return (int)(count + count*level * DataManager.AchievementAsset.numMax); }
    }
    public int getExp
    {
        get { return (int)(exp + level * DataManager.AchievementAsset.numExp); }
    }
    public int getDiamond
    {
        get { return (int)(diamond + level * DataManager.AchievementAsset.numDiamond); }
    }
    public int level
    {
        get { return PlayerPrefs.GetInt("AchievementLevel" + id, 0); }
        set { PlayerPrefs.SetInt("AchievementLevel" + id, value); }
    }

    
    public int countAchire
    {
        get { return PlayerPrefs.GetInt("countAchire" + id, 0); }
        set { PlayerPrefs.SetInt("countAchire" + id, value); }
    }
    public Sprite icon;
}

//object json
[System.Serializable]
public class DataAchievement
{
    public string name;
    public string nameV;
    public string des;
    public string desV;
    public int count;
    public int exp;
    public int diamond;
    
}
[System.Serializable]
public class ListDataAchievement
{
    public List<DataAchievement> dataAchievement;
}
#endregion