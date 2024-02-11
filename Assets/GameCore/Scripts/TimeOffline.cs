using System;
using UnityEngine;

public class TimeOffline : MonoBehaviour
{
    public static int CurrentTime(string dateTime)
    {
        if (dateTime == string.Empty)
        {
            return 0;
        }
        return (int)Math.Round(DateTime.Now.Subtract(Convert.ToDateTime(dateTime)).TotalSeconds);
    }

    public static string ConvertTime(int time)
    {
        int hours   = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if(hours > 0)
            return string.Format("{0:00h}:{1:00m}:{2:00s}", hours, minutes, seconds);
        if (minutes > 0)
            return string.Format("{0:00m}:{1:00s}", minutes, seconds);
        return string.Format("{0:00s}", seconds);
    }
}
