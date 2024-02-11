using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiExit : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] GameObject objThank;
    public void Show()
    {
        objThank.SetActive(false);
        uIAnimation.Show();
    }
    public void Hide()
    {
        uIAnimation.Hide();
    }
    public void Btn_Yes_Click()
    {
        objThank.SetActive(true);
        AdsManager.ShowFullNormal(() => {
            AnalyticsManager.LogEvent("exit_game", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay },
            { "total_coin", DataManager.UserData.totalCoin },
            { "total_diamond", DataManager.UserData.totalDiamond } });
            Invoke("DelayQuit", 1f);
        }, () => {
            AnalyticsManager.LogEvent("ShowFullNormal_fail", new Dictionary<string, object> {
            { "action", "Exit Game" }});
            Invoke("DelayQuit", 1f);
        });
    }
    void DelayQuit()
    {
        Application.Quit();
    }
}
