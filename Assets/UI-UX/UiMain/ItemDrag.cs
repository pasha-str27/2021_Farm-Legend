using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour
{
    [Header("object in sence")]
    [SerializeField] Image icon;
    [SerializeField] Text txtCount;
    [SerializeField] GameObject objCount;
    [SerializeField] GameObject txtDesFood;
    [SerializeField] ProductData data;
    ObjFollow prefabs;
    [SerializeField] ObjectMouseDown typeObject;

    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnThaTayFactory, OnThaTayFactoryHandle);
    }
    private void OnDisable()
    {
      EventDispatcher.Instance?.RemoveListener((int)EventID.OnThaTayFactory, OnThaTayFactoryHandle);
    }
    private void OnThaTayFactoryHandle(object obj)
    {
        if (data == (ProductData)obj)
        {
            icon.gameObject.SetActive(true);
        }
    }

    public void FillData(ProductData data, ObjFollow prefabs, ObjectMouseDown typeObject, Sprite sprite = null)
    {
        this.typeObject = typeObject;
        this.prefabs = prefabs;
        icon.gameObject.SetActive(true);
        objCount.SetActive(false);

        this.data = null;
        if (data == null)
        {
            icon.sprite = sprite;
            icon.SetNativeSize();
            txtDesFood.SetActive(false);
            return;
        }
        this.data = data;
        icon.sprite = data.unlocked ? data.icon : data.iconLock;
        icon.SetNativeSize();
        txtCount.text = data.total + "";
        objCount.SetActive(true);
        txtDesFood.SetActive(data.tabName == TabName.FoodPet);
    }

    public void Btn_Info_Click()
    {
        if (data == null)
            return;
        if (data.requirements == null || !data.unlocked)
            return;
        if (data.requirements.Count > 0 || data.tabName == TabName.Crops)
        {
            this.PostEvent((int)EventID.OnShowRequirements, data);
        }
    }
    public void On_Drag_Item()
    {
        if (data != null)
            if (!data.unlocked || !icon.gameObject.activeInHierarchy)
                return;

        if (typeObject == ObjectMouseDown.Factory)
        {
            icon.gameObject.SetActive(false);
        }
        else
            gameObject.SetActive(false);

        this.PostEvent((int)EventID.OnDragItem, new MessagerDragItem
        {
            obj = prefabs.gameObject,
            productData = data,
            pos = Camera.main.ScreenToWorldPoint(transform.position),
            typeObject = typeObject,
        });

        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 0 )
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 1:
                    PlayerPrefSave.stepTutorialCurrent = 2;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
                case 4:
                    PlayerPrefSave.stepTutorialCurrent = 5;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
        if (PlayerPrefSave.stepTutorial == 1 || PlayerPrefSave.stepTutorial == 3 
            || PlayerPrefSave.stepTutorial == 4
            || PlayerPrefSave.stepTutorial == 5
            || PlayerPrefSave.stepTutorial == 6
            || PlayerPrefSave.stepTutorial == 7
            || PlayerPrefSave.stepTutorial == 8)
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 1:
                    PlayerPrefSave.stepTutorialCurrent = 2;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
        
    }
}

