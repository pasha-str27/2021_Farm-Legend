using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class UITutorial : MonoBehaviour
{
    [SerializeField] bool isTutorial;
    [SerializeField] int step;
    [SerializeField] int stepCurrent;

    [SerializeField] Image lockScreen;
    [SerializeField] Button buttonHideUiSub;
    [SerializeField] GameObject LockCenter;
    [SerializeField] GameObject LockDragSuggestion;
    [SerializeField] GameObject tutHandCenter;
    [SerializeField] GameObject tutHandDragSuggestion;
    [SerializeField] GameObject tutHandClickOk;
    [SerializeField] GameObject tutHandShopDrag1;
    [SerializeField] GameObject tutHandShopDrag2;
    [SerializeField] GameObject tutHandBtnShop;

    [Header("Order")]
    [SerializeField] GameObject tutHandOrder;
    [SerializeField] GameObject tutHandSendOrder;
    [Header("Shop - chicken")]
    [SerializeField] GameObject tutHandShopTabAnimal;
    [SerializeField] GameObject tutHandSpeedUp;
    [SerializeField] GameObject lockSpeedUp;
    [SerializeField] Button buttonHideUiCoundown;
    [Header("shop - factory")]
    [SerializeField] GameObject tutHandShopTabFactory;
    private void Awake()
    {
        if (PlayerPrefSave.IsTutorial)
        {
            switch (PlayerPrefSave.stepTutorial)
            {
                case 0:
                    if (PlayerPrefSave.stepTutorialCurrent < 2)
                        PlayerPrefSave.stepTutorialCurrent = 0;

                    if (PlayerPrefSave.stepTutorialCurrent == 2)
                        PlayerPrefSave.stepTutorialCurrent = 3;

                    if (PlayerPrefSave.stepTutorialCurrent == 6)
                    {
                        PlayerPrefSave.stepTutorial = 1;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                    }
                    break;
                case 1:
                    PlayerPrefSave.stepTutorialCurrent = 0;
                    break;
                case 2:
                    //if (PlayerPrefSave.Level < 2)
                    //    PlayerPrefSave.Level = 2;

                    if (PlayerPrefSave.stepTutorialCurrent <= 3)
                        PlayerPrefSave.stepTutorialCurrent = 0;
                    break;
                case 3:
                    if (PlayerPrefSave.stepTutorialCurrent <= 1)
                        PlayerPrefSave.stepTutorialCurrent = 0;
                    if (PlayerPrefSave.stepTutorialCurrent == 2)
                        PlayerPrefSave.stepTutorialCurrent = 3;
                    if (PlayerPrefSave.stepTutorialCurrent == 4)
                        PlayerPrefSave.stepTutorialCurrent = 3;
                    if (PlayerPrefSave.stepTutorialCurrent == 6)
                        PlayerPrefSave.stepTutorialCurrent = 5;
                    break;
                case 4:
                    //if (PlayerPrefSave.Level < 2)
                    //    PlayerPrefSave.Level = 2;

                    if (PlayerPrefSave.stepTutorialCurrent < 3)
                        PlayerPrefSave.stepTutorialCurrent = 0;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    PlayerPrefSave.stepTutorialCurrent = 0;
                    break;
            }
        }
    }
    private void OnEnable()
    {
        GameStateManager.OnStateChanged += GameStateManager_OnStateChanged;
        this.RegisterListener((int)EventID.OnLoadTutorial, OnLoadTutorialHandle);
        this.RegisterListener((int)EventID.OnLevelUp, OnLevelUpHandle, DispatcherType.Late);
        this.RegisterListener((int)EventID.OnShowUIMove, OnShowUIMoveHandle);
        this.RegisterListener((int)EventID.OnShowHandTutorial, OnShowHandTutorialHandle);
    }
    private void OnDisable()
    {
        GameStateManager.OnStateChanged -= GameStateManager_OnStateChanged;
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLoadTutorial, OnLoadTutorialHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLevelUp, OnLevelUpHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowUIMove, OnShowUIMoveHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowHandTutorial, OnShowHandTutorialHandle);
    }

    private void OnShowHandTutorialHandle(object obj)
    {
        var msg = (bool)obj;
        tutHandCenter.SetActive(msg);
        LockCenter.SetActive(msg);
        this.PostEvent((int)EventID.OnLockCamera, msg);
    }

    private void OnShowUIMoveHandle(object obj)
    {
        var msg = (bool)obj;
        if (!PlayerPrefSave.IsTutorial)
            return;
        switch (PlayerPrefSave.stepTutorial)
        {
            case 1://mua chuồng
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 3://click land
                        tutHandClickOk.SetActive(msg);
                        break;
                }
                break;
            case 4://mua nhà máy
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                   
                    case 4://click land
                        tutHandClickOk.SetActive(msg);
                        break;
                }
                break;
            case 6://mua ô đất
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    
                    case 3://click land
                        tutHandClickOk.SetActive(msg);
                        break;
                }
                break;
        }
    }

    private void OnLevelUpHandle(object obj)
    {
        LockCenter.SetActive(false);
    }
   
    private void OnLoadTutorialHandle(object obj)
    {
        if (!Util.isPlayMinigame)
            return;
        lockScreen.enabled = false;
        LockCenter.SetActive(false);
        LockDragSuggestion.SetActive(false);
        tutHandCenter.SetActive(false);
        tutHandDragSuggestion.SetActive(false);
        tutHandClickOk.SetActive(false);
        tutHandShopDrag1.SetActive(false);
        tutHandShopDrag2.SetActive(false);
        tutHandBtnShop.SetActive(false);


        tutHandOrder.SetActive(false);
        tutHandSendOrder.SetActive(false);

        tutHandShopTabAnimal.SetActive(false);
        tutHandSpeedUp.SetActive(false);
        lockSpeedUp.SetActive(false);
        tutHandShopTabFactory.SetActive(false);

        buttonHideUiCoundown.interactable = true;

        if (PlayerPrefSave.IsTutorial)
        {
            lockScreen.enabled = true;
            Invoke("LoadTutorial", .5f);
        }
    }

    private void GameStateManager_OnStateChanged(GameState current, GameState last, object data)
    {
        if (current == GameState.Play)
        {
            lockScreen.enabled = false;
            if (PlayerPrefSave.IsTutorial)
            {
                lockScreen.enabled = true;
                Invoke("LoadTutorial", 3f);
            }
        }
    }

    void LoadTutorial()
    {
        if (!PlayerPrefSave.IsTutorial)
            return;
            //show
            isTutorial = PlayerPrefSave.IsTutorial;
        step = PlayerPrefSave.stepTutorial;
        stepCurrent = PlayerPrefSave.stepTutorialCurrent;

        lockScreen.enabled = false;
        buttonHideUiSub.interactable = true;
        switch (PlayerPrefSave.stepTutorial)
        {
            case 0://Thu hoạch - trồng cây
                this.PostEvent((int)EventID.OnViewCamTutorial);
                LockCenter.SetActive(true);
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://click land
                        tutHandCenter.SetActive(true);
                        break;
                    case 1://hartvesst
                        tutHandDragSuggestion.SetActive(true);
                        buttonHideUiSub.interactable = false;
                        break;
                    case 2://hide
                        break;
                    case 3://click land
                        tutHandCenter.SetActive(true);
                        break;
                    case 4://drag crop
                        tutHandDragSuggestion.SetActive(true);
                        buttonHideUiSub.interactable = false;
                        break;
                    case 5://hide
                        break;
                    case 6://next
                        LockCenter.SetActive(false);
                        lockScreen.enabled = false;
                        if (PlayerPrefSave.Level >= 2)
                            return;
                        int exp = DataManager.LevelAsset.NextDataLevel.exp - PlayerPrefSave.ExpLevel;
                        CoinManager.AddExp(exp);
                        break;
                }
                break;
            case 1://mua chuồng
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://click shop
                        this.PostEvent((int)EventID.OnViewCamTutorial);
                        tutHandBtnShop.SetActive(true);
                        break;
                    case 1://drag crop
                        this.PostEvent((int)EventID.OnShowShop, TypeShop.Farms);
                        tutHandShopDrag2.SetActive(true);
                        break;
                    case 2://hide
                        break;
                    case 3://click land
                        tutHandClickOk.SetActive(true);
                        break;
                    case 4://next
                        PlayerPrefSave.stepTutorial = 2;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
                break;
            case 2://Mua gà
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://click shop
                        this.PostEvent((int)EventID.OnViewCamTutorial);
                        tutHandBtnShop.SetActive(true);
                        break;
                    case 1://click tap
                        tutHandShopTabAnimal.SetActive(true);
                        break;
                    case 2://drag crop
                        tutHandShopDrag1.SetActive(true);
                        break;
                    case 3://hide
                        break;
                    case 4://hide
                        PlayerPrefSave.stepTutorial = 3;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
                break;
            case 3://cho ăn
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://click chuồng
                        this.PostEvent((int)EventID.OnViewCamTutorial);
                        LockCenter.SetActive(true);
                        tutHandCenter.SetActive(true);
                        break;
                    case 1://drag food
                        LockDragSuggestion.SetActive(true);
                        tutHandDragSuggestion.SetActive(true);
                        buttonHideUiSub.interactable = false;
                        break;
                    case 2:
                        break;
                    case 3://click chuồng
                        this.PostEvent((int)EventID.OnViewCamTutorial);
                        LockCenter.SetActive(true);
                        tutHandCenter.SetActive(true);
                        CoinManager.AddDiamond(5);
                        break;
                    case 4://speed up
                        tutHandSpeedUp.SetActive(true);
                        buttonHideUiCoundown.interactable = false;
                        lockSpeedUp.SetActive(true);
                        break;
                    case 5://click chuồng
                        this.PostEvent((int)EventID.OnViewCamTutorial);
                        LockCenter.SetActive(true);
                        tutHandCenter.SetActive(true);
                        break;
                    case 6://drag thu hoạch
                        LockDragSuggestion.SetActive(true);
                        tutHandDragSuggestion.SetActive(true);
                        buttonHideUiSub.interactable = false;
                        break;
                    case 7:// crop
                        PlayerPrefSave.stepTutorial = 4;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
                break;
            case 4://mua nhà máy
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://click shop
                        this.PostEvent((int)EventID.OnViewCamTutorial);
                        tutHandBtnShop.SetActive(true);
                        break;
                    case 1://click tap
                        tutHandShopTabFactory.SetActive(true);
                        break;
                    case 2://drag crop
                        tutHandShopDrag2.SetActive(true);
                        break;
                    case 3://hide
                        break;
                    case 4://click land
                        tutHandClickOk.SetActive(true);
                        break;
                    case 5://next
                        PlayerPrefSave.stepTutorial = 5;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
                break;
            case 5://sx thức ăn
                this.PostEvent((int)EventID.OnViewCamTutorial);
                LockCenter.SetActive(true);
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://click shop
                        DataManager.ProductAsset.GetProductByName("Corn").total += 2;
                        tutHandCenter.SetActive(true);
                        LockCenter.SetActive(true);
                        break;
                    case 1://drag crop
                        buttonHideUiSub.interactable = false;
                        LockDragSuggestion.SetActive(true);
                        tutHandDragSuggestion.SetActive(true);
                        break;
                    case 2://hide
                        this.PostEvent((int)EventID.OnHideToggleSuggestion);
                        PlayerPrefSave.stepTutorial = 6;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
                break;
            case 6://mua ô đất
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://click shop
                        this.PostEvent((int)EventID.OnViewCamTutorial);
                        tutHandBtnShop.SetActive(true);
                        break;
                    case 1://drag crop
                        this.PostEvent((int)EventID.OnShowShop, TypeShop.Farms);
                        tutHandShopDrag1.SetActive(true);
                        break;
                    case 2://hide
                        break;
                    case 3://click land
                        tutHandClickOk.SetActive(true);
                        break;
                    case 4://next
                        PlayerPrefSave.stepTutorial = 7;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
                break;
            case 7://sx bánh
                this.PostEvent((int)EventID.OnViewCamTutorial);
                
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://click shop
                        DataManager.ProductAsset.GetProductByName("Wheat").total += 3;
                        LockCenter.SetActive(true);
                        tutHandCenter.SetActive(true);
                        break;
                    case 1://drag crop
                        buttonHideUiSub.interactable = false;
                        tutHandDragSuggestion.SetActive(true);
                        LockDragSuggestion.SetActive(true);
                        break;
                    case 2://hide
                        break;
                    case 3://hide
                        this.PostEvent((int)EventID.OnHideToggleSuggestion);
                        PlayerPrefSave.stepTutorial = 8;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
                break;
            case 8://pha map
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://
                        this.PostEvent((int)EventID.OnViewCamTutorial);
                        LockCenter.SetActive(true);
                        tutHandCenter.SetActive(true);
                        break;
                    case 1://drag crop
                        buttonHideUiSub.interactable = false;
                        tutHandDragSuggestion.SetActive(true);
                        LockDragSuggestion.SetActive(true);
                        break;
                    case 2://hide
                        break; 
                    case 3://hide
                        PlayerPrefSave.stepTutorial = 9;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
                break;
            case 9://Chuyển đơn hàng
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 0://click map
                        LockCenter.SetActive(true);
                        this.PostEvent((int)EventID.OnViewCamTutorial);
                        tutHandCenter.SetActive(true);
                        break;
                    case 1://click order
                        tutHandOrder.SetActive(true);
                        break;
                    case 2://send
                        tutHandSendOrder.SetActive(true);
                        break;
                    case 3://hide
                        PlayerPrefSave.stepTutorial = 10;
                        PlayerPrefSave.stepTutorialCurrent = 0;
                        PlayerPrefSave.IsTutorial = false;
                        this.PostEvent((int)EventID.OnLockCamera, false);

                        AnalyticsManager.LogEvent("complete_tutorial", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay } });
                        break;
                    
                }
                break;
        }
    }
}
