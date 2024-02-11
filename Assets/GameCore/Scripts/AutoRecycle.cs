using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecycle : MonoBehaviour
{
    [SerializeField] bool isHide;
    [SerializeField] float timeDelay = 1.5f;

    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnLevelUp, OnLevelUpHandle);
        Invoke("Delay", timeDelay);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLevelUp, OnLevelUpHandle);
    }

    private void OnLevelUpHandle(object obj)
    {
        Delay();
    }

    void Delay()
    {
        if (isHide)
            gameObject.SetActive(false);
        else
            gameObject.Recycle();
    }
}
