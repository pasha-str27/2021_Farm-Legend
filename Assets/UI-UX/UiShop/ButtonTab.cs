using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypeTab
{
    TabShop, TabStorage, TabSilo, Achievement
}
public class ButtonTab : MonoBehaviour
{
    [SerializeField] GameObject noti;
    [SerializeField] GameObject select;
    [SerializeField] TypeTab typeTab;
    [SerializeField] TypeShop typeShop;
    [SerializeField] TabName tabName;
    private void Start()
    {
        select.SetActive(false);
        if(noti)
        noti.SetActive(false);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnClickButtonTab, OnClickButtonTabHandle);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickButtonTab, OnClickButtonTabHandle);
    }

    private void OnClickButtonTabHandle(object obj)
    {
        var msg = (MessagerTab)obj;
        
        switch (typeTab)
        {
            case TypeTab.TabShop:
                if (select.activeInHierarchy)
                {
                    if (msg.typeShop != typeShop)
                        Selecet(false);
                }
                else
                {
                    if (msg.typeShop == typeShop)
                        Selecet(true);
                }

                if (!PlayerPrefSave.IsTutorial)
                    return;
                if (PlayerPrefSave.stepTutorial == 2|| PlayerPrefSave.stepTutorial == 4)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 1:
                            PlayerPrefSave.stepTutorialCurrent = 2;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
                if (PlayerPrefSave.stepTutorial == 6)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 1:
                            PlayerPrefSave.stepTutorialCurrent = 2;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
                break;
            case TypeTab.TabStorage:
            case TypeTab.TabSilo:
                if (select.activeInHierarchy)
                {
                    if (msg.tabName != tabName)
                        Selecet(false);
                }
                else
                     if (msg.tabName == tabName)
                    Selecet(true);

                break;
            case TypeTab.Achievement:
                if (select.activeInHierarchy && msg.tabName != tabName)
                    Selecet(false);
                break;
        }
    }

    public void OnClick()
    {
        this.PostEvent((int)EventID.OnClickButtonTab, new MessagerTab { typeTab = typeTab, typeShop = typeShop, tabName = tabName });
        Selecet(true);
    }
    public void Selecet(bool isSelect)
    {
        select.SetActive(isSelect);
        if (noti)
            noti.SetActive(false);
    }
}

public class MessagerTab
{
    public TypeTab typeTab;
    public TypeShop typeShop;
    public TabName tabName;
}
