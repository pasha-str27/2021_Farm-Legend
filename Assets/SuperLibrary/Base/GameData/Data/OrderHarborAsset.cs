using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "OrderHarborAsset", menuName = "DataAsset/OrderHarborAsset")]
public class OrderHarborAsset : BaseAsset<OrderHarborData>
{
    [Header("Level design")]
    [SerializeField] float mutilOrder = 1f;
    [SerializeField] int maxOrderHarbor = 9;
    [SerializeField] int minItemNeed = 3;
    [SerializeField] int maxItemNeed = 6;
    [SerializeField] int maxCountNeed = 6;

    [SerializeField] List<DataOrderHarbor> dataOrders = new List<DataOrderHarbor>();

    string KEY_ORDER = "saveOrder_Harbor";
    string KEY_NAME_ORDER = "OrderHarbor";
    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        dataOrders.Clear();
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
                OrderHarborData stage = new OrderHarborData();
                stage.id = list.Count + "";
                stage.index = list.Count;
                stage.name = KEY_NAME_ORDER + list.Count;
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

    public void ResetOrder(int index)
    {
        dataOrders[index] = GetRandomOrder(index);
        AddData();
    }
    public void PustOrder(OrderHarborData dataOrder)
    {
        for (int i = 0; i < dataOrder.productNeeds.Count; i++)
        {
            ProductData productData = DataManager.ProductAsset.GetProductByName(dataOrder.productNeeds[i].name);
            productData.total -= dataOrder.productNeeds[i].need;
        }
        ResetOrder(dataOrder.index);
    }
    public void CreatOrder()
    {
        Debug.Log("=> CreatOrder ");
        //call level 10 unlock - creat maxOrderHarbor
        dataOrders.Clear();
        for (int i = 0; i < maxOrderHarbor; i++)
        {
            dataOrders.Add(GetRandomOrder(i));
        }
        AddData();
    }
    public OrderHarborData GetOrder(int index)
    {
        return list.FirstOrDefault(x => x.index == index);
    }
   
    public void LoadOrder()
    {
        //call level 10 unlock - creat maxOrderHarbor
        if (!PlayerPrefs.HasKey(KEY_ORDER))
        {
            for (int i = 0; i < maxOrderHarbor; i++)
                dataOrders.Add(GetRandomOrder(i));
            Save();
        }
        else
        {
            dataOrders = JsonUtility.FromJson<ListDataOrderHarbor>(PlayerPrefs.GetString(KEY_ORDER)).Items;
        }
        Debug.Log("=> CreatOrder LoadOrder "+ dataOrders.Count);
        AddData();
    }
    public int GetCoin(List<DataItemOrderHarbor> dataOrders)
    {
        int tempCoin = 0;
        for (int i = 0; i < dataOrders.Count; i++)
        {
            ProductData data = DataManager.ProductAsset.GetProductByName(dataOrders[i].name);
            tempCoin += data.cell * dataOrders[i].need;
        }
        
        return (int)(tempCoin + tempCoin * mutilOrder);
    }
    public int GetExp(List<DataItemOrderHarbor> dataOrders)
    {
        int tempCoin = 0;
        for (int i = 0; i < dataOrders.Count; i++)
        {
            ProductData data = DataManager.ProductAsset.GetProductByName(dataOrders[i].name);
            tempCoin += data.exp * dataOrders[i].need;
        }
        return (int)(tempCoin + tempCoin * mutilOrder);
    }

    DataOrderHarbor GetRandomOrder(int index)
    {
        DataOrderHarbor data = new DataOrderHarbor();
        data.name = KEY_NAME_ORDER+ index;
        int rdItemNeed = UnityEngine.Random.Range(minItemNeed, maxItemNeed);
        data.productNeeds = new List<DataItemOrderHarbor>();
        List<ProductData> productDatas = GetAllProductUnlockForOrder();
        int rdPr = 0;
        for (int j = 0; j < rdItemNeed; j++)
        {
            if (productDatas.Count == 0 || j > 5)
                break;

            rdPr = UnityEngine.Random.Range(0, productDatas.Count);
            ProductData product = productDatas[rdPr];
            DataItemOrderHarbor dataItem = new DataItemOrderHarbor();
            dataItem.name = product.name;
            dataItem.need = UnityEngine.Random.Range(1, maxCountNeed);
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
        Debug.Log("=> SaveOrder harbor " + dataOrders.Count);
    }
}
#region object data
[System.Serializable]
public class OrderHarborData : SaveData
{
    public int coin;
    public int exp;
    public int timeSend
    {
        get
        {
            int temp = 0;
            for (int i = 0; i < productNeeds.Count; i++)
            {
                ProductData product = DataManager.ProductAsset.GetProductByName(productNeeds[i].name);
                temp += product.time * productNeeds[i].need;
            }
            if (temp > 600)//chuyển hàng tối đa 10p
                temp = 600;
            return temp;
        }
    }
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
    public List<DataItemOrderHarbor> productNeeds;
    public bool isComplete
    {
        set
        {
            PlayerPrefs.SetString("isCompleteOrderHarbor" + name, value == true ? "true" : "false");
            isUnlocked = value;
        }
        get
        {
            isUnlocked = PlayerPrefs.GetString("isCompleteOrderHarbor" + name).Equals("true");
            return isUnlocked;
        }
    }
    public Sprite spIcon
    {
        get { return DataManager.ProductAsset.GetProductByName(productNeeds[UnityEngine.Random.Range(0, productNeeds.Count)].name).icon; }
    }
}

//object json
[System.Serializable]
public class DataOrderHarbor
{
    public string name;
    public List<DataItemOrderHarbor> productNeeds;
}
[System.Serializable]
public class DataItemOrderHarbor
{
    public string name;
    public int need;
}
[System.Serializable]
public class ListDataOrderHarbor
{
    public List<DataOrderHarbor> Items;
}
[System.Serializable]
public class DataNameOrderHarbor
{
    public List<string> names;
}
#endregion