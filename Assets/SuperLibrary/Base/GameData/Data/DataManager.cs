using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DataManager : MonoBehaviour
{
    #region STATIC
    public static GameConfig GameConfig => instance?.configAsset.gameConfig;
    public static UserData UserData
    {
        get { return instance?.userData; }
    }
    public static StageData CurrentStage
    {
        get => StagesAsset?.Current;
        set => StagesAsset.Current = value;
    }

    public static StagesAsset StagesAsset { get; private set; }
    //public static GameData gameData { get; private set; }
    private static DataManager instance { get; set; }
    public static ShopAsset ShopAsset => instance?.shopAsset;
    public static ProductAsset ProductAsset => instance?.productAsset;
    public static AchievementAsset AchievementAsset => instance?.achievementAsset;
    public static OrderAsset OrderAsset => instance?.orderAsset;
    public static OrderHarborAsset OrderHarborAsset => instance?.orderHarborAsset;
    public static FactoryAsset FactoryAsset => instance?.factoryAsset;
    public static LevelAsset LevelAsset { get; set; }
    public static MarketAsset MarketAsset => instance?.marketAsset;
    public static LanguegesAsset LanguegesAsset => instance?.languegesAsset;

    #endregion

    [Space(10)]
    [Header("Default Data")]
    [SerializeField]
    protected ConfigAsset configAsset = null;
    [SerializeField]
    protected StagesAsset stagesAsset = null;
    [SerializeField]
    protected ShopAsset shopAsset = null;
    [SerializeField] protected ProductAsset productAsset = null;
    [SerializeField] protected AchievementAsset achievementAsset = null;
    [SerializeField] protected OrderAsset orderAsset = null;
    [SerializeField] protected OrderHarborAsset orderHarborAsset = null;
    [SerializeField] protected FactoryAsset factoryAsset = null;
    [SerializeField] protected LevelAsset levelAsset = null;
    [SerializeField] protected MarketAsset marketAsset = null;
    [SerializeField] protected LanguegesAsset languegesAsset = null;
    [SerializeField] protected UserData userData = null;

    public static bool IsFirstTime = false;

    [Header("GameData auto SAVE LOAD")]
    [SerializeField]
    protected bool loadOnStart = true;
    [SerializeField]
    protected bool saveOnPause = true;
    [SerializeField]
    protected bool saveOnQuit = true;

    public delegate void LoadedDelegate(GameData gameData);
    public static event LoadedDelegate OnLoaded;

    #region BASE
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (loadOnStart)
            Load();
    }
    public static void AddLevel(DataLevel data)
    {
        instance.levelAsset.list.Add(data);
    }
    public static void Save(bool saveCloud = true)
    {
        //if (instance && gameData != null && gameData.user != null)
        if (instance)
        {
            //var time = DateTime.Now;
            //gameData.user.LastTimeUpdate = DateTime.Now;
            //gameData.stages = StagesAsset.stageSaveList;
            if (saveCloud)
            {
                instance.orderAsset.Save();
                instance.orderHarborAsset.Save();
            }

            //Debug.Log("ConvertData in " + (DateTime.Now - time).TotalMilliseconds + "ms");
            //FileExtend.SaveData<GameData>("GameData", gameData);
            //Debug.Log("SaveData in " + (DateTime.Now - time).TotalMilliseconds + "ms");

            if (saveCloud)
            {
                //Save cloud in here;
                Debug.Log("Save cloud is not implement!");
            }
        }
    }

    public static IEnumerator DoLoad()
    {
        if (instance)
        {
            Load();
            yield return null;
            //var elapsedTime = 0f;
            //if (gameData == null)
            //    Load();
            //else
            //    Debug.LogWarning("GameData not NULL");

            //while (gameData == null)
            //{
            //    if (elapsedTime < 5)
            //    {
            //        Debug.LogWarning("GameData load " + elapsedTime.ToString("0.0"));
            //        elapsedTime += Time.deltaTime;
            //        yield return null;
            //    }
            //}
        }
    }

    public static void Load()
    {
        var time = DateTime.Now;
        if (instance)
        {
            //Create default
            var tempData = new GameData();

            if (StagesAsset == null)
            {
                StagesAsset = ScriptableObject.CreateInstance("StagesAsset") as StagesAsset;
                foreach (var i in instance.stagesAsset.list)
                    StagesAsset.list.Add(i);
            }
            else
                Debug.Log("stageDatas is not NULL");

            if (LevelAsset == null)
            {
                LevelAsset = ScriptableObject.CreateInstance("LevelAsset") as LevelAsset;
                foreach (var i in instance.levelAsset.list)
                    LevelAsset.list.Add(i);
            }
            else
                Debug.Log("stageDatas is not NULL");

            instance.RessetAllAsset();

            UserData.timeStart = Util.timeNow;
        }
        else
        {
            throw new Exception("Data Manager instance is NULL. Maybe it hasn't been created.");
        }

        if (PlayerPrefSave.IsFirtOpen)
        {
            instance.SetBaseData();
        }
        instance.orderAsset.LoadOrder();
        //OnLoaded?.Invoke(gameData);
    }

    public static void Reset()
    {
        var path = FileExtend.FileNameToPath("GameData.gd");
        FileExtend.Delete(path);
        PlayerPrefs.DeleteAll();
        Debug.Log("Reset game data");
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause && !GameStateManager.isBusy && saveOnPause)
            Save(false);
    }

    private void OnApplicationQuit()
    {
        if (saveOnQuit)
            Save(true);
        PlayerPrefSave.IsFirtOpen = false;
        //Debug.Log("=>Data IsFirtOpen " + PlayerPrefSave.IsFirtOpen);
    }

    public void ResetAndUpdateData()
    {
        try
        {
            Reset();
            stagesAsset.ResetData();
            stagesAsset.UpdateCost();

            RessetAllAsset();

            Debug.Log("Reset and Update data to BUILD!!!");
        }
        catch (Exception ex)
        {
            Debug.LogError("Please update and save DATA before build!!!");
            Debug.LogException(ex);
        }
    }

    void RessetAllAsset()
    {
        shopAsset.ResetData();
        productAsset.ResetData();
        achievementAsset.ResetData();
        orderAsset.ResetData();
        orderHarborAsset.ResetData();
        factoryAsset.ResetData();
        marketAsset.ResetData();
        languegesAsset.ResetData();
    }
    #endregion

    public void SetBaseData()
    {
        if (!PlayerPrefSave.IsFirtOpen || PlayerPrefSave.Level>1)
            return;
        PlayerPrefSave.Coin = configAsset.gameConfig.CoinBase;
        PlayerPrefSave.Diamond = configAsset.gameConfig.DiamondBase;
        PlayerPrefSave.Level = 1;
        PlayerPrefs.SetInt("MaxStore" + ObjectMouseDown.Silo, configAsset.gameConfig.BaseStoreSlilo);
        PlayerPrefs.SetInt("MaxStore" + ObjectMouseDown.Storage, configAsset.gameConfig.BaseStoreStorage);
        productAsset.SetBaseMaterial("Saw", 3);
        productAsset.SetBaseMaterial("Shovel", 3);
        productAsset.SetBaseMaterial("Hammer", 3);
        productAsset.SetBaseMaterial("Rope", 3);
        productAsset.SetBaseMaterial("Axe", 3);
    }

    public static void TestMaterial()
    {
        List<ProductData> temp = instance.productAsset.list.Where(x => x.tabName == TabName.Material).ToList();
        for (int i = 0; i < temp.Count; i++)
        {
            temp[i].total += 3;
        }
    }

    public static void ChangeCountProduct()
    {
        try
        {
            if (instance)
                instance.PostEvent((int)EventID.OnLoadUiOrderMap);
        }
        catch { }
    }
}