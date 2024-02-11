using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MyBox;

public class GranpaChar : MonoBehaviour
{
    [SerializeField] SkeletonAnimation anim;
    [SerializeField] MeshRenderer mesh;
    [SerializeField] float timeDelayMove = 3;
    [Header("Are move")]
    [SerializeField] int minX;
    [SerializeField] int maxX, minY, maxY;
    [ReadOnly] [SerializeField] int areaXCurrent, areaYCurrent;
    [ReadOnly] [SerializeField] int areaX, areaY;
    Vector3 tempPos;
    Vector3 oldPos;

    Sequence sequence;
    private void Start()
    {
        //x:10;12/ y:-3;1
        anim.AnimationName = "idle";
        areaXCurrent = 11;
        areaYCurrent = 0;
        oldPos = transform.position;
        StartCoroutine(DelayMove());
    }
    IEnumerator DelayMove()
    {
        yield return new WaitForSeconds(Random.Range(1f,3f));
        anim.AnimationName = "idle";
        yield return new WaitForSeconds(Random.Range(timeDelayMove, timeDelayMove));
        ChangeStage();
    }
    void ChangeStage()
    {
        int rd = Random.Range(0, 3);
        //Debug.Log("=> ChangeStage " + rd);
        anim.timeScale = 1f;
        anim.AnimationName = "idle";
        switch (rd)
        {
            case 0:
                StartCoroutine(DelayMove());
                break;
            case 1:
                anim.AnimationName = "vaytay";
                StartCoroutine(DelayMove());
                break;
            case 2:
                areaX = Random.Range(minX, maxX);
                areaY = Random.Range(minY, maxY);
                anim.AnimationName = "go";
                anim.timeScale = 1.5f;
                //Debug.Log("=> ChangeStage go (" + areaXCurrent + ","+ areaYCurrent + ")=>(" + areaX+", "+ areaY + ")");
                Vector3 posTo = GridBuildingSystem.instance.GetPosCell(areaX, areaY);
                //rotate
                if (areaX > areaXCurrent)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 0);
                    if(areaY > areaYCurrent)
                        transform.localEulerAngles = new Vector3(0, 180, 0);
                }
                else
                if (areaX < areaXCurrent)
                {
                    transform.localEulerAngles = new Vector3(0, 180, 0);
                    if (areaY < areaYCurrent)
                        transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    if(areaY<areaYCurrent)
                        transform.localEulerAngles = new Vector3(0, 0, 0);
                    else
                        transform.localEulerAngles = new Vector3(0, 180, 0);
                }
                areaXCurrent = areaX;
                areaYCurrent = areaY;
                //transform.LookAt(posTo);
                sequence = DOTween.Sequence();
                sequence.Append(transform.DOMove(posTo, Vector3.Distance(transform.position, posTo) * 2).OnComplete(() =>
                {
                    anim.timeScale = 1f;
                    anim.AnimationName = "idle";
                    StartCoroutine(DelayMove());
                }));
                break;
        }
    }
    
    private void Update()
    {
        if(transform.localPosition != oldPos)
        {
            mesh.sortingOrder = (int)(transform.position.y * -100);
        }
    }
    private void OnDestroy()
    {
        sequence.Kill();
    }
}
