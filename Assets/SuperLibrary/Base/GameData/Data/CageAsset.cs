using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "CageAsset", menuName = "DataAsset/CageAsset")]
public class CageAsset : BaseAsset<DataCage>
{
    [Header("Data json")]
    [SerializeField] string data_json;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<CageData> lisJson;

    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        //lisJson = JsonUtility.FromJson<ListDataCage>(data_json).cageDatas;
        //AddData();
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
                DataCage stage = new DataCage();
                stage.id = list.Count + "";
                stage.index = list.Count;
                stage.isUnlocked = false;
                stage.unlockType = UnlockType.Gold;
                stage.isSelected = false;
                stage.name = lisJson[i].name + "";
                stage.products = lisJson[i].products;
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
public class DataCage : SaveData
{
    public List<string> products;
}

[System.Serializable]
public class CageData
{
    public string name;
    public List<string> products;
}

[System.Serializable]
public class ListDataCage
{
    public List<CageData> cageDatas;
}
#endregion