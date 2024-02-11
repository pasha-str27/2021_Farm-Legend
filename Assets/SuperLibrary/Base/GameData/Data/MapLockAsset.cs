using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "MapLockAsset", menuName = "DataAsset/MapLockAsset")]
public class MapLockAsset : BaseAsset<DataMapLock>
{
    [Header("Data json")]
    [SerializeField] string data_json;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<MapLockData> lisJson;

    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        lisJson = JsonUtility.FromJson<ListDataMapLock>(data_json).mapLockDatas;
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
                DataMapLock stage = new DataMapLock();
                stage.name = list.Count + "";
                stage.id = list.Count + "";
                stage.index = list.Count;
                stage.isUnlocked = false;
                stage.unlockType = UnlockType.Gold;
                stage.isSelected = false;

                stage.coin = lisJson[i].coin;
                stage.requirements = lisJson[i].items;
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
public class DataMapLock : SaveData
{
    public int coin;
    public List<Requirement> requirements;
    public bool unlocked
    {
        set
        {
            PlayerPrefs.SetString("unlockedDataMapLock" + id, value == true ? "true" : "false");
            isUnlocked = value;
        }
        get
        {
            isUnlocked = PlayerPrefs.GetString("unlockedDataMapLock" + id).Equals("true");
            return isUnlocked;
        }
    }
}

[System.Serializable]
public class MapLockData
{
    public int coin;
    public List<Requirement> items;
}

[System.Serializable]
public class ListDataMapLock
{
    public List<MapLockData> mapLockDatas;
}
#endregion