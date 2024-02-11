using UnityEngine;
using Spine.Unity;
using System.Collections;

public class MNG2_Dragon : MonoBehaviour
{
    [SerializeField] SkeletonAnimation skeleton;
    [SerializeField] ParticleSystem effectLua;
    [SerializeField] ParticleSystem effectKhoiden;

    private void Start()
    {
        skeleton.AnimationName = "phunlua";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.PostEvent((int)EventID.OnDeath);
        }
        else if (collision.CompareTag("dan"))
        {
            skeleton.loop = false;
            skeleton.AnimationName = "die";
            effectKhoiden.gameObject.SetActive(true);
            SoundManager.Play("die_boss");
            Destroy(gameObject, 0.4f);
        }
    }

    public void FunLua()
    {
        skeleton.AnimationName = "idle";
        StartCoroutine(Cho1Ty());
    }

    IEnumerator Cho1Ty()
    {
        effectLua.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        effectLua.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        effectLua.gameObject.SetActive(true);
        this.PostEvent((int)EventID.OnDeath);
        effectLua.gameObject.SetActive(false);
        skeleton.AnimationName = "phunlua";
    }
}
