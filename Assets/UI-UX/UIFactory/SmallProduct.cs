using UnityEngine;
using UnityEngine.UI;

public class SmallProduct : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] int idProduct;
    [SerializeField] Text emptyText;
    [SerializeField] Image iconProduct;
    [SerializeField] Button unlockBtn;
    [SerializeField] Text txtDiamond;

    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnAddProductFactory, OnShowUIFactoryHandle);
    }

    private void OnShowUIFactoryHandle(object obj)
    {
        var msg = (MessageFactory)obj;
        FillData(msg);
    }

    public void FillData(MessageFactory msg)
    {
        int levelFactory = PlayerPrefSave.GetLevelFactory(msg.id);
        idProduct = PlayerPrefSave.GetProductFactory(msg.id, id);

        if (id > levelFactory + 1)
        {
            emptyText.gameObject.SetActive(false);
            iconProduct.gameObject.SetActive(false);
            unlockBtn.gameObject.SetActive(true);
            unlockBtn.onClick.RemoveAllListeners();
            txtDiamond.text = "5";
            unlockBtn.onClick.AddListener(() =>
            {
                if (levelFactory < 5)
                {
                    if (PlayerPrefSave.Diamond >= 5)
                    {
                        CoinManager.AddDiamond(-5);
                        //this.PostEvent((int)EventID.OnFxPutDiamond, txtDiamond.transform.position);
                        PlayerPrefSave.SetLevelFactory(msg.id, PlayerPrefSave.GetLevelFactory(msg.id) + 1);
                        this.PostEvent((int)EventID.OnShowUIFactory, new MessagerUiFactory {time = msg.time, isShow = true, idFactory = msg.id });
                    }
                    else
                    {
                        this.PostEvent((int)EventID.OnShowVideoReward);
                        UIToast.Show("Not enought diamond!", null, ToastType.Notification, 1.5f);
                    }
                }
            });
            return;
        }

        if (idProduct == -1)
        {
            emptyText.gameObject.SetActive(true);
            emptyText.text = DataManager.LanguegesAsset.GetName("Empty");
            iconProduct.gameObject.SetActive(false);
            unlockBtn.gameObject.SetActive(false);
        }
        else if (idProduct > -1)
        {
            emptyText.gameObject.SetActive(false);
            iconProduct.gameObject.SetActive(true);
            iconProduct.sprite = DataManager.ProductAsset.list[idProduct].icon;
            iconProduct.SetNativeSize();
            unlockBtn.gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (unlockBtn.gameObject.activeInHierarchy)
            return;

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
