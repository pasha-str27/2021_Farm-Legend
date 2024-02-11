using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class UIMainScreen : MonoBehaviour
{
    public static UIMainScreen Instance;
    public UIAnimStatus Status => anim.Status;
    private UIAnimation anim;
    [SerializeField] Text txtLevel;
    [SerializeField] Text txtExpLevel;
    [SerializeField] Image fillExp;
    [Header("ui btn location")]
    [SerializeField] GameObject btnHarbor;
    [SerializeField] GameObject iconHarbor;
    [SerializeField] GameObject iconHome;
    [SerializeField] float targetY;
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject btnLevelup;
    void Awake()
    {
        Instance = this;
        anim = GetComponent<UIAnimation>();
        GameStateManager.OnStateChanged += GameStateManager_OnStateChanged;
        btnHarbor.SetActive(false);
    }
    private void Start()
    {
        btnLevelup.SetActive(GameUIManager.IsTest);
    }
    private void OnEnable()
    {
       
        this.RegisterListener((int)EventID.OnLevelUp, OnLevelUpHanlde);
        this.RegisterListener((int)EventID.OnUpdateExp, OnUpdateExpHanlde);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLevelUp, OnLevelUpHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUpdateExp, OnUpdateExpHanlde);
    }

    private void OnUpdateExpHanlde(object obj)
    {
        if (PlayerPrefSave.ExpLevel >= DataManager.LevelAsset.NextDataLevel.exp)
        {
            PlayerPrefSave.ExpLevel -= DataManager.LevelAsset.NextDataLevel.exp;
            PlayerPrefSave.Level++;
            this.PostEvent((int)EventID.OnLevelUp);
        }
        LoadUiLevel();
    }

    private void OnLevelUpHanlde(object obj)
    {
        LoadUiLevel();
        btnHarbor.SetActive(PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockOrderHarbor);
    }

    private void GameStateManager_OnStateChanged(GameState current, GameState last, object data)
    {
        if (current == GameState.Play)
        {
            LoadUiLevel();
            DataManager.ProductAsset.LoadCellAllProduct();
            MusicManager.Play(null);
            btnHarbor.SetActive(PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockOrderHarbor);
        }
    }
    private void Update()
    {
        if (btnHarbor.activeInHierarchy)
        {
            if (mainCamera.transform.position.y > targetY)
            {
                iconHarbor.SetActive(true);
                iconHome.SetActive(false);
            }
            else
            {
                iconHarbor.SetActive(false);
                iconHome.SetActive(true);
            }
        }
    }
    public void Show(TweenCallback onStart = null, TweenCallback onCompleted = null)
    {
        //start
        anim.Show(onStart, () =>
        {

            onCompleted?.Invoke();
            //AdsManager.ShowInterstitial((s) =>
            //{
            //    UIToast.Hide();
            //}, "ShowInterstitial", "Main scene");
            //MobileFullVideo.instance.ShowFullNormal();
        });

       
    }

    public void Hide()
    {
        anim.Hide();
    }

    void LoadUiLevel()
    {
        txtLevel.text = PlayerPrefSave.Level + "";
        //Debug.Log("=> exp = "+ PlayerPrefSave.ExpLevel);
        txtExpLevel.text = PlayerPrefSave.ExpLevel + "/" + DataManager.LevelAsset.NextDataLevel.exp;
        fillExp.DOFillAmount((float)CoinManager.totalExp / DataManager.LevelAsset.NextDataLevel.exp, .2f);
        DataManager.ProductAsset.LoadItemUnlockProduct();

    }
    #region button ui main

    public void BtnAdsCoinMain()
    {
        //int currentReward = 0;
        //SoundManager.Play("sfxClickRewardVideo");
        //AdsManager.ShowVideoReward((s) =>
        //{
        //    if (s == AdEvent.Success)
        //    {
        //        CoinManager.AddCoin(currentReward, transform);
        //        AdsManager.ShowNotice(s);
        //    }
        //    else
        //    {
        //        AdsManager.ShowNotice(s);
        //    }
        //}, $"ads_coin_by_ads", $"currentReward {currentReward}");
    }
    public void Btn_LevelUp_Click()
    {
        PlayerPrefSave.Level++;
        PlayerPrefSave.Coin += 1000;
        PlayerPrefSave.Diamond += 100;
        this.PostEvent((int)EventID.OnLevelUp);
    }

    public void Btn_Harbor_Click()
    {
        if (iconHome.activeInHierarchy)
            this.PostEvent((int)EventID.OnViewLocationHome);
        else
            this.PostEvent((int)EventID.OnViewLocationHarbor);
    }
    #endregion
}
