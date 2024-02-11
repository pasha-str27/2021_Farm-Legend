using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void ExitGameHandle()
    {
        AdsManager.ShowFullNormal(()=> {
            Debug.Log("Exit Game");
            Application.Quit();
        },()=> {
            Application.Quit();
            AnalyticsManager.LogEvent("ShowFullNormal_fail", new Dictionary<string, object> {
            { "action", "Exit Game" }});
        });
    }
}
