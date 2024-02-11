using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Reflection;

/// <summary>
/// [CreateAssetMenu(fileName = "NameDatasAsset", menuName = "DataAsset/NameDatasAsset")]
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class BaseAsset<T> : ScriptableObject where T : SaveData
{
    public bool useRandom;

    [NonSerialized]
    private T current = null;
    public T Current
    {
        get
        {
            if (current == null || string.IsNullOrEmpty(current.name))
            {
                current = list.LastOrDefault(x => x.isSelected && x.isUnlocked);
            }

            if (current == null || string.IsNullOrEmpty(current.name))
            {
                current = list.FirstOrDefault();
                if (current != null)
                {
                    current.isUnlocked = true;
                    current.isSelected = true;
                }
            }
            return current;
        }
        set
        {
            if (current != value)
            {
                if (current != null)
                    current.isSelected = false;
                current = value;
                current.isSelected = true;
#if UNITY_EDITOR
                Debug.Log(current.GetType() + " OnChanged " + current.id);
#endif
                OnChanged?.Invoke(current, unlockedList);
            }
        }
    }

    [Header("Datas")]
    public List<T> list = new List<T>();

    public delegate void DataChangedDelegate(T current, List<T> list);
    public static event DataChangedDelegate OnChanged;
    public void SetChanged(T data)
    {
        OnChanged?.Invoke(data, unlockedList);
    }

    public List<T> unlockedList
    {
        get
        {
            return list?.Where(x => x.isUnlocked == true && x.unlockPrice >= 0).ToList();
        }
    }


    public List<SaveData> itemSaveList
    {
        get => list.Where(x => x.isUnlocked || x.unlockPay > 0)
            .Select(x => new SaveData { id = x.id, isUnlocked = x.isUnlocked, isSelected = x.isSelected, count = x.count, unlockPay = x.unlockPay }).ToList();
    }

    public void ConvertToData(List<SaveData> saveData)
    {
        foreach (var i in saveData)
        {
            var temp = list.FirstOrDefault(x => x.id == i.id);
            if (temp != null)
            {
                temp.count = i.count;
                temp.unlockPay = i.unlockPay;

                if (i.isUnlocked)
                    temp.isUnlocked = i.isUnlocked;
                if (i.isSelected)
                    temp.isSelected = i.isSelected;
            }
        }
    }

    public void ConvertToData(List<StageSaveData> saveData)
    {
        foreach (var i in saveData)
        {
            try
            {
                StageData temp = list.FirstOrDefault(x => x.id == i.id) as StageData;
                if (temp != null)
                {
                    temp.index = i.index;
                    temp.star = i.star;
                    temp.process = i.process;
                    temp.score = i.score;
                    temp.combo = i.combo;
                    temp.count = i.count;
                    temp.totalComplete = i.totalComplete;
                    temp.totalFail = i.totalFail;
                    temp.totalPass = i.totalPass;
                    temp.totalPlay = i.totalPlay;
                    temp.totalRestart = i.totalRestart;
                    temp.totalTimePlay = i.totalTimePlay;
                    temp.unlockPay = i.unlockPay;
                    temp.processIndex = i.processIndex;

                    if (i.isUnlocked)
                        temp.isUnlocked = i.isUnlocked;
                    if (i.isSelected)
                        temp.isSelected = i.isSelected;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        UpdateIndex();
    }
    public List<T> GetNotUnlockedList(float price)
    {
        return list?.Where(x => !x.isUnlocked && x.unlockPrice <= price).ToList();
    }


    private T lastCheck = null;
    public T GetCanUnlock(UnlockType unlockType, int value, bool random)
    {
        try
        {
            if (random)
            {
                if (lastCheck != null)
                {
                    //Debug.Log("lastCheck: " + lastCheck.id);
                    var tempList = list?.Where(x => x.id != lastCheck.id && x.isUnlocked == false && x.unlockPrice <= value && x.unlockType == unlockType).ToList();
                    if (tempList != null && tempList.Any())
                    {
                        var tempCheck = tempList[UnityEngine.Random.Range(0, tempList.Count())];
                        if (tempCheck != null)
                            return lastCheck = tempCheck;
                    }
                }
                else
                {
                    var tempList = list?.Where(x => x.isUnlocked == false && x.unlockPrice <= value && x.unlockType == unlockType).ToList();
                    if (tempList != null && tempList.Any())
                    {
                        var tempCheck = tempList[UnityEngine.Random.Range(0, tempList.Count())];
                        if (tempCheck != null)
                            return lastCheck = tempCheck;
                    }
                }
            }
            else
            {
                if (lastCheck != null)
                {
                    //Debug.Log("lastCheck: " + lastCheck.id);
                    var tempCheck = list?.FirstOrDefault(x => x.id != lastCheck.id && x.isUnlocked == false && x.unlockPrice <= value && x.unlockType == unlockType);
                    if (tempCheck != null)
                        return lastCheck = tempCheck;
                }
                else
                {
                    var tempCheck = list?.FirstOrDefault(x => x.isUnlocked == false && x.unlockPrice <= value && x.unlockType == unlockType);
                    if (tempCheck != null)
                        return lastCheck = tempCheck;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("GetCanUnlockRandom: " + ex.Message);
        }
        return null;
    }

    public T GetNext(UnlockType unlockType, int index, int value)
    {
        var tempCheck = list?.FirstOrDefault(x => x.index > index && x.unlockPrice <= value && x.unlockType == unlockType);
        
        return tempCheck;
    }
    public void UpdateIdByName()
    {
        foreach (var i in list)
        {
            i.id = i.name.Replace("'", "").Replace(" ", "_");
            if (list.Count(x => x.id == i.id) > 1)
                Debug.LogError(i.index + " " + i.id + " is Duplicated");
        }
        Debug.Log("Update Id By Name");
    }

    public void UpdateIndex()
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].index = i;
            if (list.Count(x => x.id == list[i].id) > 1)
                Debug.LogError(list[i].index + " " + list[i].id + " is Duplicated");
        }
    }

    public void OderByIndex()
    {
        for (int i = 0; i < list.Count; i++)
        {
            list = list.OrderBy(x => x.index).ToList();
            if (list.Count(x => x.id == list[i].id) > 1)
                Debug.LogError(list[i].index + " " + list[i].id + " is Duplicated");
        }
    }

    public void UpdateCost(UnlockType unlockType = UnlockType.Gem)
    {
        Debug.Log("UPDATE COST " + typeof(T).Name);

        var tempList = list.ToList();

        int coinIndex = 0;
        int adsIndex = 0;
        for (int i = 0; i < tempList.Count; i++)
        {
            tempList[i].isUnlocked = i < 1;
            tempList[i].unlockPrice = 0;
            tempList[i].unlockType = unlockType;
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            var item = tempList[i];
            if (item.unlockType == UnlockType.Gold)
            {
                item.unlockPrice = coinIndex * 100;
                coinIndex ++;
                Debug.Log(item.index + " " + item.unlockType + " " + item.unlockPrice + " " + item.id);
            }
            else if (item.unlockType == UnlockType.Ads)
            {
                item.unlockPrice = adsIndex;
                adsIndex ++;
                Debug.Log(item.index + " " + item.unlockType + " " + item.unlockPrice + " " + item.id);
            }
            else
            {
                if(i < 1)
                    tempList[i].isUnlocked = true;
                item.unlockPrice = i;
                Debug.Log(item.index + " " + item.unlockType + " " + item.unlockPrice + " " + item.id);
            }
        }
    }

    public void UnlockAll()
    {
        bool isUnlockedAll = list.Count(x => x.isUnlocked) >= list.Count();
        foreach (var i in list)
            i.isUnlocked = !isUnlockedAll;
        list.FirstOrDefault().isUnlocked = true;
    }

    /// <summary>
    /// Reset Data Asset Settings
    /// </summary>
    public virtual void ResetData()
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].count = 0;
            list[i].isSelected = false;
            list[i].isUnlocked = i < 1;
            list[i].unlockPay = 0;
        }
        current = null;
    }

    public void ClearLog()
    {
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
    }
}

public class CloudStages
{
    public string time_created = DateTime.Now.ToString();
    public List<StageCloud> list;

    public CloudStages(List<StageData> list)
    {
        this.list = new List<StageCloud>();
        foreach (var i in list)
        {
            this.list.Add(new StageCloud
            {
                index = i.index,
                id = i.id,
                name = i.name,
                description = i.description,
                unlockPrice = i.unlockPrice,
            });
        }
    }

    public static StageData UpdateToStageData(StageData stageData, StageCloud i)
    {
        stageData.index = i.index;
        stageData.id = i.id;
        stageData.name = i.name;
        stageData.description = i.description;
        stageData.unlockPrice = i.unlockPrice;
        return stageData;
    }
}