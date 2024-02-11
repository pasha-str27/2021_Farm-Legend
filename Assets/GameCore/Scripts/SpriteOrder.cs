using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrder : MonoBehaviour
{
    [Tooltip("offSetY object big")]
    [SerializeField] float offSetY = 0;
    [SerializeField] SpriteRenderer[] sprites;
    [Header("Cage")]
    [SerializeField] Cage[] cagesOder;
    [ReadOnly] [SerializeField] float tempY = 0;
    string nameLayer = "Default";
    float posY;
    bool isDecoMap = false;
    private void Start()
    {
        if (sprites.Length == 0)
        {
            sprites = new SpriteRenderer[1];
            sprites[0] = GetComponent<SpriteRenderer>();
            isDecoMap = true;
        }
        LoadOrder(nameLayer);
    }

    public void LoadOrder(string nameLayer)
    {
        posY = transform.position.y - offSetY + (isDecoMap ? 100 : 0);
        if (posY == 0)
            posY = 0.01f;
        tempY = posY * 100;
        if (tempY > 0)
        {
            for (int i = sprites.Length - 1; i >= 0; i--)
            {
                if(!sprites[i].name.Equals("bg_thuhoach") && !sprites[i].name.Equals("icon_Product"))
                    sprites[i].sortingLayerName = nameLayer;
                sprites[i].sortingOrder = (int)(tempY * -1) + i;
            }
            if (cagesOder == null)
                return;
            for (int i = cagesOder.Length - 1; i >= 0; i--)
            {
                cagesOder[i].ReloadOrder(nameLayer, (int)(tempY * -1) + i);
            }
            if (cagesOder.Length > 0 && sprites.Length >= 2)
                sprites[sprites.Length - 1].sortingOrder = (int)(tempY * -1) + cagesOder.Length;
        }
        else
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (!sprites[i].name.Equals("bg_thuhoach") && !sprites[i].name.Equals("icon_Product"))
                    sprites[i].sortingLayerName = nameLayer;
                sprites[i].sortingOrder = (int)(tempY * -1) + i;
            }
            if (cagesOder == null)
                return;
            for (int i = 0; i < cagesOder.Length; i++)
            {
                cagesOder[i].ReloadOrder(nameLayer, (int)(tempY * -1) + i);
            }
            if (cagesOder.Length > 0 && sprites.Length >= 2)
                sprites[sprites.Length - 1].sortingOrder = (int)(tempY * -1) + cagesOder.Length;
        }
    }

    public float GetTempY()
    {
        return tempY;
    }
}
