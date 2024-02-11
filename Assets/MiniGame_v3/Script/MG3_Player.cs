using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using MyBox;
using System;

public class MG3_Player : MonoBehaviour
{
    [SerializeField] float runSpeed;
    [SerializeField] SkeletonAnimation[] skeletons;
    [SerializeField] MeshRenderer[] meshs;
    [SerializeField] string[] sounds;
    [SerializeField] GameObject[] skinPlayers;
    [SerializeField] List<Transform> listMove;
    [SerializeField] ParticleSystem smoke;

    SkeletonAnimation skeleton;
    MeshRenderer mesh;
    Vector3 oldPos;
    Vector3 posStart;
    Vector3 posEnd;
    Transform posNext;
    float progress = 0;
    public bool isMove;
    bool useSkin;
    int tempIndex = 0;
    float step;
    int rd;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
        this.RegisterListener((int)EventID.OnGameWin, OnGameWinHandle);
        this.RegisterListener((int)EventID.OnGameLost, OnGameWinHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnGameWin, OnGameWinHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnGameLost, OnGameWinHandle);
    }

    private void OnGameWinHandle(object obj)
    {
        Idle(true);
    }

    void ActiveRandomAnimal()
    {
        rd = UnityEngine.Random.Range(0,skeletons.Length);
        for (int i = 0; i < skeletons.Length; i++)
        {
            skeletons[i].gameObject.SetActive(false);
        }
        skeletons[rd].gameObject.SetActive(true);
        skeleton = skeletons[rd];
        mesh = meshs[rd];
    }
    private void OnCompleteKeyHandle(object obj)
    {
        var msg = (MessagerKeyHandle)obj;
        if (msg.posPlayerMoveTo == null)
            return;
        posNext = msg.posPlayerMoveTo;
    }

    public void Init(MG3_SkinPlayer[] skinPlayers, List<Transform> listMove, float speed, Vector3 pos)
    {
        ActiveRandomAnimal();
        this.runSpeed = speed;
        transform.position = pos;
        posStart = pos;
        oldPos = pos;
        this.listMove = listMove;
        this.posEnd = listMove[listMove.Count - 1].position;
        CameraFollow.instance.SetTarget(transform, posEnd);
        useSkin = false;
        tempIndex = 0;
        posNext = listMove[tempIndex];
        Idle(false);
        ActiveSkin(skinPlayers);
    }
    void ActiveSkin(MG3_SkinPlayer[] skinPlayers)
    {
        for (int i = 0; i < skinPlayers.Length; i++)
        {
            this.skinPlayers[i].SetActive(false);
        }
        for (int i = 0; i < skinPlayers.Length; i++)
        {
            this.skinPlayers[(int)skinPlayers[i]].SetActive(true);
            if (skinPlayers[i] == MG3_SkinPlayer.raft)
            {
                skeleton.AnimationName = "idle";
                useSkin = true;
                PlayEffectSmoke(); ;
            }
        }
    }
    void Idle(bool isIdle)
    {
        skeleton.AnimationName = (isIdle || useSkin) ? "idle" : "go";
        isMove = !isIdle;
        PlayEffectSmoke();
        SoundManager.Play(sounds[rd]);
    }
    void PlayEffectSmoke()
    {
        if (skeleton.AnimationName == "go")
            smoke.Play();
        else smoke.Stop();
    }
    private void Update()
    {
        if (isMove)
        {
            if (transform.position == posNext.position)
            {
                tempIndex++;
                if (tempIndex < listMove.Count)
                    posNext = listMove[tempIndex];
                MG3_StopHandle mG3_Stop = posNext.GetComponent<MG3_StopHandle>();
                if (mG3_Stop != null)
                {
                    if (mG3_Stop.nextPosMove != null)
                    {
                        posNext = mG3_Stop.nextPosMove;
                    }
                }
            }
            step = runSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, posNext.position, step);
            if (transform.position.x >= posEnd.x)
            {
                this.PostEvent((int)EventID.OnGameWin);
                Idle(true);
                this.PostEvent((int)EventID.OnStopHandle, true);
            }

            if (transform.position != oldPos)
            {
                mesh.sortingOrder = (int)(transform.position.y + 100);
            }
        }
    }

    private void LateUpdate()
    {
        if (transform.position != oldPos)
        {
            oldPos = transform.position;
            progress = Vector2.Distance(transform.position, posEnd) / Vector2.Distance(posStart, posEnd);
            this.PostEvent((int)EventID.OnUpdateProgressG3, 1 - progress);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("=> OnTriggerEnter2D " + collision.tag);
        switch (collision.tag)
        {
            case "stop":
                Idle(true);
                GameManagerMiniGame.instance.ShowObjTutorial(true);
                this.PostEvent((int)EventID.OnStopHandle, true);
                if (collision.name.Equals("StopHanlde_disable_raft"))
                {
                    for (int i = 0; i < this.skinPlayers.Length; i++)
                    {
                        this.skinPlayers[i].SetActive(false);
                    }
                    this.PostEvent((int)EventID.OnCompleteKeyHandle, new MessagerKeyHandle { nameObjectAction = "StopHanlde_disable_raft" });
                    useSkin = false;
                    Idle(false);
                }
                break;
            case "enemy":
            case "da":
            case "muikhoan":
            case "duiga":
                SoundManager.Play("attack");
                this.PostEvent((int)EventID.OnStopHandle, true);
                this.PostEvent((int)EventID.OnGameLost);
                break;
            case "draft":
                MG3_SkinPlayer[] skinPlayers = new MG3_SkinPlayer[1];
                skinPlayers[0] = MG3_SkinPlayer.raft;
                ActiveSkin(skinPlayers);
                collision.gameObject.SetActive(false);
                break;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "stop":
                Idle(false);
                this.PostEvent((int)EventID.OnStopHandle, false);
                break;
        }
    }
}
