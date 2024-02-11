using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CountDownTime : MonoBehaviour
{
    [SerializeField] public string keyId;
    [SerializeField] UnityEvent hanldeEvent;
    [ReadOnly] [SerializeField] int timeCount = 0;
    Coroutine coroutine;
    public void SetKey(string key)
    {
        this.keyId = key;
    }
    public void Init(string keyId, int time)
    {
        this.keyId = keyId;
        if (timeLife > 0)
        {
            timeLife -= Util.timeOffline;
            //Debug.Log("=> CountDownTime[" + keyId + "]=" + Util.timeOffline);
        }
        else
            if (isComplete == 0)
                timeLife = time;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        //Debug.Log("=> CountDownTime " + keyId + " -> " + timeLife + " - timeout = " + (Util.timeNow - Util.timeOut));
        coroutine = StartCoroutine(CountDown());
    }
    public void SpeedUp()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        timeLife = 0;
        isComplete = 1;
        hanldeEvent?.Invoke();
    }
    IEnumerator CountDown()
    {
        if (timeLife <= 0)
        {
            isComplete = 1;
        }
        hanldeEvent?.Invoke();
        timeCount = timeLife;
        yield return new WaitForSeconds(1);
        if (timeLife > 0)
        {
            timeLife--;
            coroutine = StartCoroutine(CountDown());
        }
    }

    public int timeLife
    {
        set
        {
            PlayerPrefs.SetInt("time" + keyId, value);
        }
        get
        {
            return PlayerPrefs.GetInt("time" + keyId, 0);
        }
    }
    public int isComplete
    {
        set { PlayerPrefs.SetInt("timeisComplete" + keyId, value); }
        get
        {
            return PlayerPrefs.GetInt("timeisComplete" + keyId, 0);
        }
    }
}

public class MessagerCountDown
{
    public string keyId;
    public int timeLife;
}
