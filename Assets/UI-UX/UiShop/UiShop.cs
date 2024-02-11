using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShop : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] Animator anim;
    [SerializeField] ItemShop prItemShop;
    [SerializeField] Transform content;
    [SerializeField] GameObject noti;
    [SerializeField] Text txtCountNoti;
    [SerializeField] ScrollRect scrollRect;
    bool isShow = false;
    List<ShopData> shopDatas;
    TypeShop typeShopCurrent = TypeShop.Farms;
    private void Start()
    {
        noti.SetActive(false);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnClickButtonTab, OnClickButtonTabHandle);
        this.RegisterListener((int)EventID.OnDragItem, OnHideUIShopHanlde);
        this.RegisterListener((int)EventID.OnShowShop, OnShowShopHanlde);
        this.RegisterListener((int)EventID.OnHidePopupLevelUp, OnHidePopupLevelUpHanlde);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickButtonTab, OnClickButtonTabHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnDragItem, OnHideUIShopHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowShop, OnShowShopHanlde);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnHidePopupLevelUp, OnHidePopupLevelUpHanlde);
    }

    private void OnHidePopupLevelUpHanlde(object obj)
    {
        if (PlayerPrefSave.IsTutorial)
            return;
        var msg = (int)obj;
        if (msg > 0)
        {
            noti.SetActive(true);
            txtCountNoti.text = msg + "";
        }
    }

    private void OnShowShopHanlde(object obj)
    {
        isShow = true;
        anim.SetBool("show", isShow);
        this.PostEvent((int)EventID.OnClickButtonTab, new MessagerTab { typeTab = TypeTab.TabShop, typeShop = (TypeShop)obj });
    }

    private void OnHideUIShopHanlde(object obj)
    {
        var msg = (MessagerDragItem)obj;
        if (msg.productData != null)
            return;

        isShow = false;
        anim.SetBool("show", isShow);
    }

    private void OnClickButtonTabHandle(object obj)
    {
        var msg = (MessagerTab)obj;
        if (msg.typeTab != TypeTab.TabShop)
            return;
        if (typeShopCurrent != msg.typeShop && !PlayerPrefSave.IsTutorial)
            content.GetComponent<RectTransform>().localPosition = Vector3.zero;
        typeShopCurrent = msg.typeShop;
        InstanceItemShop(msg.typeShop);
    }
    void InstanceItemShop(TypeShop typeShop)
    {
        content.RecycleChild();
        shopDatas = DataManager.ShopAsset.GetListItemShop(typeShop);
        for (int i = 0; i < shopDatas.Count; i++)
        {
            var item = prItemShop.Spawn(content);
            item.FillData(shopDatas[i]);
        }
    }

    public void Show()
    {
        scrollRect.enabled = !PlayerPrefSave.IsTutorial;
        uIAnimation.Show(null, () =>
        {
        });
        AnalyticsManager.LogEvent("shop_open", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "time", DataManager.UserData.TotalTimePlay } });
    }
    public void Hide()
    {
        isShow = false;
        anim.SetBool("show", isShow);
    }
    public void Btn_Shop_Click()
    {
        if (!isShow)
        {
            isShow = true;
        }
        else isShow = false;
        noti.SetActive(false);
        anim.SetBool("show", isShow);
        scrollRect.enabled = !PlayerPrefSave.IsTutorial;
        InstanceItemShop(typeShopCurrent);
        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 1 || PlayerPrefSave.stepTutorial == 2 || PlayerPrefSave.stepTutorial == 4 || PlayerPrefSave.stepTutorial == 6)
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 0:
                    PlayerPrefSave.stepTutorialCurrent = 1;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
    }
}
