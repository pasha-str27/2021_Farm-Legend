using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLockManager : MonoBehaviour
{
    [SerializeField] MapLockController[] mapLockControllers;
    private void Start()
    {
        Invoke("FillData", .5f);
    }
    void FillData()
    {
        for (int i = 0; i < mapLockControllers.Length; i++)
        {
            mapLockControllers[i].Init(i);
        }
    }
}
