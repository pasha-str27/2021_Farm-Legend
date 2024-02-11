using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "MarketAsset", menuName = "DataAsset/MarketAsset")]
public class MarketAsset : BaseAsset<DataMarket>
{
    [ReadOnly] public List<DataMarket> listBuy;
    [ReadOnly] public List<DataMarket> listSell;

    [Header("Data json")]
    [SerializeField] string data_json_Buy;
    [SerializeField] string data_json_Cell;
    [Header("ReadOnly")]
    [ReadOnly] [SerializeField] List<MarketData> lisJson_Buy;
    [ReadOnly] [SerializeField] List<MarketData> lisJson_Cell;

    [ButtonMethod]
    public void AddAllData()
    {
        list.Clear();
        listBuy.Clear();
        listSell.Clear();
        lisJson_Buy = JsonUtility.FromJson<ListDataMarket>(data_json_Buy).marketDatas;
        lisJson_Cell = JsonUtility.FromJson<ListDataMarket>(data_json_Cell).marketDatas;
        AddData(listBuy, lisJson_Buy, "buy");
        AddData(listSell, lisJson_Cell, "Sale");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    public override void ResetData()
    {
        base.ResetData();
        AddAllData();
    }
    private void AddData(List<DataMarket> listBuy, List<MarketData> lisJson, string type)
    {
        for (int i = 0; i < lisJson.Count; i++)
        {
            try
            {
                DataMarket stage = new DataMarket();
                stage.name = "slot" + lisJson[i].id;
                stage.id = lisJson[i].id + "";
                stage.index = lisJson[i].id;
                stage.isUnlocked = false;
                stage.unlockType = UnlockType.Gold;
                stage.isSelected = false;

                stage.type = type;
                if (type == "buy")
                    stage.unlocked = true;
                //stage.isSold = false;
                //stage.nameProduct = "";
                //stage.countProduct = 0;
                stage.gem = lisJson[i].gem;
                listBuy.Add(stage);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AddAllExercises: " + i + " " + ex.Message + " " + ex.StackTrace);
            }
        }
    }

    public void LoadNewProductBuy()
    {
        Debug.Log("=> LoadNewProductBuy");
        List<ProductData> productDatas = DataManager.ProductAsset.list.Where(x => x.unlocked && x.tabName != TabName.Material).ToList();
        List<ProductData> tempList = DataManager.ProductAsset.list.Where(x => x.unlocked && x.tabName != TabName.Material).ToList();
        int tempRd = 0;
        for (int i = 0; i < listBuy.Count; i++)
        {
            if (productDatas.Count > 0)
            {
                tempRd = Random.Range(0, productDatas.Count);
                listBuy[i].nameProduct = productDatas[tempRd].name;
                listBuy[i].description = productDatas[tempRd].name;
                listBuy[i].countProduct = Random.Range(1, 6);
                listBuy[i].isSold = false;
                productDatas.RemoveAt(tempRd);
            }
            else
            {
                tempRd = Random.Range(0, tempList.Count);
                listBuy[i].nameProduct = tempList[tempRd].name;
                listBuy[i].description = tempList[tempRd].name;
                listBuy[i].countProduct = Random.Range(1, 6);
                listBuy[i].isSold = false;
            }
        }
    }
    public void NewDataMarket(DataMarket dataMarket)
    {
        List<ProductData> productDatas = DataManager.ProductAsset.list.Where(x => x.unlocked && x.tabName != TabName.Material).ToList();
        DataMarket temp = listBuy.FirstOrDefault(x => x.id == dataMarket.id);
        temp.nameProduct = productDatas[Random.Range(0, productDatas.Count)].name;
        temp.description = temp.nameProduct;
        temp.countProduct = Random.Range(1, 6);
        temp.isSold = false;
    }
}
#region object data
[System.Serializable]
public class DataMarket : SaveData
{
    public string type;
    public int gem;
    public string nameProduct
    {
        set
        {
            PlayerPrefs.SetString("nameProductMarket" + type + id, value);
        }
        get { return PlayerPrefs.GetString("nameProductMarket" + type + id, ""); }
    }
    public int countProduct
    {
        set
        {
            PlayerPrefs.SetInt("countProductMarket" + type + id, value);
        }
        get { return PlayerPrefs.GetInt("countProductMarket" + type + id, 0); }
    }
    public bool unlocked
    {
        set
        {
            isUnlocked = value;
            PlayerPrefs.SetInt("unlockedMarket" + type + id, value == true ? 1 : 0);
            //Debug.Log("=> Unlock itemSell->" + id + type);
        }
        get
        {
            if (index < 3)
                return true;
            //Debug.Log("=> Unlock get itemSell->" + id + type+ (PlayerPrefs.GetInt("unlockedMarket" + type + id, 0) == 1));
            return PlayerPrefs.GetInt("unlockedMarket" + type + id, 0) == 1;
        }
    }
    public bool isSold
    {
        set
        {
            PlayerPrefs.SetString("isSoldMarket" + type + id, value == true ? "true" : "false");
        }
        get
        {
            return PlayerPrefs.GetString("isSoldMarket" + type + id).Equals("true");
        }
    }

}

[System.Serializable]
public class MarketData
{
    public int id;
    public int gem;
}

[System.Serializable]
public class ListDataMarket
{
    public List<MarketData> marketDatas;
}
#endregion