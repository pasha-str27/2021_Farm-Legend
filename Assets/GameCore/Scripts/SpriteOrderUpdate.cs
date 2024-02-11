using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderUpdate : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] spr;
    Vector3 oldPos;
    private void Start()
    {
        oldPos = transform.position;
    }
    private void Update()
    {
        if (transform.localPosition != oldPos)
        {
            oldPos = transform.position;
            for (int i = 0; i < spr.Length; i++)
            {
                spr[i].sortingOrder = (int)(transform.position.y * -100) + i;
            }
        }
    }
}
