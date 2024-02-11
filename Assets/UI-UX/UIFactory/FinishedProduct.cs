using MyBox;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FinishedProduct : MonoBehaviour
{
    [SerializeField] ItemProductComplite[] itemComplite;
    [ReadOnly] public List<ProductData> listData = new List<ProductData>();
    [ReadOnly] [SerializeField] int idFactory;
    bool isFullStore = false;
    [ReadOnly] [SerializeField] List<ProductComplite> temp = new List<ProductComplite>();
    private void Start()
    {
        DisableAllItem();
        
    }
    public void StartLoadProductComplite(int id)
    {
        idFactory = id;
        if (listComplite != "")
        {
            temp = JsonHelper.FromJson<ProductComplite>(PlayerPrefs.GetString("listComplite" + idFactory));
            if (temp.Count > 0)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    listData.Add(DataManager.ProductAsset.GetProductByName(temp[i].nameProduct));
                }
            }
        }
        Invoke("LoadProductComplite", 3f);
    }
    private void OnDestroy()
    {
        temp = new List<ProductComplite>();
        if (listData.Count > 0)
        {
            for (int i = 0; i < listData.Count; i++)
            {
                temp.Add(new ProductComplite { nameProduct= listData[i].name });
            }
        }
        listComplite= JsonHelper.ToJson<ProductComplite>(temp);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnThuHoachFactory, OnThuHoachFactoryHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnThuHoachFactory, OnThuHoachFactoryHandle);
    }
    private void OnThuHoachFactoryHandle(object obj)
    {
        if ((int)obj == idFactory)
        {
            Hartvest();
        }
    }

    void Hartvest()
    {
        isFullStore = false;
        while (listData.Count > 0 && !isFullStore)
        {
            this.PostEvent((int)EventID.OnAddProduct,
              new MessagerAddProduct
              {
                  data = listData[0],
                  onDone = () =>
                  {
                      //thu huoạch
                      listData[0].total++;
                      CoinManager.AddExp(listData[0].exp, transform);
                      this.PostEvent((int)EventID.OnFxPutIn, new MessageFx { data = listData[0], typePut = TypePut.Collect, pos = transform.position });

                      //if ("Feed".Contains(data.name))
                      if (listData[0].name.Contains("Feed"))
                      {
                          this.PostEvent((int)EventID.OnUpdateAchie, 1);
                      }
                      if (listData[0].name.Equals("Bread"))
                      {
                          this.PostEvent((int)EventID.OnUpdateAchie, 8);
                      }
                      if (listData[0].name.Equals("Pants") || listData[0].name.Equals("Shirt")
                      || listData[0].name.Equals("Tshirt") || listData[0].name.Equals("Sweater"))
                      {
                          this.PostEvent((int)EventID.OnUpdateAchie, 9);
                      }
                      if (listData[0].name.Equals("Popcorn"))
                      {
                          this.PostEvent((int)EventID.OnUpdateAchie, 11);
                      }
                      if (listData[0].name.Equals("Apple Cakes") || listData[0].name.Equals("Bread")
                      || listData[0].name.Equals("Corn Bread") || listData[0].name.Equals("Pancake")
                      || listData[0].name.Equals("Cornflakes") || listData[0].name.Equals("Cabbage Pie")
                      || listData[0].name.Equals("Noodles") || listData[0].name.Equals("Park Noodles")
                      || listData[0].name.Equals("French Fries") || listData[0].name.Equals("Toast"))
                      {
                          this.PostEvent((int)EventID.OnUpdateAchie, 12);
                      }
                      //Burrito,Cheese Nachos, Enchiladas, Nachos, Quesadilla, Tacos
                      if (listData[0].name.Equals("Burrito") || listData[0].name.Equals("Cheese Nachos")
                      || listData[0].name.Equals("Enchiladas") || listData[0].name.Equals("Nachos")
                      || listData[0].name.Equals("Quesadilla") || listData[0].name.Equals("Tacos"))
                      {
                          this.PostEvent((int)EventID.OnUpdateAchie, 13);
                      }
                      listData.RemoveAt(0);
                      LoadProductComplite();
                  },
                  onFail = () =>
                  {
                      isFullStore = true;
                      UIToast.Show("The warehouse is full!", null, ToastType.Notification, 1.5f);
                  }
              }
              );
        }
        this.PostEvent((int)EventID.OnZoomCamera, false);
        SoundManager.Play("sfxHarvest");

    }

    public void ShowProductComplite(ProductData _data, int idFactory)
    {
        this.idFactory = idFactory;
        listData.Add(_data);
        LoadProductComplite();
    }
    void LoadProductComplite()
    {
        DisableAllItem();
        List<ProductData> tempListData = listData.Distinct().ToList();
        for (int i = 0; i < tempListData.Count; i++)
        {
            if (i < itemComplite.Length)
            {
                itemComplite[i].gameObject.SetActive(true);
                itemComplite[i].FillData(tempListData[i].icon, idFactory);
            }
        }
    }
    void DisableAllItem()
    {
        for (int i = 0; i < itemComplite.Length; i++)
        {
            itemComplite[i].gameObject.SetActive(false);
        }
    }

    string listComplite
    {
        set { PlayerPrefs.SetString("listComplite" + idFactory, value); }
        get { return PlayerPrefs.GetString("listComplite" + idFactory, ""); }
    }
}
[System.Serializable]
public class ProductComplite
{
    public string nameProduct;
}
