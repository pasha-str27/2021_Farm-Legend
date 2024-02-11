using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapLockController : MonoBehaviour
{
    [SerializeField] Building building;
    [SerializeField] public PolygonCollider2D box;
    [SerializeField] MapLockController[] mapRequiments;
    [SerializeField] GarbageController[] garbageControllers;
    public int idMap;
    public void Init(int id)
    {
        idMap = id;
        box.enabled = !PlayerPrefSave.GetLockMap(idMap);
        if (PlayerPrefSave.GetLockMap(idMap))
        {
            GridBuildingSystem.instance.DeleteArea(building.area);
            this.PostEvent((int)EventID.OnUnLockMap, this);
        }

        for (int i = 0; i < garbageControllers.Length; i++)
        {
            garbageControllers[i].Init("map" + idMap + i, !PlayerPrefSave.GetLockMap(idMap));
        }
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnExpandMap, OnExpandMapHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnExpandMap, OnExpandMapHandle);
    }

    private void OnExpandMapHandle(object obj)
    {
        var msg = (int)obj;
        if (msg == idMap)
        {
            PlayerPrefSave.UnLockMap(idMap, 1);
            GridBuildingSystem.instance.DeleteArea(building.area);
            Init(idMap);
            //bonus 1 xeng+1cua+1 rui
            DataManager.ProductAsset.GetProduct(TabName.Material, "Shovel").total++;
            DataManager.ProductAsset.GetProduct(TabName.Material, "Saw").total++;
            DataManager.ProductAsset.GetProduct(TabName.Material, "Hammer").total++;

            AnalyticsManager.LogEvent("unlock_map", new Dictionary<string, object> {
            { "name", name },
            { "level", PlayerPrefSave.Level }});
        }
    }
    public void CheckMapRequiment(Action onDone, Action onFail)
    {
        if(mapRequiments==null)
        {
            onDone?.Invoke();
            return;
        }
        bool check = true;
        MapLockController temp = null;
        for (int i = 0; i < mapRequiments.Length; i++)
        {
            if (mapRequiments[i].box.enabled)
            {
                temp = mapRequiments[i];
                check = false;
                break;
            }

        }
        if (check)
            onDone?.Invoke();
        else
        {
            onFail?.Invoke();
            if (temp.GetComponent<MouseDownObject>() != null)
                temp.GetComponent<MouseDownObject>().OnHighlight();
        }
    }
}
