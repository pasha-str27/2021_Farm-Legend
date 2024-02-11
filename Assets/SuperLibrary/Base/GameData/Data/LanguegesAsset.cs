using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "LanguegesAsset", menuName = "DataAsset/LanguegesAsset")]
public class LanguegesAsset : BaseAsset<DataLangueges>
{
    [Header("Data json")]
    [SerializeField] string data_json;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<LanguegesData> lisJson;

    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        lisJson = JsonUtility.FromJson<ListDataLangueges>(data_json).languegesDatas;
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
                DataLangueges stage = new DataLangueges();
                stage.id = list.Count + "";
                stage.index = list.Count;
                stage.isUnlocked = false;
                stage.unlockType = UnlockType.Gold;
                stage.isSelected = false;
                stage.name = lisJson[i].English;
                stage.nameV = lisJson[i].VietNam;
                list.Add(stage);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AddAll LanguegesAsset: " + i + " " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
    public string GetName(string text)
    {
        DataLangueges data = list.FirstOrDefault(x => x.name.Trim().Equals(text.Trim()));
        if(data == null)
        {
            data = list.FirstOrDefault(x => x.nameV.Trim().Equals(text.Trim()));
        }
        return data ==null? text: data.GetName;
    }
}
#region object data
[System.Serializable]
public class DataLangueges : SaveData
{
    public string nameV;
    public string GetName
    {
        get
        {
            if (Util.isVietnamese)
                return nameV;
            return name;
        }
    }
}

[System.Serializable]
public class LanguegesData
{
    public string English;
    public string VietNam;
}

[System.Serializable]
public class ListDataLangueges
{
    public List<LanguegesData> languegesDatas;
}
#endregion