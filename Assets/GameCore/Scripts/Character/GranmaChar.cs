using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranmaChar : MonoBehaviour
{
    [SerializeField] SkeletonAnimation anim;
    [SerializeField] MeshRenderer mesh;
    [SerializeField] float timeDelayAnim = 5;

    private void Start()
    {
        mesh.sortingOrder = (int)(transform.position.y * -100);
        StartCoroutine(DelayAnim());
    }
    IEnumerator DelayAnim()
    {
        anim.AnimationName = "idle";
        yield return new WaitForSeconds(Random.Range(3, timeDelayAnim));
        anim.AnimationName = "vaytay";
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        StartCoroutine(DelayAnim());
    }
}
