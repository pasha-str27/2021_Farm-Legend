using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text txtName;
    [SerializeField] Text txtExp;
    [SerializeField] Text txtPrice;
    [SerializeField] Text txtCount;
    [SerializeField] Text txtLevelLock;
    [SerializeField] GameObject objLock;
    [SerializeField] GameObject objUnLock;
    [ReadOnly]
    [SerializeField]
    public ShopData shopData;

    [HideInInspector]
    public GameObject prefab;
    [HideInInspector]
    public int indexBuilding;
    [ReadOnly] [SerializeField] int _price;
    int tempLevelUnlock;
    int tempQuantityBuild;
    public void FillData(ShopData shopData)
    {
        this.shopData = shopData;
        objLock.SetActive(false);
        objUnLock.SetActive(false);

        icon.sprite = shopData.spIcon;
        icon.SetNativeSize();
        txtName.text = shopData.GetName;
        txtExp.text = "+" + shopData.exp;
        prefab = shopData.prefabs;
        indexBuilding = shopData.index;


        txtPrice.text = Util.Convert(shopData.GetPrice);

        tempLevelUnlock = shopData.levelUnlock;
        tempQuantityBuild = shopData.quantityBuild;
        switch (shopData.typeShop)
        {
            case TypeShop.Farms:
                if (shopData.name.Equals("Land"))
                {
                    if (PlayerPrefSave.Level < 2)
                    {
                        if (shopData.countBuild < shopData.quantityBuild)
                        {
                            txtCount.text = shopData.countBuild + "/" + shopData.quantityBuild;
                            objUnLock.SetActive(true);
                        }
                        else
                        {
                            txtCount.text = shopData.countBuild + "/" + shopData.quantityBuild;
                            objLock.SetActive(true);
                            txtLevelLock.text = DataManager.LanguegesAsset.GetName("Unlock level")+" " + (PlayerPrefSave.Level + 1);
                        }
                    }
                    else
                    {
                        int quanti = shopData.quantityBuild + (PlayerPrefSave.Level - 1) * DataManager.GameConfig.NumUnlockLand;
                        if (shopData.countBuild < quanti)
                        {
                            txtCount.text = shopData.countBuild + "/" + quanti;
                            objUnLock.SetActive(true);
                        }
                        else
                        {
                            txtCount.text = shopData.countBuild + "/" + shopData.quantityBuild;
                            objLock.SetActive(true);
                            txtLevelLock.text = DataManager.LanguegesAsset.GetName("Unlock level") + " " + (PlayerPrefSave.Level + 1);
                        }
                    }

                    if(shopData.countBuild>= DataManager.GameConfig.MaxLand)
                    {
                        objUnLock.SetActive(false);
                        objLock.SetActive(true);
                        txtLevelLock.text = DataManager.LanguegesAsset.GetName("Bought out");
                    }
                }
                else
                {
                    if (PlayerPrefSave.Level >= tempLevelUnlock)
                    {
                        if (shopData.countBuild >= shopData.quantityBuild / 2)
                        {
                            tempLevelUnlock = shopData.levelUnlock2;
                        }
                    }
                }
                break;
            case TypeShop.Animals:
                if (PlayerPrefSave.Level >= tempLevelUnlock)
                {
                    tempQuantityBuild = shopData.quantityBuild * DataManager.ShopAsset.GetCountBuildCage(shopData);
                    if (shopData.countBuild >= tempQuantityBuild)
                    {
                        tempLevelUnlock = shopData.levelUnlock2;
                    }
                }
                break;
            case TypeShop.Factories:
            case TypeShop.Plants:
                if (PlayerPrefSave.Level >= tempLevelUnlock)
                {
                    if (shopData.countBuild >= shopData.quantityBuild / 2)
                    {
                        tempLevelUnlock = shopData.levelUnlock2;
                    }
                }
                break;
            case TypeShop.Decorations:
                if (PlayerPrefSave.Level >= tempLevelUnlock)
                {
                    txtCount.gameObject.SetActive(false);
                }
                break;
        }

        if (shopData.name.Equals("Land"))
            return;

        if (PlayerPrefSave.Level >= tempLevelUnlock)
        {
            objUnLock.SetActive(true);
        }
        else
        {
            objLock.SetActive(true);
        }

        txtCount.text = shopData.countBuild + "/" + tempQuantityBuild;
        txtLevelLock.text = DataManager.LanguegesAsset.GetName("Unlock level")+" " + tempLevelUnlock;

        if (shopData.countBuild >= tempQuantityBuild && shopData.typeShop != TypeShop.Decorations && shopData.typeShop != TypeShop.Animals)
        {
            objUnLock.SetActive(false);
            objLock.SetActive(true);
            txtLevelLock.text = DataManager.LanguegesAsset.GetName("Bought out");
        }

        //objLock.SetActive(false);
    }
}
