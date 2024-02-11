using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "LevelAsset", menuName = "DataAsset/LevelAsset")]
public class LevelAsset : BaseAsset<DataLevel>
{
    [Header("Level design")]
    [SerializeField] float mutilExp = .5f;
    [SerializeField] float mutilCoin = .1f;
    [Header("Data json")]
    [SerializeField] string data_json;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<LevelData> lisJson;

    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        lisJson = JsonUtility.FromJson<ListDataLevel>(data_json).levelDatas;
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
                DataLevel stage = new DataLevel();
                stage.name = list.Count + "";
                stage.id = list.Count + "";
                stage.index = list.Count;
                stage.isUnlocked = false;
                stage.unlockType = UnlockType.Gold;
                stage.isSelected = false;

                stage.level = lisJson[i].level;
                stage.exp = lisJson[i].exp;
                stage.coin = lisJson[i].coin;
                stage.gem = lisJson[i].gem;
                list.Add(stage);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AddAllExercises: " + i + " " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
    public void Add(DataLevel data)
    {
        list.Add(data);
    }
    public DataLevel CurrentDataLevel
    {
        get
        {
            if (PlayerPrefSave.Level + 1 > list.Count - 1)
                return list[list.Count - 1];
            return list[PlayerPrefSave.Level];
        }
    }
    //public DataLevel NextDataLevel 
    //{
    //    get
    //    {
    //        DataLevel data = CurrentDataLevel;
    //        if (PlayerPrefSave.Level + 1 > list.Count - 1)
    //        {
    //            data.exp += (int)(data.exp * mutilExp);
    //            data.coin += (int)(data.coin * mutilCoin);
    //        }
    //        return data;
    //    }
    //}
    public DataLevel NextDataLevel
    {
        get
        {
            DataLevel data = CurrentDataLevel;
            if (PlayerPrefSave.Level + 1 > list.Count - 1)
            {
                data = new DataLevel(); // 2,147,483,647
                data.exp = CurrentDataLevel.exp + (int)(list[list.Count - 1].exp * mutilExp);
                if (CurrentDataLevel.exp + (int)(list[list.Count - 1].exp * mutilExp)>=2147000000)
                    data.exp = 2147000000;
                data.coin = CurrentDataLevel.coin + (int)(list[list.Count - 1].coin * mutilCoin);
                data.name = (PlayerPrefSave.Level + 1) + "";
                list.Add(data);
            }
            return data;
        }
    }
}
#region object data
[System.Serializable]
public class DataLevel : SaveData
{
    public int level;
    public int exp;
    public int coin;
    public int gem;
}

[System.Serializable]
public class LevelData
{
    public int level;
    public int exp;
    public int coin;
    public int gem;
}

[System.Serializable]
public class ListDataLevel
{
    public List<LevelData> levelDatas;
}
#endregion