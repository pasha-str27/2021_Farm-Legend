using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonType : MonoBehaviour
{
    [SerializeField] TabName tabName;
    public void OnClick()
    {
        this.PostEvent((int)EventID.OnClickButtonTab, tabName);
    }
}
