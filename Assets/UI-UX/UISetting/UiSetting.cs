using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSetting : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;
    //[SerializeField] Text txtVersion;
    
    public void Show()
    {
        //txtVersion.text = "Version "+Application.version+" build "+ UIManager.BundleVersion + "";
        uIAnimation.Show();
    }
    public void Hide()
    {
        uIAnimation.Hide();
    }
}
