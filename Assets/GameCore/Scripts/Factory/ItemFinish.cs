using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFinish : MonoBehaviour
{
    [SerializeField] SpriteRenderer spr;
    public void Show(ProductData data)
    {
        gameObject.SetActive(true);
        spr.sprite = data.icon;
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
