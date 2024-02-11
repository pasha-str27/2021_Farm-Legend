using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Purchasing;

public class RemoveAdsButton : MonoBehaviour
{
    [SerializeField] string itemID = "no_ads";

    void Start()
    {
        if(PlayerPrefs.GetInt(itemID, 0) == 1)
            Destroy(gameObject);
    }

    public void ButtonClick()
    {
        IAPManager.Instance.BuyConsumable(itemID, RemoveAdvertisement);
    }

    void RemoveAdvertisement(string productID, bool result, PurchaseFailureReason failureReason)
    {
        if (!result)
            return;

        PlayerPrefs.SetInt(itemID, 1);
        Invoke(nameof(DestroyButton), 1);
    }

    void DestroyButton() => Destroy(gameObject);
}
