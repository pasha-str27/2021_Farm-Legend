using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "FactoryAsset", menuName = "DataAsset/FactoryAsset")]
public class FactoryAsset : BaseAsset<DataFactory>
{
    [Header("Data json")]
    [SerializeField] string data_json;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<FactoryData> lisJson;

    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        lisJson = JsonUtility.FromJson<ListDataFactory>(data_json).factoryDatas;
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
        for (int i = 0; i < lisJson.Count; i++)
        {
            try
            {
                DataFactory stage = new DataFactory();
                stage.id = list.Count + "";
                stage.index = list.Count;
                stage.isUnlocked = false;
                stage.unlockType = UnlockType.Gold;
                stage.isSelected = false;
                stage.name = lisJson[i].name;
                stage.nameV = lisJson[i].nameV;
                stage.products = lisJson[i].GetListProduct;
                list.Add(stage);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AddAllExercises: " + i + " " + ex.Message + " " + ex.StackTrace);
            }
        }
    }

    public List<ProductData> GetAllProductFactory(string nameFactory)
    {
        return list.FirstOrDefault(x => x.name.ToUpper().Contains(nameFactory.ToUpper())).AllProduct();
    }

    public void SetFullRequiment(string nameFactory)
    {
        list.FirstOrDefault(x => x.name.ToUpper().Contains(nameFactory.ToUpper())).FullProductRequiment();
    }
}
#region object data
[System.Serializable]
public class DataFactory : SaveData
{
    public string nameV;
    public List<string> products;
    public string GetName
    {
        get
        {
            if (Util.isVietnamese)
                return nameV;
            return name;
        }
    }
    public List<ProductData> AllProduct()
    {
        List<ProductData> tempAll = DataManager.ProductAsset.list;
        List<ProductData> temp = new List<ProductData>();
        for (int i = 0; i < products.Count; i++)
        {
            ProductData data = new ProductData();
            data = tempAll.FirstOrDefault(x => x.name.ToUpper().Equals(products[i].ToUpper()) && x.unlocked);
            if(data != null)
                temp.Add(data);
        }
        return temp.OrderBy(x => x.levelUnlock).ToList();
    }
    public void FullProductRequiment()
    {
        List<ProductData> temp = new List<ProductData>();
        temp = AllProduct();
        if (temp.Count == 0)
            return;
        ProductData product = temp[0];
        ProductData tempRe = null;
        for (int i = 0; i < product.requirements.Count; i++)
        {
            tempRe = DataManager.ProductAsset.GetProductByName(product.requirements[i].name);
            if (tempRe != null)
            tempRe.total += product.requirements[i].count;
        }
    }
}

[System.Serializable]
public class FactoryData
{
    public string name;
    public string nameV;
    public string products;
    public List<string> GetListProduct
    {
        get { return products.Split(',').ToList(); }
    }
}

[System.Serializable]
public class ListDataFactory
{
    public List<FactoryData> factoryDatas;
}
#endregion