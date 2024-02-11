using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFactory : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    [SerializeField] GameObject[] listSmallProduct;
    [SerializeField] GameObject mainProduct;

    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnShowUIFactory, OnShowUIFactoryHandle);
    }

    private void OnShowUIFactoryHandle(object obj)
    {
        var msg = (MessagerUiFactory)obj;
        if (msg.isShow)
        {
            uIAnimation.Show();
            PlayerPrefSave.IDChoose = msg.idFactory;

            for (int i = 0; i < listSmallProduct.Length; i++)
            {
                listSmallProduct[i].SetActive(false);
            }

            for (int i = 0; i <= (PlayerPrefSave.GetLevelFactory(msg.idFactory) + 1); i++)
            {
                if (i == 6) break;
                listSmallProduct[i].SetActive(true);
                listSmallProduct[i].GetComponent<SmallProduct>().FillData(new MessageFactory { id = msg.idFactory, time = msg.time});
            }

            mainProduct.GetComponent<MainProduct>().OnShowUIFactoryHandle(new MessageFactory { id = msg.idFactory, time = msg.time });
        }
        else
        {
            uIAnimation.Hide();
            PlayerPrefSave.IDChoose = -1;
        }
    }

    public void ButtonHideHandle()
    {
        uIAnimation.Hide();
    }
}

public class MessagerUiFactory
{
    public bool isShow = false;
    public int idFactory;
    public int time;
}
