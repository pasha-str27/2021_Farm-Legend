using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPManager : MonoBehaviour, IStoreListener
{

    public static IAPManager Instance;
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    private Action<string, bool, PurchaseFailureReason> PurchaserManager_Callback = delegate (string _iapID, bool _callBackState, PurchaseFailureReason reason) { };

    // public string 
    [SerializeField] string gold1 = "gold1";
    [SerializeField] string gold2 = "gold2";
    [SerializeField] string gold3 = "gold3";
    [SerializeField] string gold4 = "gold4";
    [SerializeField] string gold5 = "gold5";
    [SerializeField] string gold6 = "gold6";

    [SerializeField] string gem1 = "gem1";
    [SerializeField] string gem2 = "gem2";
    [SerializeField] string gem3 = "gem3";
    [SerializeField] string gem4 = "gem4";
    [SerializeField] string gem5 = "gem5";
    [SerializeField] string gem6 = "gem6";

    public string no_ads = "no_ads";

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public bool IsInitialized()
    {
//#if UNITY_EDITOR
//        return true;
//#endif
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void InitializePurchasing()
    {
        Debug.Log("here");

        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(this.gold1, ProductType.Consumable);
        builder.AddProduct(this.gold2, ProductType.Consumable);
        builder.AddProduct(this.gold3, ProductType.Consumable);
        builder.AddProduct(this.gold4, ProductType.Consumable);
        builder.AddProduct(this.gold5, ProductType.Consumable);
        builder.AddProduct(this.gold6, ProductType.Consumable);

        builder.AddProduct(this.gem1, ProductType.Consumable);
        builder.AddProduct(this.gem2, ProductType.Consumable);
        builder.AddProduct(this.gem3, ProductType.Consumable);
        builder.AddProduct(this.gem4, ProductType.Consumable);
        builder.AddProduct(this.gem5, ProductType.Consumable);
        builder.AddProduct(this.gem6, ProductType.Consumable);

        builder.AddProduct(no_ads, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyConsumable(string iapID, Action<string, bool, PurchaseFailureReason> _purchaserManager_Callback)
    {
        PurchaserManager_Callback = _purchaserManager_Callback;
        BuyProductID(iapID);
    }

    void BuyProductID(string productId)
    {
#if UNITY_EDITOR
        PurchaserManager_Callback.Invoke(productId, true, PurchaseFailureReason.Unknown);
#else
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                PurchaserManager_Callback.Invoke(productId, false,PurchaseFailureReason.Unknown);
            }
        }
        else
        {
            PurchaserManager_Callback.Invoke(productId, false,PurchaseFailureReason.Unknown);
        }
#endif
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    public string GetItemPrice(string itemID) => m_StoreController.products.all.First(x => x.definition.id == itemID).metadata.localizedPriceString;

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        bool validPurchase = true;
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);
        try
        {
            var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        }
        catch (IAPSecurityException)
        {
            Debug.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#endif

        if (validPurchase)
        {
            PurchaserManager_Callback.Invoke(purchaseEvent.purchasedProduct.definition.id, true, PurchaseFailureReason.Unknown);
        }
        else
        {
            PurchaserManager_Callback.Invoke(purchaseEvent.purchasedProduct.definition.id, false, PurchaseFailureReason.Unknown);
        }


        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        PurchaserManager_Callback.Invoke(product.definition.id, false, failureReason);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        //throw new NotImplementedException();
    }
}