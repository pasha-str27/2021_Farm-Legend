using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AchievementManager : MonoBehaviour
{
    [SerializeField] GameObject notifi;
    List<AchievementData> achievementDatas;
    private void Start()
    {
        achievementDatas = DataManager.AchievementAsset.list;
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnUpdateAchie, OnUpdateAchieHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUpdateAchie, OnUpdateAchieHandle);
    }

    private void OnUpdateAchieHandle(object obj)
    {
        var msg = (int)obj;
        AchievementData data = achievementDatas[msg];
        data.countAchire++;
        if (data.countAchire >= data.maxAchire)
        {
            //complete AchieHandle
            //notifi?.SetActive(true);
        }
    }
}
