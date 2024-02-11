using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "OrderAsset", menuName = "DataAsset/OrderAsset")]
public class OrderAsset : BaseAsset<OrderData>
{
    [Header("Level design")]
    [SerializeField] float numCoin = 0.5f;
    [SerializeField] float numExp = 0.5f;
    [Header("Data json")]
    [SerializeField] string data_name;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<string> lisName;

    [SerializeField] List<DataOrder> dataOrders = new List<DataOrder>();

    string KEY_ORDER = "saveOrder";
    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        dataOrders.Clear();
        lisName = JsonUtility.FromJson<DataNameOrder>(data_name).names;
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
        list.Clear();
        for (int i = 0; i < dataOrders.Count; i++)
        {
            try
            {
                OrderData stage = new OrderData();
                stage.id = list.Count + "";
                stage.index = list.Count;
                stage.name = lisName[i];
                stage.productNeeds = dataOrders[i].productNeeds;
                stage.coin = GetCoin(dataOrders[i].productNeeds);
                stage.exp = GetExp(dataOrders[i].productNeeds);
                list.Add(stage);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("OrderAsset: " + i + " " + ex.Message + " " + ex.StackTrace);
            }
        }
    }

    public void ResetOrder(string nameOrder)
    {
        int tempId = dataOrders.FirstIndex(x => x.name.Equals(nameOrder));
        Debug.Log("=> ResetOrder " + (tempId));
        dataOrders[tempId] = GetRandomOrder(nameOrder);
        AddData();
    }
    public void PustOrder(string nameOrder)
    {
        DataOrder dataOrder = dataOrders.FirstOrDefault(x => x.name.Equals(nameOrder));
        for (int i = 0; i < dataOrder.productNeeds.Count; i++)
        {
            ProductData productData = DataManager.ProductAsset.GetProductByName(dataOrder.productNeeds[i].name);
            productData.total -= dataOrder.productNeeds[i].need;

        }
        ResetOrder(nameOrder);
    }
    public void LoadOrder()
    {
        if (!PlayerPrefs.HasKey(KEY_ORDER))
        {
            for (int i = 0; i < PlayerPrefSave.Level; i++)
            {
                if (i < DataManager.GameConfig.maxOrder)
                {
                    dataOrders.Add(GetRandomOrder(lisName[i]));
                }
            }
        }
        else
        {
            dataOrders = JsonUtility.FromJson<ListDataOrder>(PlayerPrefs.GetString(KEY_ORDER)).Items;
        }
        AddData();
    }
    public void LoadNewOrder()
    {
        for (int i = dataOrders.Count; i < PlayerPrefSave.Level; i++)
        {
            if (i < DataManager.GameConfig.maxOrder)
            {
                dataOrders.Add(GetRandomOrder(lisName[i]));
            }
        }
        AddData();
    }
    public int GetCoin(List<DataItemOrder> dataOrders)
    {
        int tempCoin = 0;
        for (int i = 0; i < dataOrders.Count; i++)
        {
            ProductData data = DataManager.ProductAsset.list.FirstOrDefault(x => x.name.ToUpper().Equals(dataOrders[i].name.ToUpper()));
            tempCoin += data.cell * dataOrders[i].need;
        }
        return (int)(tempCoin+ tempCoin * DataManager.GameConfig.mutilOrder);
    }
    public int GetExp(List<DataItemOrder> dataOrders)
    {
        int tempCoin = 0;
        for (int i = 0; i < dataOrders.Count; i++)
        {
            ProductData data = DataManager.ProductAsset.list.FirstOrDefault(x => x.name.ToUpper().Equals(dataOrders[i].name.ToUpper()));
            tempCoin += data.exp * dataOrders[i].need;
        }
        return (int)(tempCoin + tempCoin * DataManager.GameConfig.mutilOrder);
    }

    DataOrder GetRandomOrder(string name)
    {
        DataOrder data = new DataOrder();
        data.name = name;
        int rdItemNeed = UnityEngine.Random.Range(1, PlayerPrefSave.Level);
        data.productNeeds = new List<DataItemOrder>();
        List<ProductData> productDatas = GetAllProductUnlockForOrder();
        int rdPr = 0;
        for (int j = 0; j < rdItemNeed; j++)
        {
            if (productDatas.Count == 0 || j > 5)
                break;

            rdPr = UnityEngine.Random.Range(0, productDatas.Count);
            ProductData product = productDatas[rdPr];
            DataItemOrder dataItem = new DataItemOrder();
            dataItem.name = product.name;
            dataItem.need = UnityEngine.Random.Range(1, (PlayerPrefSave.Level < 8 ? PlayerPrefSave.Level : 8) + 2);
            data.productNeeds.Add(dataItem);
            productDatas.RemoveAt(rdPr);
        }
        return data;
    }
    List<ProductData> GetAllProductUnlockForOrder()
    {
        return DataManager.ProductAsset.list.Where(x => x.unlocked
        && (x.tabName == TabName.Crops || x.tabName == TabName.Food
        || x.tabName == TabName.Garment || x.tabName == TabName.OldTree)).ToList();
    }

    public void Save()
    {
        var json = JsonHelper.ToJson(dataOrders, true);
        PlayerPrefs.SetString(KEY_ORDER, json);
        Debug.Log("=> SaveOrder " + dataOrders.Count);
    }
}
#region object data
[System.Serializable]
public class OrderData : SaveData
{
    public int coin;
    public int exp;
    public bool checkCompleteOrder
    {
        get
        {

            for (int i = 0; i < productNeeds.Count; i++)
            {
                if (DataManager.ProductAsset.GetProductByName(productNeeds[i].name).total < productNeeds[i].need)
                    return false;
            }

            return true;
        }
    }
    public List<DataItemOrder> productNeeds;
    public bool isComplete
    {
        set
        {
            PlayerPrefs.SetString("isCompleteOrder" + name, value == true ? "true" : "false");
            isUnlocked = value;
        }
        get
        {
            isUnlocked = PlayerPrefs.GetString("isCompleteOrder" + name).Equals("true");
            return isUnlocked;
        }
    }
}

//object json
[System.Serializable]
public class DataOrder
{
    public string name;
    public List<DataItemOrder> productNeeds;
}
[System.Serializable]
public class DataItemOrder
{
    public string name;
    public int need;
}
[System.Serializable]
public class ListDataOrder
{
    public List<DataOrder> Items;
}
[System.Serializable]
public class DataNameOrder
{
    public List<string> names;
}
#endregion