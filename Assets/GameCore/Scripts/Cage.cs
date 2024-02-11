using UnityEngine;
using System.Collections;
using Spine.Unity;

public class Cage : MonoBehaviour
{
    [SerializeField] GameObject EffectChoAn;
    [SerializeField] GameObject EffectSleep;
    [SerializeField] SkeletonAnimation anim;

    public void ReloadOrder(string nameLayer,int order)
    {
        anim.GetComponent<MeshRenderer>().sortingLayerName = nameLayer;
        anim.GetComponent<MeshRenderer>().sortingOrder = order;
    }
    public void ChoAn()
    {
        
        An();
        EffectSleep.SetActive(false);
        EffectChoAn.SetActive(true);
        StartCoroutine(DeactiveEffectChoAn());
    }

    IEnumerator DeactiveEffectChoAn()
    {
        yield return new WaitForSeconds(3);
        EffectChoAn.SetActive(false);
    }
    public void An()
    {
        anim.timeScale = Random.Range(0.8f, 1.2f);
        if (name.Contains("Chicken"))
            anim.AnimationName = "idle";
        else
            anim.AnimationName = "an";
        EffectSleep.SetActive(false);
    }
    public void Idle()
    {
        anim.timeScale = Random.Range(0.8f, 1.2f);
        anim.AnimationName = "idle";
        EffectSleep.SetActive(false);
    }

    public void Thuhoach()
    {
        anim.timeScale = Random.Range(0.8f, 1.2f);
        anim.AnimationName = "thuhoach";
    }

    public void Doi()
    {
        anim.timeScale = Random.Range(0.8f, 1.2f);
        EffectSleep.SetActive(true);
        anim.AnimationName = "doi";
    }
}
