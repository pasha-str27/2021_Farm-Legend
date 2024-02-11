using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguagesFontNumber : MonoBehaviour
{
    [SerializeField] bool isNotChangAlignment;
    [ReadOnly] [SerializeField] Text text;
    private void OnEnable()
    {
        if (GameUIManager.Instance == null)
            return;
        text = GetComponent<Text>();
        if (Util.isVietnamese)
        {
            text.font = GameUIManager.FontVietnamese;
            if (!isNotChangAlignment)
                text.alignment = TextAnchor.UpperCenter;
        }
        else
        {
            text.font = GameUIManager.FontEnglish;
            if (!isNotChangAlignment)
                text.alignment = TextAnchor.MiddleCenter;
        }
    }
}
