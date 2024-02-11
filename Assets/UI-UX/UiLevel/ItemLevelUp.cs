using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLevelUp : MonoBehaviour
{
    [SerializeField] Transform tfScale;
    [SerializeField] Image icon;
    public void FillData(Sprite sprite, float scale)
    {
        tfScale.localScale = new Vector3(scale, scale, 1);
        icon.sprite = sprite;
        icon.SetNativeSize();
    }
}
