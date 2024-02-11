using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_FollowPlayer : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] SkeletonAnimation[] skeletons;
    [SerializeField] MeshRenderer[] meshs;
    [SerializeField] float distanceStop;
    [SerializeField] Transform target;
    [SerializeField] ParticleSystem smoke;
    SkeletonAnimation skeleton;
    MeshRenderer mesh;
    Vector3 oldPos;
    int rd;
    private void Start()
    {
        speed = GameManagerMiniGame.Speed;
        distanceStop = GameManagerMiniGame.DistanceStop;
        ActiveRandomAnimal();
        skeleton.AnimationName = "idle";
    }
    void ActiveRandomAnimal()
    {
        rd = UnityEngine.Random.Range(0, skeletons.Length);
        for (int i = 0; i < skeletons.Length; i++)
        {
            skeletons[i].gameObject.SetActive(false);
        }
        skeletons[rd].gameObject.SetActive(true);
        skeleton = skeletons[rd];
        mesh = meshs[rd];
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
    }

    private void OnCompleteKeyHandle(object obj)
    {
        var msg = (MessagerKeyHandle)obj;
        if (msg.nameObjectAction.Equals(name))
            SetTarget();
    }

    public void SetTarget()
    {
        target = GameManagerMiniGame.Player.transform;
        skeleton.AnimationName = "go";
    }
    void LateUpdate()
    {
        if(target != null)
        {
            if (Vector2.Distance(transform.position, target.position) >= distanceStop)
            {
                if (skeleton.AnimationName != "go")
                {
                    skeleton.AnimationName = "go";
                    PlayEffectSmoke();
                }
                transform.position = Vector2.MoveTowards(transform.position, target.position+new Vector3(0,-0.5f,0), speed * Time.deltaTime);
                mesh.sortingOrder = (int)(transform.position.y + 102);
            }
            else
            if (Vector2.Distance(transform.position, target.position) < distanceStop)
            {
                if (skeleton.AnimationName != "idle")
                {
                    skeleton.AnimationName = "idle";
                    PlayEffectSmoke();
                }
            }
        }
    }
    void PlayEffectSmoke()
    {
        if (skeleton.AnimationName == "go")
            smoke.Play();
        else smoke.Stop();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "enemy":
                this.PostEvent((int)EventID.OnGameLost);
                break;
        }

    }
}
