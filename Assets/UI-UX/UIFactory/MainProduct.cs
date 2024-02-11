using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainProduct : MonoBehaviour
{
    [SerializeField] int idProduct;
    [SerializeField] Text txtTime;
    [SerializeField] Text txtEmpty;
    [SerializeField] Image imgIcon;
    [SerializeField] Image imgIconSpeedUp;
    [SerializeField] Button btnTime;
    [SerializeField] Text txtDiamond;
    [SerializeField] GameObject txtX6FeedPet;
    [ReadOnly] [SerializeField]  ProductData product;
    int diamond = 0;
    int idFactory = 0;
    string key = "";
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnAddProductFactory, OnShowUIFactoryHandle);
        this.RegisterListener((int)EventID.OnSendTimeFactory, OnSendTimeFactoryHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnAddProductFactory, OnShowUIFactoryHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnSendTimeFactory, OnSendTimeFactoryHandle);
    }
    private void OnSendTimeFactoryHandle(object obj)
    {
        var msg = (int)obj;
        txtTime.text = Util.ConvertTime(msg);
    }

    public void OnShowUIFactoryHandle(object obj)
    {
        var msg = (MessageFactory)obj;
        key = msg.id.ToString();
        idProduct = PlayerPrefSave.GetProductFactory(msg.id, 0);
        if (idProduct == -1)
        {
            txtTime.gameObject.SetActive(false);
            txtEmpty.gameObject.SetActive(true);
            txtEmpty.text = DataManager.LanguegesAsset.GetName("Empty");
            imgIcon.gameObject.SetActive(false);
            imgIconSpeedUp.gameObject.SetActive(false);
            btnTime.gameObject.SetActive(false);
        }
        else if (idProduct > -1)
        {
            product = DataManager.ProductAsset.list[idProduct];
            txtTime.gameObject.SetActive(true);
            txtTime.text = Util.ConvertTime(msg.time);
            txtEmpty.gameObject.SetActive(false);
            imgIcon.gameObject.SetActive(true);
            imgIconSpeedUp.gameObject.SetActive(true);
            imgIcon.sprite = product.icon;
            imgIconSpeedUp.sprite = product.icon;
            txtX6FeedPet.SetActive(product.tabName == TabName.FoodPet);

            imgIconSpeedUp.SetNativeSize();
            imgIcon.SetNativeSize();

            btnTime.gameObject.SetActive(true);
            diamond = DataManager.GameConfig.GetDiamondTime(msg.time);
            txtDiamond.text = diamond.ToString();
        }
    }
    public void Btn_SpeedUp_Click()
    {
        AdsManager.ShowVideoAds(() =>
        {
            this.PostEvent((int)EventID.OnSpeedUp, new MessagerCountDown { keyId = key });
            this.PostEvent((int)EventID.OnHideToggleSuggestion);
        }, null,null);
    }
    public void Btn_Diamond_Click()
    {
        if (PlayerPrefSave.Diamond >= diamond)
        {
            CoinManager.AddDiamond(-diamond);
            this.PostEvent((int)EventID.OnSpeedUp, new MessagerCountDown { keyId = key });
        }
        else
        {
            this.PostEvent((int)EventID.OnShowVideoReward);
            UIToast.Show("Not enought diamond!", null, ToastType.Notification, 1.5f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("ObjFollow"))
        {
            ObjFollow objFollow = collision.GetComponent<ObjFollow>();
            if (objFollow.type == ObjectMouseDown.Factory)
            {
                if (objFollow.productData != null)
                {
                    objFollow.isTrigger = true;
                    this.PostEvent((int)EventID.OnAddProductFactoryUi, objFollow.productData);
                }
            }
        }
    }
}
