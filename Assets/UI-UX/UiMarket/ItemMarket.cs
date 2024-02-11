using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMarket : MonoBehaviour
{
    [SerializeField] GameObject objLock;
    [SerializeField] Text txtGem;
    [SerializeField] GameObject objActive;
    [SerializeField] GameObject objBuy;
    [SerializeField] Image iconProduct;
    [SerializeField] Text txtQuantityProduct;
    [SerializeField] Text txtCoinProduct;
    [SerializeField] GameObject objSold;
    [SerializeField] GameObject objEmpty;
    [SerializeField] GameObject bgQuantity;

    [SerializeField] Sprite spCoin;
    [ReadOnly] [SerializeField] bool isBuy;
    [ReadOnly] [SerializeField] DataMarket dataMarket;
    [ReadOnly] [SerializeField] ProductData productData;

    int coin = 0;
    public void FillData(DataMarket dataMarket, bool isBuy)
    {
        this.isBuy = isBuy;
        this.dataMarket = dataMarket;
        ReLoad();
    }
    void ReLoad()
    {
        objLock.SetActive(false);
        objActive.SetActive(false);
        objBuy.SetActive(false);
        iconProduct.gameObject.SetActive(false);
        objSold.SetActive(false);
        objEmpty.SetActive(false);
        if (isBuy)
        {
            LoadBuy();
        }
        else
        {
            LoadSale();
        }
    }
    void LoadSale()
    {
        if (dataMarket.unlocked)
        {
            objActive.SetActive(true);
            if (dataMarket.nameProduct != "")
            {
                productData = DataManager.ProductAsset.GetProductByName(dataMarket.nameProduct);
                coin = dataMarket.countProduct * productData.cell;
                objBuy.SetActive(true);
                iconProduct.gameObject.SetActive(true);
                bgQuantity.SetActive(!MarketManager.IsCompliteSale);
                iconProduct.sprite = MarketManager.IsCompliteSale ? spCoin : productData.icon;
                //iconProduct.sprite = productData.icon;
                iconProduct.SetNativeSize();
                txtCoinProduct.text = Util.Convert(coin);
                txtQuantityProduct.text = dataMarket.countProduct + "";
            }
            else
            {
                objEmpty.SetActive(true);
            }
        }
        else
        {
            objLock.SetActive(true);
            txtGem.text = dataMarket.gem + "";
        }
    }
    void LoadBuy()
    {
        if (dataMarket.unlocked)
        {
            objActive.SetActive(true);
            bgQuantity.SetActive(true);
            objSold.SetActive(dataMarket.isSold);
            if (!dataMarket.isSold)
            {
                if (dataMarket.nameProduct == "")
                    DataManager.MarketAsset.NewDataMarket(dataMarket);
                productData = DataManager.ProductAsset.GetProductByName(dataMarket.nameProduct);
                objBuy.SetActive(true);
                iconProduct.gameObject.SetActive(true);
                coin = dataMarket.countProduct * productData.cell;
                iconProduct.sprite = productData.icon;
                iconProduct.SetNativeSize();
                txtCoinProduct.text = Util.Convert(coin);
                txtQuantityProduct.text = dataMarket.countProduct + "";
            }
        }
        else
        {
            objLock.SetActive(true);
            txtGem.text = dataMarket.gem + "";
        }
    }
    public void Btn_AddPos_Click()
    {
        if (CoinManager.totalDiamond >= dataMarket.gem)
        {
            CoinManager.AddDiamond(-dataMarket.gem);
            //this.PostEvent((int)EventID.OnFxPutDiamond, txtGem.transform.position);
            dataMarket.unlocked = true;
            
            if(isBuy)
                DataManager.MarketAsset.NewDataMarket(dataMarket);
            this.PostEvent((int)EventID.OnUnlockSlotMarket, isBuy);
            ReLoad();
        }
        else
        {
            UIToast.Show("Not enough diamond!", null, ToastType.Notification, 1.5f);
            this.PostEvent((int)EventID.OnShowVideoReward);
            return;
        }
    }

    public void OnEmpty_Click()
    {
        if (MarketManager.IsCompliteSale)
        {
            if (MarketManager.IsNotEmptySale)
            {
                UIToast.Show("Please accept all sales!", null, ToastType.Notification, 1.5f);
            }
            else
                this.PostEvent((int)EventID.OnAddProductMarket, dataMarket);
        }
        else
        {
            this.PostEvent((int)EventID.OnAddProductMarket, dataMarket);
        }
    }
    public void OnItem_Click()
    {
        if (isBuy)
        {
            if (!dataMarket.isSold)
            {
                if (CoinManager.totalCoin >= dataMarket.countProduct * productData.cell)
                {
                    CoinManager.AddCoin(-(dataMarket.countProduct * productData.cell));
                }
                else
                {
                    UIToast.Show("Not enough coin!", null, ToastType.Notification, 1.5f);
                    this.PostEvent((int)EventID.OnShowVideoReward);
                    return;
                }

                this.PostEvent((int)EventID.OnAddProduct,
                new MessagerAddProduct
                {
                    data = productData,
                    onDone = () =>
                    {
                        productData.total += dataMarket.countProduct;
                        dataMarket.isSold = true;
                        ReLoad();
                    },
                    onFail = () =>
                    {
                        UIToast.Show("The warehouse is full!", null, ToastType.Notification, 1.5f);
                    }
                }
                );
            }else
                UIToast.Show("Please wait for the new sale!", null, ToastType.Notification, 1.5f);
        }
        else
        {
            if (MarketManager.IsCompliteSale)
            {
                CoinManager.AddCoin(dataMarket.countProduct * productData.cell, transform,null,"canvas");
                dataMarket.nameProduct = "";
                ReLoad();
                this.PostEvent((int)EventID.OnClaimCoinMarket);
            }
            else
            {
                UIToast.Show("Products are on sale!", null, ToastType.Notification, 1.5f);
            }
        }
    }
}
