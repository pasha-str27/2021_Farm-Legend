using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguagesText : MonoBehaviour
{
    [ReadOnly] [SerializeField] Text text;
    private void OnEnable()
    {
        if (GameUIManager.Instance == null)
            return;
        text = GetComponent<Text>();
        if (Util.isVietnamese)
        {
            text.font = GameUIManager.FontVietnamese;
            text.alignment = TextAnchor.UpperCenter;
            text.text = DataManager.LanguegesAsset.GetName(text.text);
        }
        else
        {
            text.font = GameUIManager.FontEnglish;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = DataManager.LanguegesAsset.GetName(text.text);
        }
    }
}
