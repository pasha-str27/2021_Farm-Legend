using UnityEngine;

public class PlayerPrefSave
{
    public static bool IsFirtOpen
    {
        get { return PlayerPrefs.GetInt("IsFirtOpen") == 0; }
        set { PlayerPrefs.SetInt("IsFirtOpen", value == false ? 1 : 0); }
    }

    public static int Coin
    {
        get { return PlayerPrefs.GetInt("Coin", 0); }
        set { PlayerPrefs.SetInt("Coin", value >= 2147000000 ? 2147000000 : value); }
    }
    public static int Diamond
    {
        get { return PlayerPrefs.GetInt("Diamond", 0); }
        set { PlayerPrefs.SetInt("Diamond", value); }
    }
    public static int Level
    {
        get { return PlayerPrefs.GetInt("LevelGame", 1); }
        set { PlayerPrefs.SetInt("LevelGame", value); }
    }
    public static int ExpLevel
    {
        get { return PlayerPrefs.GetInt("ExpLevel", 0); }
        set { PlayerPrefs.SetInt("ExpLevel", value >= 2147000000 ? 2147000000 : value); }
    }
    public static int stepTutorial
    {
        get { return PlayerPrefs.GetInt("stepTutorial", 0); }
        set { PlayerPrefs.SetInt("stepTutorial", value); }
    }
    public static int stepTutorialCurrent
    {
        get { return PlayerPrefs.GetInt("stepTutorialCurrent", 0); }
        set { PlayerPrefs.SetInt("stepTutorialCurrent", value); }
    }
    public static bool IsTutorial
    {
        get { return PlayerPrefs.GetInt("IsTutorial") == 0; }
        set { PlayerPrefs.SetInt("IsTutorial", value == false ? 1 : 0); }
    }
    public static int GetTimeGrowing(int idLand)
    {
        return PlayerPrefs.GetInt("TimeGrowing:" + idLand);
    }

    public static void SetTimeGrowing(int idLand, int value)
    {
        PlayerPrefs.SetInt("TimeGrowing:" + idLand, value);
    }

    public static string GetSeed(int idLand)
    {
        return PlayerPrefs.GetString("Seed:" + idLand, "");
    }

    public static void SetSeed(int idLand, string value)
    {
        PlayerPrefs.SetString("Seed:" + idLand, value);
    }

    public static int GetStorageSeed(int idSeed)
    {
        return PlayerPrefs.GetInt("StorageSeed:" + idSeed);
    }

    public static void SetStorageSeed(int idSeed, int value)
    {
        PlayerPrefs.SetInt("StorageSeed:" + idSeed, value);
    }

    public static int GetMaxStore(ObjectMouseDown objectMouseDown)
    {
        return PlayerPrefs.GetInt("MaxStore" + objectMouseDown, objectMouseDown == ObjectMouseDown.Silo ? DataManager.GameConfig.BaseStoreSlilo : DataManager.GameConfig.BaseStoreStorage);
    }
    public static void SetMaxStore(ObjectMouseDown objectMouseDown, int vl)
    {
        PlayerPrefs.SetInt("MaxStore" + objectMouseDown, GetMaxStore(objectMouseDown) + vl);
    }
    public static void UpLevelStore(ObjectMouseDown objectMouseDown)
    {
        PlayerPrefs.SetInt("LevelStore" + objectMouseDown, GetLevelStore(objectMouseDown) + 1);
    }
    public static int GetLevelStore(ObjectMouseDown objectMouseDown)
    {
        return PlayerPrefs.GetInt("LevelStore" + objectMouseDown);
    }
    public static int IDChoose
    {
        get { return PlayerPrefs.GetInt("IDChoose", -1); }
        set { PlayerPrefs.SetInt("IDChoose", value); }
    }

    public static int GetTimeCage(int idCage)
    {
        return PlayerPrefs.GetInt("TimeCage:" + idCage);
    }

    public static void SetTimeCage(int idCage, int value)
    {
        PlayerPrefs.SetInt("TimeCage:" + idCage, value);
    }

    public static int GetTypeCage(int idCage)
    {
        return PlayerPrefs.GetInt("TypeCage:" + idCage);
    }

    public static void SetTypeCage(int idCage, int value)
    {
        PlayerPrefs.SetInt("TypeCage:" + idCage, value);
    }

    public static int TotalObject
    {
        get { return PlayerPrefs.GetInt("TotalObject"); }
        set { PlayerPrefs.SetInt("TotalObject", value); }
    }
    public static bool IsDeleteBuilding(int total)
    {
        return PlayerPrefs.GetInt("IsDeleteBuilding" + total, 0) == 1;
    }
    public static void DeleteBuilding(int total)
    {
        PlayerPrefs.SetInt("IsDeleteBuilding" + total, 1);
    }

    public static int GetTypeObject(int idBuilding)
    {
        return PlayerPrefs.GetInt("TypeObject" + idBuilding);
    }

    public static void SetTypeObject(int idBuilding, int value)
    {
        PlayerPrefs.SetInt("TypeObject" + idBuilding, value);
    }

    public static float GetPositionX(int idBuilding)
    {
        return PlayerPrefs.GetFloat("PositionX" + idBuilding);
    }

    public static void SetPositionX(int idBuilding, float value)
    {
        PlayerPrefs.SetFloat("PositionX" + idBuilding, value);
    }

    public static float GetPositionY(int idBuilding)
    {
        return PlayerPrefs.GetFloat("PositionY" + idBuilding);
    }

    public static void SetPositionY(int idBuilding, float value)
    {
        PlayerPrefs.SetFloat("PositionY" + idBuilding, value);
    }

    public static int GetAreaX(int idBuilding)
    {
        return PlayerPrefs.GetInt("AreaX" + idBuilding);
    }

    public static void SetAreaX(int idBuilding, int value)
    {
        PlayerPrefs.SetInt("AreaX" + idBuilding, value);
    }

    public static int GetAreaY(int idBuilding)
    {
        return PlayerPrefs.GetInt("AreaY" + idBuilding);
    }

    public static void SetAreaY(int idBuilding, int value)
    {
        PlayerPrefs.SetInt("AreaY" + idBuilding, value);
    }

    public static int GetProductFactory(int idFactory, int index)
    {
        return PlayerPrefs.GetInt("ProductFactory" + idFactory + "index" + index);
    }

    public static void SetProductFactory(int idFactory, int index, int value)
    {
        PlayerPrefs.SetInt("ProductFactory" + idFactory + "index" + index, value);
    }

    public static int GetLevelFactory(int idFactory)
    {
        return PlayerPrefs.GetInt("LevelFactory" + idFactory);
    }

    public static void SetLevelFactory(int idFactory, int value)
    {
        PlayerPrefs.SetInt("LevelFactory" + idFactory, value);
    }

    public static int LevelMap
    {
        get { return PlayerPrefs.GetInt("LevelMap", 1); }
        set { PlayerPrefs.SetInt("LevelMap", value); }
    }
    public static void UnLockMap(int id, int vl)
    {
        PlayerPrefs.SetInt("LockMap" + id, vl);
    }
    public static bool GetLockMap(int id)
    {
        return PlayerPrefs.GetInt("LockMap" + id) == 1;
    }

    public static string TimeOutGame
    {
        get { return PlayerPrefs.GetString("TimeOutGame"); }
        set { PlayerPrefs.SetString("TimeOutGame", value); }
    }

    public static int GetTotalAnimal(int idCage)
    {
        return PlayerPrefs.GetInt("TotalAnimal" + idCage);
    }

    public static void SetTotalAnimal(int idCage, int value)
    {
        PlayerPrefs.SetInt("TotalAnimal" + idCage, value);
    }

    public static int FirstGame
    {
        get { return PlayerPrefs.GetInt("FirstGame"); }
        set { PlayerPrefs.SetInt("FirstGame", value); }
    }

    public static float GetLocalScaleX(int idBuilding)
    {
        return PlayerPrefs.GetFloat("LocalScaleX" + idBuilding);
    }

    public static void SetLocalScaleX(int idBuilding, float value)
    {
        PlayerPrefs.SetFloat("LocalScaleX" + idBuilding, value);
    }

    public static int FirstAnimal
    {
        get { return PlayerPrefs.GetInt("FirstAnimal"); }
        set { PlayerPrefs.SetInt("FirstAnimal", value); }
    }

    public static int FirstFactory
    {
        get { return PlayerPrefs.GetInt("FirstFactory"); }
        set { PlayerPrefs.SetInt("FirstFactory", value); }
    }

    public static int GetAnimalEating(int idBuilding, int index)
    {
        return PlayerPrefs.GetInt("AnimalEating" + idBuilding + "Index" + index);
    }

    public static void SetAnimalEating(int idBuilding, int index, int value)
    {
        PlayerPrefs.SetInt("AnimalEating" + idBuilding + "Index" + index, value);
    }
    public static int bonusOrder
    {
        get { return PlayerPrefs.GetInt("bonusOrder", 0); }
        set { PlayerPrefs.SetInt("bonusOrder", value); }
    }

    public static int LevelMiniGame
    {
        get { return PlayerPrefs.GetInt("LevelMiniGame" + version_minigame, 0); }
        set { PlayerPrefs.SetInt("LevelMiniGame" + version_minigame, value); }
    }
    public static string version_minigame
    {
        get { return PlayerPrefs.GetString("version_minigame", "v3"); }
        set { PlayerPrefs.SetString("version_minigame", value); }
    }
    public static bool IsEnableMiniGame(string version)
    {
        return PlayerPrefs.GetInt("IsEnableMiniGame" + version) == 0;
    }
    public static void SetEnableMiniGame(string version, bool isEnable)
    {
        PlayerPrefs.SetInt("IsEnableMiniGame" + version, isEnable ? 0 : 1);
    }
    public static bool IsNewHarbor
    {
        get { return PlayerPrefs.GetInt("IsNewHarbor") == 0; }
        set { PlayerPrefs.SetInt("IsNewHarbor", value == false ? 1 : 0); }
    }

}
