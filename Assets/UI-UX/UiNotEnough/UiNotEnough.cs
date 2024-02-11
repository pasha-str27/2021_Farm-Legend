using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiNotEnough : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] ItemExpand[] itemExpand;
    ProductData product;
    //private void OnEnable()
    //{
    //    this.RegisterListener((int)EventID.)
    //}
    //private void OnDisable()
    //{
        
    //}
    public void Show(MessagerUiNotEnought msg)
    {
        uIAnimation.Show();
        product = msg.productData;
        DisableAllItem();
        itemExpand[0].gameObject.SetActive(true);
        itemExpand[0].FillData(msg.productData, msg.need);
    }
    public void Hide()
    {
        uIAnimation.Hide();
    }
    void DisableAllItem()
    {
        for (int i = 0; i < itemExpand.Length; i++)
        {
            itemExpand[i].gameObject.SetActive(false);
        }
    }
    public void Btn_BuyAds_Click()
    {
        AdsManager.ShowVideoAds(() =>
        {
            product.total++;
            Hide();
        }, null,
        () => {
            AnalyticsManager.LogEvent("ads_buy_item_fail", new Dictionary<string, object> {
            { "name_product", product.name } });
        });
    }
}

public class MessagerUiNotEnought
{
    public ProductData productData;
    public int need;
}
