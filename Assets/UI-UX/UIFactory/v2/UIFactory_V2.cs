using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFactory_V2 : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] ItemWaitingProduct[] itemWaitings;
    [ReadOnly] [SerializeField] List<ProductData> listDataWaiting = new List<ProductData>();
    public void Show(List<ProductData> listDataWaiting)
    {
        this.listDataWaiting = listDataWaiting;

        uIAnimation.Show();
    }
    public void Hie()
    {
        uIAnimation.Hide();
    }
    private void OnEnable()
    {
        //this.RegisterListener((int)EventID.OnShowUIFactory, OnShowUIFactoryHandle);
        //this.RegisterListener((int)EventID.OnHideUIFactory, OnHideUIFactoryHandle);
    }
}
