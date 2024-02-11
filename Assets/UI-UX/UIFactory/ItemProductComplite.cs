using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProductComplite : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprIcon;
    int idFactory;
    public void FillData(Sprite icon, int idFactory)
    {
        this.idFactory = idFactory;
        sprIcon.sprite = icon;
    }

    private void OnMouseDown()
    {
        this.PostEvent((int)EventID.OnThuHoachFactory, idFactory);
    }
}
