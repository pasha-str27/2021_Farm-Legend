using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UiSuggestions : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] ObjFollow objFollows;
    [SerializeField] ItemDrag[] itemDrags;
    [SerializeField] ToggleSuggestions do_list;
    [SerializeField] Sprite spBasket, spSickle;
    [SerializeField] UiRequirements uiRequirements;
    [SerializeField] RectTransform bgRequiment;
    [SerializeField] float minWith;
    [SerializeField] float currentWith;

    List<ProductData> tempList;
    ObjectMouseDown objectMouseDown;
    private void OnEnable()
    {
        for (int i = 0; i < itemDrags.Length; i++)
        {
            itemDrags[i].gameObject.SetActive(false);
        }
        this.RegisterListener((int)EventID.OnDragItem, OnDragItemHandle);
        this.RegisterListener((int)EventID.OnLoadToggleSuggestion, OnLoadToggleSuggestionHandle);
        this.RegisterListener((int)EventID.OnHideToggleSuggestion, OnHideToggleSuggestionHandle);
        this.RegisterListener((int)EventID.OnShowRequirements, OnShowRequirementsHandle);
    }

    private void OnHideToggleSuggestionHandle(object obj)
    {
        Hide();
    }

    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnDragItem, OnDragItemHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLoadToggleSuggestion, OnLoadToggleSuggestionHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnHideToggleSuggestion, OnHideToggleSuggestionHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowRequirements, OnShowRequirementsHandle);
    }

    private void OnShowRequirementsHandle(object obj)
    {
        uiRequirements.gameObject.SetActive(true);
        var data = (ProductData)obj;
        if (data.tabName == TabName.Crops)
        {
            uiRequirements.ShowCoin(data);
            return;
        }
        uiRequirements.Show(data);
        bgRequiment.sizeDelta = new Vector2( minWith 
            + currentWith * (data.requirements.Count - 2> 0?data.requirements.Count - 2 :0),
            bgRequiment.rect.height);
    }
    
    private void OnLoadToggleSuggestionHandle(object obj)
    {
        LoadToggleSuggestion((int)obj);
    }

    private void OnDragItemHandle(object obj)
    {
        var msg = (MessagerDragItem)obj;
        var follow = objFollows.Spawn();
        follow.transform.position = msg.pos;
        follow.FillData(msg.productData, msg.typeObject, GetIconHarvest(msg.typeObject));

        uiRequirements.gameObject.SetActive(false);

        if (msg.typeObject != ObjectMouseDown.Factory)
        {
            Hide();
        }
    }

    Sprite GetIconHarvest(ObjectMouseDown type)
    {
        if (type == ObjectMouseDown.Crops)
            return spSickle;
        if (type == ObjectMouseDown.Factory || type == ObjectMouseDown.Cage || type == ObjectMouseDown.OldTree)
            return spBasket;
        return null;
    }

    public void Show(MessageObject msg)
    {
        uIAnimation.Show();
        objectMouseDown = msg.type;
        do_list.gameObject.SetActive(false);
        uiRequirements.gameObject.SetActive(false);
        switch (msg.type)
        {
            case ObjectMouseDown.Crops:
                tempList = DataManager.ProductAsset.GetListType(TabName.Crops);
                ProductData data = DataManager.ProductAsset.GetProductUnlockNext(TabName.Crops);
                if (data != null)
                    tempList.Add(data);
                if (msg.isHarvest)
                {
                    //Thu hoạch
                    itemDrags[0].gameObject.SetActive(true);
                    itemDrags[0].FillData(null, objFollows, msg.type, spSickle);
                }
                else
                    LoadToggleSuggestion(0);
                break;
            case ObjectMouseDown.MapLock:

                break;
            case ObjectMouseDown.Garbage:
                if (msg.name.Contains("tree"))
                {
                    //cua
                    itemDrags[0].gameObject.SetActive(true);
                    itemDrags[0].FillData(DataManager.ProductAsset.GetProduct(TabName.Material, "Saw"), objFollows, msg.type);
                }
                else
                {
                    if (msg.name.Contains("stone"))
                    {
                        //da
                        itemDrags[0].gameObject.SetActive(true);
                        itemDrags[0].FillData(DataManager.ProductAsset.GetProduct(TabName.Material, "Hammer"), objFollows, msg.type);
                    }
                    else
                    {
                        //xeng
                        itemDrags[0].gameObject.SetActive(true);
                        itemDrags[0].FillData(DataManager.ProductAsset.GetProduct(TabName.Material, "Shovel"), objFollows, msg.type);
                    }
                }
                break;
            case ObjectMouseDown.Cage:
                Debug.Log("Name of Product: " + msg.name);
                if (msg.isHarvest && msg.isRaising)
                {
                    //Thu hoạch va cho an
                    itemDrags[0].gameObject.SetActive(true);
                    itemDrags[0].FillData(null, objFollows, msg.type, spBasket);
                    itemDrags[1].gameObject.SetActive(true);
                    itemDrags[1].FillData(DataManager.ProductAsset.GetProductByName(msg.name), objFollows, msg.type);
                }
                else if(msg.isHarvest && !msg.isRaising)
                {
                    // Thu hoach
                    itemDrags[0].gameObject.SetActive(true);
                    itemDrags[0].FillData(null, objFollows, msg.type, spBasket);
                }
                else if(!msg.isHarvest && msg.isRaising)
                {
                    // Cho an
                    itemDrags[0].gameObject.SetActive(true);
                    itemDrags[0].FillData(DataManager.ProductAsset.GetProductByName(msg.name), objFollows, msg.type);
                }
                break;
            case ObjectMouseDown.Factory:
                tempList = DataManager.FactoryAsset.GetAllProductFactory(msg.name);
                LoadToggleSuggestion(0);
                break;
            case ObjectMouseDown.OldTree:
                if (msg.isHarvest)
                {
                    //Thu hoạch
                    itemDrags[0].gameObject.SetActive(true);
                    itemDrags[0].FillData(null, objFollows, msg.type, spBasket);
                }
                else
                {
                    //chặt
                    itemDrags[0].gameObject.SetActive(true);
                    itemDrags[0].FillData(DataManager.ProductAsset.GetProduct(TabName.Material, "Saw"), objFollows, msg.type);
                }
                break;
        }
    }

    public void Hide()
    {
        this.PostEvent((int)EventID.OnShowUIFactory,new MessagerUiFactory { isShow = false});
        this.PostEvent((int)EventID.OnZoomCamera, false);
        uIAnimation.Hide();

        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 2)
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 0:
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
    }

    void LoadToggleSuggestion(int index)
    {
        for (int i = 0; i < itemDrags.Length; i++)
        {
            itemDrags[i].gameObject.SetActive(false);
        }

        int temp = 0;
        for (int i = index * 5; i < tempList.Count; i++)
        {
            if (temp > itemDrags.Length - 1)
                break;
            itemDrags[temp].gameObject.SetActive(true);
            itemDrags[temp].FillData(tempList[i], objFollows, objectMouseDown);
            temp++;
        }
        do_list.gameObject.SetActive(tempList.Count > itemDrags.Length);
        do_list.FillData(tempList.Count);
    }
}
