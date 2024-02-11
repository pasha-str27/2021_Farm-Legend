using UnityEngine;
using Spine.Unity;
using System.Collections;

public class MNG2_Enemy : MonoBehaviour
{
    [SerializeField] SkeletonAnimation skeleton;
    [SerializeField] string nameAnimation;
    [SerializeField] ParticleSystem khoiDen;

    private void Start()
    {
        if (skeleton != null)
        {
            skeleton.AnimationName = nameAnimation;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.PostEvent((int)EventID.OnDeath, this);
        }
        else if (collision.CompareTag("da"))
        {
            if (skeleton != null)
            {
                skeleton.loop = false;
                skeleton.AnimationName = "idle2";
            }
            SoundManager.Play("die_boss");
            if (khoiDen != null)
            {
                khoiDen.gameObject.SetActive(true);
            }
            Destroy(GetComponent<BoxCollider2D>());
            StartCoroutine(ChoMotChutRoiXoa(collision.gameObject, 0.75f));
        }
        else if (collision.CompareTag("duiga"))
        {
            skeleton.AnimationName = "idle1";
            skeleton.loop = true;
            Destroy(GetComponent<BoxCollider2D>());
            StartCoroutine(ChoMotChutRoiXoa(collision.gameObject, 0.1f));
        }
    }

    IEnumerator ChoMotChutRoiXoa(GameObject _gameObject, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(_gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("duiga"))
        {
            skeleton.AnimationName = "idle1";
            skeleton.loop = true;
            Destroy(GetComponent<BoxCollider2D>());
            Destroy(GetComponent<Rigidbody2D>());
            StartCoroutine(ChoMotChutRoiXoa(collision.gameObject, 0.1f));
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            this.PostEvent((int)EventID.OnDeath);
        }
    }
}
