using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;

[CreateAssetMenu(fileName = "ShopAsset", menuName = "DataAsset/ShopAsset")]
public class ShopAsset : BaseAsset<ShopData>
{
    [SerializeField] string tempNull = "";
    [Header("Data json")]
    [SerializeField] string data_json;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<DataShop> dataJson;

    [Header("Assets")]
    [SerializeField] Sprite[] spIcon;

    [Header("Prefabs")]
    [SerializeField] GameObject[] prefabsItem;

    List<ShopData> tempList = new List<ShopData>();
    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        dataJson = JsonUtility.FromJson<ListDataShop>(data_json).dataShops;
        tempNull = "";
        AddData();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    public override void ResetData()
    {
        base.ResetData();
        AddAllData();
    }

    private void AddData()
    {
        for (int i = 0; i < dataJson.Count; i++)
        {
            try
            {
                ShopData stage = new ShopData();
                stage.name = dataJson[i].Name;
                stage.nameV = dataJson[i].NameV;
                stage.id = i + "";
                stage.index = list.Count;
                stage.isUnlocked = false;
                stage.unlockType = UnlockType.Gold;
                stage.isSelected = false;

                stage.quantityBuild = dataJson[i].QuantityBuild;
                stage.exp = dataJson[i].Exp;
                stage.levelUnlock = dataJson[i].Level1;
                stage.levelUnlock2 = dataJson[i].Level2;
                stage.typeShop = (TypeShop)Enum.Parse(typeof(TypeShop), dataJson[i].TypeShop);
                stage.spIcon = spIcon.FirstOrDefault(x => x.name.Replace("_", " ").ToUpper().Equals(stage.name.ToUpper()));
                stage.price = dataJson[i].Price;
                stage.price2 = dataJson[i].Price2;
                stage.prefabs = prefabsItem.FirstOrDefault(x => x.name.ToUpper().Equals(stage.name.ToUpper()));

                if (stage.typeShop == TypeShop.Decorations)
                {
                    stage.levelUnlock2 = dataJson[i].Level1;
                    stage.price2 = dataJson[i].Price;
                }
                if (stage.typeShop == TypeShop.Animals)
                {
                    if (stage.name.Equals("Chicken"))
                    {
                        stage.description = "Henhouse";
                    }
                    else
                    {
                        stage.description = dataJson.FirstOrDefault(x => x.TypeShop.Equals("Farms") && x.Name.ToUpper().Contains(stage.name.ToUpper())).Name;
                    }
                }
                if (stage.prefabs==null)
                {
                    tempNull += stage.name + ",";
                }
                list.Add(stage);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("ShopAsset: " + i + " " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
    public List<ShopData> GetListItemShop(TypeShop typeShop)
    {
        return list.Where(x => x.typeShop == typeShop).OrderBy(x =>
        x.countBuild >=
        (x.typeShop == TypeShop.Animals ? x.quantityBuild * GetCountBuildCage(x) 
        : x.quantityBuild/2)
        ? x.levelUnlock2 : x.levelUnlock).ToList();
    }
    public string GetNameCage(string name)
    {
        return dataJson.FirstOrDefault(x => x.TypeShop.Equals("Farms") && name.ToUpper().Contains(x.Name.ToUpper())).Name;
    }
    public int GetCountBuildCage(ShopData animals)
    {
        int temp = list.FirstOrDefault(x => x.name.Equals(animals.description)).countBuild;
        return temp == 0 ? 1 : temp;
    }

    public ShopData GetAnimalsByProduct(string nameCage)
    {
        return list.Where(x =>x.typeShop == TypeShop.Animals).FirstOrDefault(x => nameCage.ToUpper().Contains(x.description.ToUpper()));
    }

    public void DestroyOldTre(string name)
    {
        list.FirstOrDefault(x => x.name.ToUpper().Contains(name.ToUpper())).countBuild--;
    }
}
#region object data
[System.Serializable]
public class ShopDataSave : SaveData
{
    public int exp;
    public int price;
    public int price2;
    public int levelUnlock;
    public int levelUnlock2;
    public int quantityBuild;

    public int countBuild
    {
        set { PlayerPrefs.SetInt("countBuild" + name, value); }
        get { return PlayerPrefs.GetInt("countBuild" + name); }
    }

}
[System.Serializable]
public class ShopData : ShopDataSave
{
    public string nameV;
    public TypeShop typeShop;
    public Sprite spIcon;
    public GameObject prefabs;
    public string GetName
    {
        get
        {
            if (Util.isVietnamese)
                return nameV;
            return name;
        }
    }
    public int GetPrice
    {
        get
        {
            if (typeShop == TypeShop.Animals)
            {
                return countBuild > quantityBuild * DataManager.ShopAsset.GetCountBuildCage(this) ? price2 : price;
            }
            return countBuild > quantityBuild ? price2 : price;
        }
    }
}
[System.Serializable]
public enum TypeShop
{
    Farms = 0, Factories = 1, Animals = 2, Plants = 3, Decorations = 4
}
[System.Serializable]
public class DataShop
{
    public string Name;
    public string NameV;
    public string TypeShop;
    public int Exp;
    public int Price;
    public int Price2;
    public int Level1;
    public int Level2;
    public int QuantityBuild;
}
[System.Serializable]
public class ListDataShop
{
    public List<DataShop> dataShops;
}

#endregion