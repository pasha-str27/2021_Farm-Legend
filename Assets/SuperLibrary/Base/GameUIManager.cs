using BitBenderGames;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : GameManagerBase<GameUIManager>
{
    public static GameUIManager Instance => instance;
    public static Canvas Canvas => instance.canvas;
    public static bool IsShowUiMove => instance.isShowUiMove;
    public static bool IsTest => instance.isTest;
    public static BuildMarketing BuildMarketing => instance.buildMarketing;
    [Header("=====BuildMarketing=====")]
    [SerializeField] bool isTest;
    [SerializeField] BuildMarketing buildMarketing = BuildMarketing.AllGame;
    [SerializeField] bool isEnable_MinigameTower;
    [SerializeField] bool isEnable_MinigameRescue;
    [SerializeField] bool isEnable_MinigameFarmRescue;
    [Header("======================")]
    [SerializeField] Canvas canvas;
    [SerializeField]
    private float waitTimeForLoadAd = 1;

    [SerializeField]
    private UIAnimation splashScreen = null;

    [SerializeField]
    private UIMainScreen mainScreen = null;
    public static UIMainScreen MainScreen => instance?.mainScreen;
    public bool IsFirtOpen
    {
        get { return isFirtOpen; }
        set { isFirtOpen = value; }

    }

    public static Font FontVietnamese => instance.fontVietnamese;
    public static Font FontEnglish => instance.fontEnglish;
    [SerializeField] Font fontVietnamese;
    [SerializeField] Font fontEnglish;

    [Header("Popup ui")]
    [SerializeField]
    private UiSetting popupSetting = null;
    [SerializeField]
    private UiShop uiShop = null;
    [SerializeField]
    private UISiloStorage uISiloStorage = null;
    [SerializeField]
    private UiUpgradeSiloStorage uIUpgradeSiloStorage = null;
    [SerializeField]
    private UiOrder uiOrder = null;
    [SerializeField]
    private UiHarbor uiHarbor = null;
    [SerializeField]
    private UIAchievement uIAchievement = null;
    [SerializeField]
    private UiMarket uiMarket = null;
    [SerializeField]
    private UiSuggestions uiSuggestions = null;
    [SerializeField]
    private UiExpand uiExpand = null;
    [SerializeField]
    private UiExpand uiGoldMine = null;
    [SerializeField]
    private UiCountDown uiCountDown = null;
    [SerializeField]
    private UiLevelUp uiLevelUp = null;
    [SerializeField]
    private UIMove uIMove = null;
    [SerializeField]
    private UiNotEnough uiNotEnough = null;
    [SerializeField]
    private UiGiftMain uiCoinGift = null;
    [SerializeField]
    private UiVideoReward uiVideoReward = null;
    [SerializeField]
    private UiExit uiExit = null;

    [Header("Minigame")]
    [SerializeField] Camera cameraMain;
    [SerializeField] public Camera cameraMinigame;
    [SerializeField] public Camera cameraMinigameInFarm;
    [SerializeField] UIAnimation uiMainScreen;
    [SerializeField] UIAnimation uiCoin;

    private DateTime startLoadTime = DateTime.Now;

    private GameConfig gameConfig => DataManager.GameConfig;
    private UserData user => DataManager.UserData;
    private float lastTimeShow { get; set; }
    bool isShowUiMove;
     bool isFirtOpen = false;
    protected override void Awake()
    {
        base.Awake();
        if (splashScreen)
            splashScreen.gameObject.SetActive(true);
        startLoadTime = DateTime.Now;
        //PlayerPrefSave.Level = 60;


#if UNITY_EDITOR
        DOTween.useSafeMode = false;
#endif
    }

    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnClickObject, OnClickObjectHanlde);
        this.RegisterListener((int)EventID.OnShowPopupUpgrade, OnShowPopupUpgradeHanlde);
        this.RegisterListener((int)EventID.OnLevelUp, OnLevelUpHanlde);
        this.RegisterListener((int)EventID.OnShowUIMove, OnShowUIMoveHanlde);
        this.RegisterListener((int)EventID.OnNotEnought, OnNotEnoughtHanlde);
        this.RegisterListener((int)EventID.OnShowVideoReward, OnShowVideoRewardHanlde);
        this.RegisterListener((int)EventID.OnAddProductMarket, OnAddProductMarketHanlde);
        this.RegisterListener((int)EventID.OnLoadProductSale, OnLoadProductSellHanlde);
        this.RegisterListener((int)EventID.OnShowExit, OnShowExitHanlde);
        this.RegisterListener((int)EventID.OnShowMinigameInFarm, OnShowMinigameInFarmHanlde);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickObject, OnClickObjectHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowPopupUpgrade, OnShowPopupUpgradeHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLevelUp, OnLevelUpHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowUIMove, OnShowUIMoveHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnNotEnought, OnNotEnoughtHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowVideoReward, OnShowVideoRewardHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnAddProductMarket, OnAddProductMarketHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnLoadProductSale, OnLoadProductSellHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowExit, OnShowExitHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowMinigameInFarm, OnShowMinigameInFarmHanlde);
    }

    private void OnShowMinigameInFarmHanlde(object obj)
    {
        var msg = (bool)obj;
        this.PostEvent((int)EventID.OnLockCamera, msg);
        cameraMain.enabled = !msg;
        cameraMinigame.enabled = false;
        cameraMinigameInFarm.enabled = msg;
    }

    private void OnShowExitHanlde(object obj)
    {
        uiExit.Show();
    }

    private void OnLoadProductSellHanlde(object obj)
    {
        HideAllPopup();
        uiMarket.Show(TabName.SaleMarket);
    }

    private void OnAddProductMarketHanlde(object obj)
    {
        HideAllPopup();
        var msg = (DataMarket)obj;
        uISiloStorage.Show(ObjectMouseDown.Silo, msg);
    }

    private void OnShowVideoRewardHanlde(object obj)
    {
        HideAllPopup();
        this.PostEvent((int)EventID.OnShowUIFactory, new MessagerUiFactory { isShow = false });

        uiVideoReward.Show();
    }
    void HideAllPopup()
    {
        uiNotEnough.Hide();
        uIMove.Hide();
        uiShop.Hide();
        uiExpand.Hide();
        uiSuggestions.Hide();
        uiCountDown.Hide();
        uiCoinGift.Hide();
        uiMarket.Hide();
        uiOrder.Hide();
        uiHarbor.Hide();
        uiVideoReward.Hide();
        uISiloStorage.Hide();
        uIAchievement.Hide();
    }
    private void OnNotEnoughtHanlde(object obj)
    {
        var msg = (MessagerUiNotEnought)obj;
        uiNotEnough.Show(msg);
    }

    private void OnShowUIMoveHanlde(object obj)
    {
        var msg = (bool)obj;
        if (msg)
            uIMove.Show();
        else uIMove.Hide();
        isShowUiMove = msg;
    }

    private void OnLevelUpHanlde(object obj)
    {
        if (!PlayerPrefSave.IsTutorial)
        {
            AdsManager.ShowFullNormal(() =>
            {
                uiLevelUp.Show();
            }, () =>
            {
                uiLevelUp.Show();
                AnalyticsManager.LogEvent("ShowFullNormal_fail", new Dictionary<string, object> {
            { "action", "LevelUp" }});
            });
        }
        else
            uiLevelUp.Show();
    }

    private void OnShowPopupUpgradeHanlde(object obj)
    {
        HideAllPopup();
        var msg = (ObjectMouseDown)obj;
        uIUpgradeSiloStorage.Show(msg);
    }

    private void OnClickObjectHanlde(object obj)
    {
        var msg = (MessageObject)obj;
        //Debug.Log("=> OnClickObjectHanlde" + msg.type);
        switch (msg.type)
        {
            case ObjectMouseDown.Silo:
            case ObjectMouseDown.Storage:
                if (!PlayerPrefSave.IsTutorial)
                    uISiloStorage.Show(msg.type);
                break;
            case ObjectMouseDown.MainHouse:
                //UIToast.Show("Comming soon", null, ToastType.Notification, 1.5f);
                if (PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockAchie)
                    uIAchievement.Show();
                else
                    UIToast.Show(DataManager.LanguegesAsset.GetName("Unlock level") + " " + DataManager.GameConfig.LevelUnlockAchie, null, ToastType.Notification, 1.5f);
                break;
            case ObjectMouseDown.Market:
                //UIToast.Show("Comming soon", null, ToastType.Notification, 1.5f);
                if (PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockMarket)
                    uiMarket.Show(TabName.BuyMarket);
                else
                    UIToast.Show(DataManager.LanguegesAsset.GetName("Unlock level") + " " + DataManager.GameConfig.LevelUnlockMarket, null, ToastType.Notification, 1.5f);
                break;
            case ObjectMouseDown.Orders:
                if (!PlayerPrefSave.IsTutorial || (PlayerPrefSave.stepTutorial == 9 && PlayerPrefSave.stepTutorialCurrent == 0))
                    uiOrder.Show();
                break;
            case ObjectMouseDown.Crops:
            case ObjectMouseDown.Garbage:
            case ObjectMouseDown.OldTree:
                if (msg.timeCount > 0)
                {
                    uiCountDown.Show(msg.name, msg.data, msg.timeCount);
                }
                else
                    uiSuggestions.Show(msg);
                this.PostEvent((int)EventID.OnZoomCamera, true);
                break;
            case ObjectMouseDown.MapLock:
                if (!PlayerPrefSave.IsTutorial)
                    uiExpand.Show(msg);
                break;
            case ObjectMouseDown.Cage:
                if (msg.timeCount > 0 && msg.isRaising)
                {
                    uiCountDown.Show(msg.nameKey, msg.data, msg.timeCount, msg.nameCage);
                    uiSuggestions.Show(msg);
                }
                else if (msg.timeCount > 0 && !msg.isRaising)
                {
                    uiCountDown.Show(msg.nameKey, msg.data, msg.timeCount, msg.nameCage);
                }
                else
                {
                    uiSuggestions.Show(msg);
                }
                this.PostEvent((int)EventID.OnZoomCamera, true);
                break;
            case ObjectMouseDown.Factory:
                this.PostEvent((int)EventID.OnZoomCamera, true);
                if (!msg.isHarvest)
                {
                    uiSuggestions.Show(msg);
                    this.PostEvent((int)EventID.OnShowUIFactory, new MessagerUiFactory { time = msg.timeCount, isShow = true, idFactory = msg.idFactory });
                }
                else
                {
                    this.PostEvent((int)EventID.OnThuHoachFactory, msg.idFactory);
                }
                break;
            case ObjectMouseDown.Building:
                break;
            case ObjectMouseDown.Gift:
                if (!PlayerPrefSave.IsTutorial)
                    uiCoinGift.Show();
                break;
            case ObjectMouseDown.Harbor:
                if (PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockOrderHarbor)
                    uiHarbor.Show();
                else
                    UIToast.Show(DataManager.LanguegesAsset.GetName("Unlock level") + " " + DataManager.GameConfig.LevelUnlockOrderHarbor, null, ToastType.Notification, 1.5f);
                break;
            case ObjectMouseDown.GoldMine:
                if (PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockGoldMine)
                {
                    if (msg.timeCount > 0)
                    {
                        uiCountDown.Show(msg.name, msg.nameKey, msg.timeCount);
                    }
                    else
                    {
                        if (msg.isHarvest)
                            this.PostEvent((int)EventID.OnHarvestGoldMine);
                        else
                            uiGoldMine.Show(msg);
                    }
                }
                else
                    UIToast.Show(DataManager.LanguegesAsset.GetName("Unlock level") + " " + DataManager.GameConfig.LevelUnlockGoldMine, null, ToastType.Notification, 1.5f);
                break;
        }
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LoadGameData());
    }

    public IEnumerator LoadGameData()
    {
        yield return DataManager.DoLoad();
        //UIToast.ShowLoading(Util.isVietnamese?"Đang tải... vui lòng đợi!": "Loading... please wait!");

        while (user == null)
        {
            DebugMode.Log("Load game data...");
            yield return null;
        }

        SoundManager.LoadAllSounds();

#if USE_FIREBASE_REMOTE
        var remote = new GameConfig();
        FirebaseHelper.defaultRemoteConfig = new Dictionary<string, object>
        {
            { "timePlayToShowAds", gameConfig.timePlayToShowAds},
            { "timePlayReduceToShowAds", gameConfig.timePlayReduceToShowAds},
            { "suggestUpdateVersion", gameConfig.suggestUpdateVersion},

            { "goldByAds", gameConfig.goldByAds},
            { "goldScale", gameConfig.goldScale},
            { "goldScaleByAds", gameConfig.goldScaleByAds},

            { "maxLevelDifficult", gameConfig.maxLevelDifficult},
            { "winStreakStep", gameConfig.winStreakStep},
            { "winStreakScale", gameConfig.winStreakScale},
            { "loseStreakStep", gameConfig.loseStreakStep},
            { "loseStreakScale", gameConfig.loseStreakScale},

            { "adInterNotToReward", gameConfig.adInterNotToReward},
            { "adRewardNotToInter", gameConfig.adRewardNotToInter},
            { "adInterViewToReward", gameConfig.adInterViewToReward},
        };
#endif

#if USE_FIREBASE
        yield return FirebaseHelper.DoCheckStatus(null, true);
#endif

#if USE_FIREBASE_REMOTE
        yield return FirebaseHelper.DoFetchRemoteData((status) =>
        {
            if (status == FirebaseStatus.Completed && userData != null && gameConfig != null)
            {
                gameConfig.timePlayToShowAds = FirebaseHelper.RemoteGetValueFloat("timePlayToShowAds", gameConfig.timePlayToShowAds);
                gameConfig.timePlayReduceToShowAds = FirebaseHelper.RemoteGetValueFloat("timePlayReduceToShowAds", gameConfig.timePlayReduceToShowAds);
                gameConfig.suggestUpdateVersion = FirebaseHelper.RemoteGetValueInt("suggestUpdateVersion", gameConfig.suggestUpdateVersion);

                gameConfig.easyStage = FirebaseHelper.RemoteGetValueInt("easyStage", gameConfig.easyStage);
                gameConfig.randomStageStart = FirebaseHelper.RemoteGetValueInt("randomStageStart", gameConfig.randomStageStart);

                gameConfig.goldByAds = FirebaseHelper.RemoteGetValueInt("goldByAds", gameConfig.goldByAds);
                gameConfig.goldScale = FirebaseHelper.RemoteGetValueInt("goldScale", gameConfig.goldScale);
                gameConfig.goldScaleByAds = FirebaseHelper.RemoteGetValueInt("goldScaleByAds", gameConfig.goldScaleByAds);

                gameConfig.maxLevelDifficult = FirebaseHelper.RemoteGetValueInt("maxLevelDifficult", gameConfig.maxLevelDifficult);
                gameConfig.winStreakStep = FirebaseHelper.RemoteGetValueFloat("winStreakStep", gameConfig.winStreakStep);
                gameConfig.winStreakScale = FirebaseHelper.RemoteGetValueFloat("winStreakScale", gameConfig.winStreakScale);
                gameConfig.loseStreakStep = FirebaseHelper.RemoteGetValueFloat("loseStreakStep", gameConfig.loseStreakStep);
                gameConfig.loseStreakScale = FirebaseHelper.RemoteGetValueFloat("loseStreakScale", gameConfig.loseStreakScale);

                gameConfig.adInterNotToReward = FirebaseHelper.RemoteGetValueInt("autoInterToReward", gameConfig.adInterNotToReward);
                gameConfig.adRewardNotToInter = FirebaseHelper.RemoteGetValueInt("adRewardNotToInter", gameConfig.adRewardNotToInter);
                gameConfig.adInterViewToReward = FirebaseHelper.RemoteGetValueInt("adInterViewToReward", gameConfig.adInterViewToReward);
            }

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                AnalyticsManager.LogEvent("DoFetchRemoteData_" + status.ToString());
                Debug.Log("DoFetchRemoteData_" + status.ToString());
                DebugMode.Log(JsonUtility.ToJson(gameConfig));
            });
        });
#endif

        if (user.VersionInstall == 0)
        {
            user.VersionInstall = UIManager.BundleVersion;
#if USE_FIREBASE
            FirebaseHelper.SetUser("Type", "New");
            AnalyticsManager.LogEvent("User_New");
#endif
        }
        else if (user.VersionInstall != UIManager.BundleVersion)
        {
#if USE_FIREBASE
            FirebaseHelper.SetUser("Type", "Update");
            AnalyticsManager.LogEvent("User_Update");
#endif
        }
        user.VersionCurrent = UIManager.BundleVersion;

        while ((int)(DateTime.Now - startLoadTime).TotalSeconds < waitTimeForLoadAd)
        {
            yield return null;
        }
        GameStateManager.LoadMain(null);

        PlayerPrefSave.SetEnableMiniGame("v1", isEnable_MinigameTower);
        PlayerPrefSave.SetEnableMiniGame("v2", isEnable_MinigameRescue);
        PlayerPrefSave.SetEnableMiniGame("v3", isEnable_MinigameFarmRescue);

        if (isEnable_MinigameFarmRescue)
            PlayerPrefSave.version_minigame = "v3";
        else if (isEnable_MinigameRescue)
            PlayerPrefSave.version_minigame = "v2";
        else if (isEnable_MinigameTower)
            PlayerPrefSave.version_minigame = "v1";
         
        //if (isEnable_MinigameTower)
        //    PlayerPrefSave.version_minigame = "v1";
        //else if (isEnable_MinigameRescue)
        //    PlayerPrefSave.version_minigame = "v2";zzz z
        //else if (isEnable_MinigameFarmRescue)
        //    PlayerPrefSave.version_minigame = "v3";

        switch (buildMarketing)
        {
            case BuildMarketing.AllGame:
                if (PlayerPrefSave.IsFirtOpen && PlayerPrefSave.LevelMiniGame == 0)
                {
                    GameStateManager.Idle(null);
                }
                else
                {
                    Util.isPlayMinigame = true;
                    GameStateManager.LoadGame(null);
                }
                break;
            case BuildMarketing.Farm:
                Util.isPlayMinigame = true;
                GameStateManager.LoadGame(null);
                break;
            case BuildMarketing.MiniGameTower:
                PlayerPrefSave.version_minigame = "v1";
                GameStateManager.Idle(null);
                break;
            case BuildMarketing.MiniGameRescue:
                PlayerPrefSave.version_minigame = "v2";
                GameStateManager.Idle(null);
                break;
            case BuildMarketing.MiniGameFarmRescue:
                PlayerPrefSave.version_minigame = "v3";
                GameStateManager.Idle(null);
                break;
        }
        
        yield return new WaitForSeconds(.5f);
        splashScreen?.Hide();

        int loadGameIn = (int)(DateTime.Now - startLoadTime).TotalSeconds;
        Debug.Log("loadGameIn: " + loadGameIn + "s");

        AnalyticsManager.LogEvent("game_start", AnalyticsManager.logUser);
    }

    public void ForeUpdate()
    {
        string title = "New version avaiable!";
        string body = "We are trying to improve the game quality by updating it regularly.\nPlease update new version for the best experience!";
        PopupMes.Show(title, body,
            "Update", () =>
            {
                //if (!string.IsNullOrEmpty(UIManager.shareUrl))
                //    Application.OpenURL(UIManager.shareUrl);
            },
            "Later", () =>
            {
                splashScreen?.Hide();
                uiShop.Show();
                mainScreen.Show(null, () =>
                {
                    GameStateManager.Idle(null);
                });
            });
    }

    protected override void LoadMain(object data)
    {
        base.LoadMain(data);
        //DataManager.CurrentStage = DataManager.StagesAsset.GetRandom();
        //LoadGameContent.PrepairDataToPlay(DataManager.CurrentStage);
    }

    public override void IdleGame(object data)
    {
        StartCoroutine(LoadIdleScreen());
        Action callback = () =>
        {
            MusicManager.Pause();
            uiShop.Show();
            mainScreen.Show(() =>
            {
            }, () =>
            {
                UILoadGame.Hide();
            });

            uiMainScreen.Hide();
            uiShop.Hide();
            uiCoin.Hide();

            cameraMain.enabled = false;
            cameraMinigame.enabled = true;
            HideAllPopup();
            this.PostEvent((int)EventID.OnCompleteLoadMain);
        };

        UILoadGame.Init(true, null);
        StartCoroutine(WaitForLoading(callback, 1f));

        this.PostEvent((int)EventID.OnLockCamera, true);
        //if (GameStateManager.LastState == GameState.Idle
        //    || GameStateManager.LastState == GameState.GameOver
        //    || GameStateManager.LastState == GameState.Next)
        //{
        //    UILoadGame.Init(true, null);
        //    StartCoroutine(WaitForLoading(callback, 0.5f));
        //}
        //else
        //{
        //    callback.Invoke();
        //}
    }
    IEnumerator LoadIdleScreen()
    {
        yield return SceneHelper.DoLoadIdleScene();
        while (!SceneHelper.isLoaded)
            yield return null; 
    }

    protected override void GoToShop(object data)
    {
        base.GoToShop(data);
        mainScreen.Hide();

    }

    public override void LoadGame(object data)
    {
        //Time.timeScale = 1;
        cameraMain.enabled = true;
        cameraMinigame.enabled = false;
        cameraMinigameInFarm.enabled = false;
        DataManager.CurrentStage = DataManager.StagesAsset.GetRandom();
        LoadGameContent.PrepairDataToPlay(DataManager.CurrentStage);
        MusicManager.UnPause();
        uiMainScreen.Show();
        uiShop.Show();
        uiCoin.Show();
        this.PostEvent((int)EventID.OnLockCamera, false);
       
    }

    public override void InitGame(object data)
    {
        foreach (var i in UIManager.listPopup)
            i.Hide();
        DOTween.Kill(this);
    }

    IEnumerator WaitForLoading(Action onComplete, float time = 0)
    {
        if (time > 0)
            yield return new WaitForSeconds(time);

        while (UILoadGame.currentProcess < 1)
        {
            UILoadGame.Process();
            yield return null;
        }
        UILoadGame.Hide();
        onComplete?.Invoke();
    }

    public override void PlayGame(object data)
    {
        //MusicManager.UnPause();
        //DataManager.OrderHarborAsset.LoadOrder();
    }

    public override void PauseGame(object data)
    {
        MusicManager.Pause();
    }

    protected override void GameOver(object data)
    {
    }

    protected override void CompleteGame(object data)
    {
    }

    protected override void ReadyGame(object data)
    {
        StartCoroutine(WaitForLoading(() =>
        {
            StartCoroutine(WaitToAutoPlay());
        }));
    }

    public override void ResumeGame(object data)
    {
        //MusicManager.UnPause();
    }

    public override void RestartGame(object data)
    {
        GameStateManager.Idle(null);
    }

    public override void NextGame(object data)
    {
        GameStateManager.Idle(null);
    }

    protected override void WaitingGameOver(object data)
    {
        MusicManager.Stop(null);

        DOVirtual.Float(1.0f, 0.25f, 0.5f, (t) => Time.timeScale = t).SetDelay(0.25f)
            .OnComplete(() => Time.timeScale = 1);

        float timeWaitDie = 1f;
        SoundManager.Play("sfx_crowd_oohs_0" + UnityEngine.Random.Range(1, 4));

        DOVirtual.DelayedCall(timeWaitDie, () =>
        {
            if (GameStateManager.CurrentState == GameState.WaitGameOver)
                GameStateManager.GameOver(data);
        }).SetUpdate(false).SetId(this);
    }

    protected override void WaitingGameComplete(object data)
    {
        SoundManager.Play("sfx_crowd_applause_0" + UnityEngine.Random.Range(1, 4));
        MusicManager.Stop(null);

    }

    protected override void RebornContinueGame(object data)
    {
        GameStateManager.Init(null);
        StartCoroutine(WaitToAutoPlay());
    }

    protected override void RebornCheckPointGame(object data)
    {
        GameStateManager.Init(null);
        StartCoroutine(WaitToAutoPlay());
    }
    IEnumerator WaitToAutoPlay()
    {
        var wait01s = new WaitForSeconds(0.1f);
        var wait05s = new WaitForSeconds(0.5f);
        while (GameStateManager.CurrentState != GameState.Ready)
            yield return wait01s;
        yield return wait05s;
        GameStateManager.Play(null);
    }
    int tempCount = 0;
    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            tempCount++;
            if (tempCount > 1)
            {
                this.PostEvent((int)EventID.OnShowExit);
                tempCount = 0;
            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.U))
        {
            PlayerPrefSave.Level++;
            this.PostEvent((int)EventID.OnLevelUp);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            this.PostEvent((int)EventID.OnShowShop, TypeShop.Animals);
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            DataManager.TestMaterial();
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            GameStateManager.LoadGame(null);
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            GameStateManager.Idle(null);
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            DataManager.OrderHarborAsset.CreatOrder();
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            this.PostEvent((int)EventID.OnShowMinigameInFarm,true);
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            this.PostEvent((int)EventID.OnShowMinigameInFarm, false);
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            //DataManager.LevelAsset.Add(new DataLevel());
            DataManager.AddLevel(new DataLevel());
        }
#endif
    }
}

public enum BuildMarketing
{
    AllGame, Farm, MiniGameTower, MiniGameRescue, MiniGameFarmRescue
}