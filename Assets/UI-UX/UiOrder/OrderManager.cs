using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;
    [SerializeField] Transform car;
    [SerializeField] GameObject[] orderViews;
    [SerializeField] Sprite spNone, spComplete;
    [ReadOnly] [SerializeField] List<OrderData> orderDatas;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DataManager.OrderAsset.LoadNewOrder();
        LoadViewOrder();
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnLevelUp, OnLevelUpHanlde);
        this.RegisterListener((int)EventID.OnLoadUiOrderMap, OnLoadUiOrderMapHanlde, DispatcherType.Late);
        this.RegisterListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle, DispatcherType.Late);
        this.RegisterListener((int)EventID.OnUpdateBonus, OnUpdateBonusHandle, DispatcherType.Late);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLevelUp, OnLevelUpHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLoadUiOrderMap, OnLoadUiOrderMapHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUpdateBonus, OnUpdateBonusHandle);
    }
    private void OnUpdateBonusHandle(object obj)
    {
        PlayerPrefSave.bonusOrder++;
        if (PlayerPrefSave.bonusOrder >= DataManager.GameConfig.GetCountBonusOder)
        {
            PlayerPrefSave.bonusOrder = 0;
            ProductData data = DataManager.ProductAsset.GetProductByName("Nails");
            data.total++;
            this.PostEvent((int)EventID.OnFxMaterial, new MessageFx { data = data, pos = transform.position });
            AnalyticsManager.LogEvent("Bonus_Send_order", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "name_material", data.name },
            { "item_total", data.total } });
        }
    }
    private void OnViewCamTutorialHandle(object obj)
    {
        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 9)
        {
            Vector3 temp = transform.position;
            temp.y += 1.5f;
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 0:

                    break;
                case 4:
                    temp = car.position;
                    temp.y += 0.5f;
                    break;
            }

            this.PostEvent((int)EventID.OnLockCamera, true);

            //do somthing
            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                pos = temp,
            });
        }
    }
    private void OnLoadUiOrderMapHanlde(object obj)
    {
        LoadViewOrder();
    }

    private void OnLevelUpHanlde(object obj)
    {
        DataManager.OrderAsset.LoadNewOrder();
        LoadViewOrder();
    }

    void LoadViewOrder()
    {
        for (int i = 0; i < orderViews.Length; i++)
        {
            orderViews[i].SetActive(false);
        }
        orderDatas = DataManager.OrderAsset.list;
        for (int i = 0; i < orderDatas.Count; i++)
        {
            if (i < orderViews.Length)
            {
                orderViews[i].SetActive(true);
                orderViews[i].GetComponent<SpriteRenderer>().sprite = spNone;
                if (orderDatas[i].checkCompleteOrder)
                {
                    orderViews[i].GetComponent<SpriteRenderer>().sprite = spComplete;
                }
            }
        }
    }
}

