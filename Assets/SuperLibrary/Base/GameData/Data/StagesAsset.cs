using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "StagesAsset", menuName = "DataAsset/StagesAsset")]
public class StagesAsset : BaseAsset<StageData>
{
    [SerializeField] UnityEngine.Object[] allSceneStages = null;
    public List<StageSaveData> stageSaveList
    {
        get => list.Where(x => x.isUnlocked || x.unlockPay > 0).Select(x => new StageSaveData {
            totalComplete = x.totalComplete,
            totalFail = x.totalFail,
            totalPlay = x.totalPlay,
            totalReborn = x.totalReborn,
            totalRestart = x.totalRestart,
            totalTimePlay = x.totalTimePlay,
            process = x.process,
            processIndex = x.processIndex,
            score = x.score,
            star = x.star,
            combo = x.combo,
            time = x.time,
        }).ToList();
    }

    public StageData GetStageByIndex(int index)
    {
        var nextStage = list.FirstOrDefault(s=>s.index == index);
        if (nextStage != null)
        {
            nextStage.isUnlocked = true;
            Current = nextStage;
        }
        return Current;
    }
    public StageData GetRandom()
    {
        var randomList = list.OrderBy(x => System.Guid.NewGuid()).ToList();
        var nextStage = randomList.FirstOrDefault() as StageData;
        if (nextStage != null)
        {
            nextStage.processIndex = 0;
            nextStage.isUnlocked = true;
            Current = nextStage;
        }
        return Current;
    }

    public override void ResetData()
    {
        base.ResetData();

        for (int i = 0; i < list.Count; i++)
        {
            list[i].totalComplete = 0;
            list[i].totalFail = 0;
            list[i].totalPlay = 0;
            list[i].totalReborn = 0;
            list[i].totalRestart = 0;
            list[i].totalTimePlay = 0;
            list[i].process = 0;
            list[i].score = 0;
            list[i].star = 0;
            list[i].combo = 0;
            list[i].time = 0;
        }
    }
    public void AddAllStages()
    {
        list.Clear();

        for (int i = 0; i < allSceneStages.Length; i++)
        {
            try
            {
                var stage = new StageData();
                stage.name = "Stage " + (i + 1);
                stage.id = allSceneStages[i].name;
                stage.index = i;
                stage.totalComplete = 0;
                stage.totalFail = 0;
                stage.totalPlay = 0;
                stage.totalReborn = 0;
                stage.totalRestart = 0;
                stage.totalTimePlay = 0;
                stage.process = 0;
                stage.score = 0;
                stage.star = 0;
                stage.combo = 0;
                stage.time = 0;
                list.Add(stage);
            }
            catch (Exception ex)
            {
                Debug.LogError("AddAllStages: " + i + " " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}

[Serializable]
public class StageData : StageSaveData
{
}
[Serializable]
public class StageSaveData : SaveData
{
    [Header("StageData")]
    public int processIndex = 0;
    public int totalPlay = 0;
    [HideInInspector]
    public int star;
    [HideInInspector]
    public int process;
    [HideInInspector]
    public int score;
    [HideInInspector]
    public int combo;
    [HideInInspector]
    public float time;
    [HideInInspector]
    public long totalTimePlay = 0;
    [HideInInspector]
    public int totalRestart = 0;
    [HideInInspector]
    public int totalReborn = 0;
    [HideInInspector]
    public int totalPass = 0;
    [HideInInspector]
    public int totalFail = 0;
    [HideInInspector]
    public int totalComplete = 0;
}

[Serializable]
public class StageCloud
{
    public int index = 1;
    public string name;
    public string id;
    public string description;
    public bool publish = true;
    public int unlockPrice = 1;
}

