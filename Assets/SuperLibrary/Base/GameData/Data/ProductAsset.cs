using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "ProductAsset", menuName = "DataAsset/ProductAsset")]
public class ProductAsset : BaseAsset<ProductData>
{
    [Header("Level design")]
    public float mutilCoin = 0.1f;
    public float mutilExp = 0.2f;
    public int baseTimeExp = 30;
    [Header("Data json")]
    [SerializeField] string data_json;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<DataProduct> lisJson;
    [SerializeField] string tempProductNull = "";

    [Header("Assets")]
    [SerializeField] Sprite[] spIcon;
    [SerializeField] Sprite[] spIcon_Lock;
    [SerializeField] Sprite[] spStageCrops;

    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        tempProductNull = "";
        lisJson = JsonUtility.FromJson<ListDataProduct>(data_json).dataProducts;
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
        TabName tabName = TabName.Crops;

        for (int i = 0; i < lisJson.Count; i++)
        {
            tabName = (TabName)Enum.Parse(typeof(TabName), lisJson[i].tabName);
            for (int j = 0; j < lisJson[i].dataItem.Count; j++)
            {
                try
                {
                    ProductData stage = new ProductData();
                    stage.id = list.Count + "";
                    stage.index = list.Count;
                    stage.name = lisJson[i].dataItem[j].name;
                    stage.nameV = lisJson[i].dataItem[j].nameV;
                    stage.price = lisJson[i].dataItem[j].price;
                    stage.cell = lisJson[i].dataItem[j].cell;
                    stage.time = lisJson[i].dataItem[j].time;
                    stage.exp = GetExpByTime(stage.time);
                    stage.cropYields = lisJson[i].dataItem[j].harvest;
                    stage.levelUnlock = lisJson[i].dataItem[j].level;
                    stage.tabName = tabName;
                    stage.requirements = new List<Requirement>();
                    stage.unlocked = false;

                    if (lisJson[i].dataItem[j].Requirement != null)
                        if (lisJson[i].dataItem[j].Requirement.Length > 0)
                            stage.requirements = lisJson[i].dataItem[j].listRequirement;

                    stage.icon = spIcon.FirstOrDefault(x => x.name.Replace("_", " ").ToUpper().Equals(stage.name.ToUpper()));
                    stage.iconLock = spIcon_Lock.FirstOrDefault(x => x.name.Replace("_", " ").ToUpper().Contains(stage.name.ToUpper()));
                    //stage.unlocked = true;
                    if (tabName == TabName.Crops || tabName == TabName.OldTree || tabName == TabName.Flower)
                    {
                        if (!stage.unlocked)
                            stage.unlocked = j == 0 && tabName == TabName.Crops;

                        stage.spStage1 = spStageCrops.FirstOrDefault(x => x.name.Replace("_", " ").ToUpper().Equals(stage.name.ToUpper() + 1));
                        stage.spStage2 = spStageCrops.FirstOrDefault(x => x.name.Replace("_", " ").ToUpper().Equals(stage.name.ToUpper() + 2));
                        stage.spStage3 = spStageCrops.FirstOrDefault(x => x.name.Replace("_", " ").ToUpper().Equals(stage.name.ToUpper() + 3));

                        if (stage.spStage1 == null)
                            tempProductNull += stage.name + ",";
                    }



                    if (tabName == TabName.Material)
                    {
                        stage.unlocked = true;
                    }
                    tempProductNull += stage.icon == null ? stage.name + "," : "";
                    list.Add(stage);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("ProductAsset: " + i + " " + ex.Message + " " + ex.StackTrace);
                }
            }
        }
    }
    int GetExpByTime(int time)
    {
        int temp = (int)(time / baseTimeExp * mutilExp);
        return temp == 0 ? 1 : temp;
    }

    public int GetTotal(ObjectMouseDown objectMouseDown)
    {
        int temp = 0;
        switch (objectMouseDown)
        {
            case ObjectMouseDown.Silo:
                temp = list.Where(x => x.tabName == TabName.Crops || x.tabName == TabName.Flower || x.tabName == TabName.OldTree || x.tabName == TabName.Pet)
                    .Sum(x => x.total);
                break;
            case ObjectMouseDown.Storage:
                temp = list.Where(x => x.tabName == TabName.Food || x.tabName == TabName.Garment || x.tabName == TabName.FoodPet)
                    .Sum(x => x.total);
                break;
        }
        return temp;
    }

    public ProductData GetProduct(TabName tabName, string name)
    {
        return list.FirstOrDefault(x => x.tabName == tabName && x.name.ToUpper().Contains(name.ToUpper()) && x.icon != null);
    }
    public ProductData GetProductByName(string name)
    {
        return list.FirstOrDefault(x => x.name.ToUpper().Trim().Equals(name.ToUpper().Trim()) && x.icon != null);
    }
    public ProductData GetProductUnlockNext(TabName tabName)
    {
        return list.Where(x => x.tabName == tabName && x.isUnlocked == false).OrderBy(x => x.levelUnlock).FirstOrDefault();
    }
    public List<ProductData> GetListType(TabName tabName)
    {
        return list.Where(x => x.tabName == tabName && x.unlocked).OrderBy(x => x.levelUnlock).ToList();
    }
    public bool IsEnough(string name, int num)
    {
        return list.FirstOrDefault(x => x.name.ToUpper().Equals(name.ToUpper())).total >= num;
    }
    public void LoadItemUnlockProduct()
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].levelUnlock <= PlayerPrefSave.Level)
            {
                list[i].unlocked = true;
            }
        }
    }
    public void LoadCellAllProduct()
    {
        //LoadRequirments();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].requirements.Count > 0)
            {
                list[i].cell = GetCellRequirements(list[i].requirements);
            }
        }
    }
    int GetCellRequirements(List<Requirement> requirements)
    {
        int temp = 0;
        for (int i = 0; i < requirements.Count; i++)
        {
            if (GetProductByName(requirements[i].name) == null)
            {
                //Debug.Log("=> requirements[i].name " + requirements[i].name);
                continue;
            }
            temp += requirements[i].count * GetProductByName(requirements[i].name).cell;
        }
        return temp;
    }
    public void SetBaseMaterial(string name, int vl)
    {
        list.FirstOrDefault(x => x.tabName == TabName.Material && x.name.Equals(name)).total += vl;
    }
}
#region object data
[System.Serializable]
public class TabProductData : SaveData
{
    [Header("TabProductData")]
    public string tabName;
    public Sprite iconTab;
    [Header("Product tabName")]
    public List<ProductData> dataItem;
}
[System.Serializable]
public class ProductData : SaveData
{
    [Header("ProductData")]
    public string nameV;
    public Sprite icon;
    public Sprite iconLock;
    public int price;
    public int cell;
    public int time;
    public int exp;
    public int cropYields;
    public int levelUnlock;


    public string GetName
    {
        get
        {
            if (Util.isVietnamese)
                return nameV;
            return name;
        }
    }
    public int total
    {
        set
        {
            DataManager.ChangeCountProduct();
            PlayerPrefs.SetInt("totalProduct" + name, value);
        }
        get { return PlayerPrefs.GetInt("totalProduct" + name); }
    }
    public bool unlocked
    {
        set
        {
            PlayerPrefs.SetString("unlockedProduct" + name, value == true ? "true" : "false");
            isUnlocked = value;
        }
        get
        {
            isUnlocked = PlayerPrefs.GetString("unlockedProduct" + name).Equals("true");
            return isUnlocked;
        }
    }
    public TabName tabName;
    [Header("Crops stage")]
    public Sprite spStage1;
    public Sprite spStage2;
    public Sprite spStage3;
    [Header("Product requirements")]
    public List<Requirement> requirements;
}

[System.Serializable]
public class Requirement
{
    [Header("Requirement")]
    public string name;
    public int count;
}

[System.Serializable]
public enum TabName
{
    Food = 0, Garment = 1, FoodPet = 2, Material = 3, Crops = 4, Flower = 5, OldTree = 6, Pet = 7, BuyMarket = 8, SaleMarket = 9,
}
//object json
[System.Serializable]
public class DataItem
{
    public string name;
    public string nameV;
    public int price;
    public int cell;
    public int time;
    public int exp;
    public int harvest;
    public int level;
    public string Requirement;
    public List<Requirement> listRequirement
    {
        get
        {
            List<Requirement> tempList = new List<Requirement>();
            string[] tempArr = Requirement.Split(',');
            Requirement requirement = new Requirement();
            for (int j = 0; j < tempArr.Length; j++)
            {
                int num = int.Parse(tempArr[j].Substring(0, 1));
                string nameProduct = tempArr[j].Substring(2);
                requirement = new Requirement();
                requirement.count = num;
                requirement.name = nameProduct;
                tempList.Add(requirement);
            }
            return tempList;
        }
    }
}
[System.Serializable]
public class DataProduct
{
    public string tabName;
    public List<DataItem> dataItem;
}
[System.Serializable]
public class ListDataProduct
{
    public List<DataProduct> dataProducts;
}
#endregion