using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfxMoney : MonoBehaviour
{
    [SerializeField] TextMesh txtMoney;
    [SerializeField] float height = 3;
    [SerializeField] float animTime = 0.5f;
    [SerializeField] float autoHide = 1;

    private Transform tf;
    private void Awake()
    {
        tf = transform;
    }
    public void DoInit(string money, Vector3 pos)
    {
        tf.DOKill();

        txtMoney.text = money;
        tf.localPosition = pos.Multi(1, 1, 0);
        var y = tf.localPosition.y + height;

        tf.DOLocalMoveY(y, animTime).OnComplete(() =>
        {
            tf.DOLocalMoveY(y, autoHide).OnComplete(() =>
            {
                gameObject.Recycle();
            });
        });
    }
}
