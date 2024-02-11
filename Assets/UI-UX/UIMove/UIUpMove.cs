using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIUpMove : MonoBehaviour
{
    MessagerUpMove msg;
    public void Init(MessagerUpMove msg)
    {
        this.msg = msg;
        transform.position = msg.pos;
    }
    public void EventStartMove()
    {
        this.PostEvent((int)EventID.OnCompliteUpMove, msg);
        transform.Recycle();
    }
}

public class MessagerUpMove
{
    public int idBuilding;
    public Vector3 pos;
}
