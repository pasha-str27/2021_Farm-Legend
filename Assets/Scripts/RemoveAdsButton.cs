using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class RemoveAdsButton : MonoBehaviour
{
    [SerializeField] string itemID = "no_ads";
    [SerializeField] Button removeAdsButton;
    [SerializeField] UnityEvent onRemoveButtonClick;

    [SerializeField] List<GameObject> removeAdsButtons;

    private void Start()
    {
        removeAdsButton.onClick.AddListener(() => onRemoveButtonClick.Invoke());
    }

    void OnEnable()
    {
        if(PlayerPrefs.GetInt(itemID, 0) == 1)
            removeAdsButtons.ForEach(x => Destroy(x));
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

    void DestroyButton() => removeAdsButtons.ForEach(x => Destroy(x));
}
