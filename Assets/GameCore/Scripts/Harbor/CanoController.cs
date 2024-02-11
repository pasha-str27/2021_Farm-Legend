using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanoController : MonoBehaviour
{
    [SerializeField] Animator anim;
    [Header("Move")]
    [SerializeField] float minTimeDelay=10;
    [SerializeField] float maxTimeDelay=60;
    [SerializeField] DOTweenPath path;
    [SerializeField] GameObject objGo;
    [SerializeField] GameObject objBack;
    void Start()
    {
        if(anim != null)
            anim.speed = Random.Range(.8f, 1.2f);

        Go();
    }
    void Go()
    {
        if (path != null)
        {
            objGo.SetActive(true);
            objBack.SetActive(false);
            path.DORestart();
            path.DOPlay();
        }
    }
    public void Back()
    {
        if (path != null)
        {
            objGo.SetActive(false);
            objBack.SetActive(true);
            path.DOPlayBackwards();
        }
    }
    public void DelayRepeat()
    {
        Invoke("Go", Random.Range(minTimeDelay, maxTimeDelay));
    }
}
