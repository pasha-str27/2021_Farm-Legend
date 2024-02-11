using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UIAchievement : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] ItemAchievement itemAchievement;
    [SerializeField] Transform content;
    [SerializeField] Text txtCount;
    List<AchievementData> achievementDatas;
 
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnClickButtonTab, OnClickButtonTabHandle);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickButtonTab, OnClickButtonTabHandle);
    }

    private void OnClickButtonTabHandle(object obj)
    {
        var msg = (MessagerTab)obj;
        if (msg.typeTab != TypeTab.Achievement)
            return;
        //to do something
    }

    public void Show()
    {
        achievementDatas = DataManager.AchievementAsset.list.OrderByDescending(x=>x.countAchire).ToList();
        uIAnimation.Show();
        content.RecycleChild();
        txtCount.text = achievementDatas.Count + "";
        for (int i = 0; i < achievementDatas.Count; i++)
        {
            ItemAchievement item = itemAchievement.Spawn(content);
            item.FillData(achievementDatas[i]);
        }

        AnalyticsManager.LogEvent("show_achiement", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay } });
    }
    public void Hide()
    {
        uIAnimation.Hide();
    }
}
