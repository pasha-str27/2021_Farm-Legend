using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ObjFollow : MonoBehaviour
{
    [SerializeField] public bool isTrigger;
    [SerializeField] public SpriteRenderer spIcon;
    [SerializeField] public ProductData productData;
    [SerializeField] public ObjectMouseDown type;


    public void FillData(ProductData productData, ObjectMouseDown type, Sprite sprite = null)
    {
        isTrigger = false;
        this.type = type;
        if (productData == null)
        {
            spIcon.sprite = sprite;
            this.productData = null;
            return;
        }
        this.productData = productData;
        //spIcon.sprite = sprites.FirstOrDefault(x => x.name.Contains(productData.name));
        spIcon.sprite = productData.icon;

    }
    private void OnEnable()
    {
        this.PostEvent((int)EventID.OnLockCamera, true);
        this.RegisterListener((int)EventID.OnLevelUp, OnLevelUpHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLevelUp, OnLevelUpHandle);
    }

    private void OnLevelUpHandle(object obj)
    {
        if (type == ObjectMouseDown.Factory)
        {
            this.PostEvent((int)EventID.OnThaTayFactory, productData);
        }
        this.PostEvent((int)EventID.OnLockCamera, false);
        try
        {
            gameObject.Recycle();
        }
        catch { }
    }

    private void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
        if (Input.GetMouseButtonUp(0))
        {
            if (type == ObjectMouseDown.Factory)
            {
                this.PostEvent((int)EventID.OnThaTayFactory, productData);
            }
            this.PostEvent((int)EventID.OnLockCamera, false);

            if (isTrigger)
            {
                isTrigger = false;
                if (PlayerPrefSave.stepTutorial == 0)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 2:
                            PlayerPrefSave.stepTutorialCurrent = 3;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                        case 5:
                            PlayerPrefSave.stepTutorialCurrent = 6;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
                if (PlayerPrefSave.stepTutorial == 7)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 2:
                            PlayerPrefSave.stepTutorialCurrent = 3;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
            }
            else
            {
                if (PlayerPrefSave.stepTutorial == 0)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 1:
                            PlayerPrefSave.stepTutorialCurrent = 0;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                        case 2:
                            PlayerPrefSave.stepTutorialCurrent = 0;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                        case 4:
                            PlayerPrefSave.stepTutorialCurrent = 3;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
            }
            
            gameObject.Recycle();
        }
    }
}