using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using System;
using UnityEngine.SceneManagement;

public class GameCoreManager : GameManagerBase<GameCoreManager>
{
    public static GameCoreManager Instance => instance;
    [SerializeField] Transform tfTutorialCage;
    [SerializeField] Transform tfTutorialFactory;
    [SerializeField] Transform tfTutorialLand;
    [SerializeField] Transform posHarbor;
    [SerializeField] Transform posHome;
    
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle);
        this.RegisterListener((int)EventID.OnViewLocationHarbor, OnViewLocationHarborHandle);
        this.RegisterListener((int)EventID.OnViewLocationHome, OnViewLocationHomeHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnViewLocationHarbor, OnViewLocationHarborHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnViewLocationHome, OnViewLocationHomeHandle);
    }

    private void OnViewLocationHomeHandle(object obj)
    {
        this.PostEvent((int)EventID.OnClickObject, new MessageObject
        {
            pos = posHome.position,
        });
    }

    private void OnViewLocationHarborHandle(object obj)
    {
        this.PostEvent((int)EventID.OnClickObject, new MessageObject
        {
            pos = posHarbor.position,
        });
    }

    private void OnViewCamTutorialHandle(object obj)
    {
        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 1)
        {
            Vector3 temp = tfTutorialCage.position;

            this.PostEvent((int)EventID.OnLockCamera, true);

            //do somthing
            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                pos = temp,
            });
        }
        if (PlayerPrefSave.stepTutorial == 4)
        {
            Vector3 temp = tfTutorialFactory.position;

            this.PostEvent((int)EventID.OnLockCamera, true);

            //do somthing
            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                pos = temp,
            });
        }
        if (PlayerPrefSave.stepTutorial == 6)
        {
            Vector3 temp = tfTutorialLand.position;

            this.PostEvent((int)EventID.OnLockCamera, true);

            //do somthing
            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                pos = temp,
            });
        }
    }
    protected override void Start()
    {
        base.Start();
        Invoke("DelayAdFirtOpen", 2f);
    }

    void DelayAdFirtOpen()
    {
        if (!GameUIManager.instance.IsFirtOpen)
        {
            GameUIManager.instance.IsFirtOpen = true;
            AdsManager.ShowAdOpening();
        }
    }
    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.H))
        {
            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                pos = posHarbor.position,
            });
        }
    }

    private void OnPointerDownHandle(Vector3 p)
    {
        if (GameStateManager.CurrentState == GameState.Pause)
        {
            GameStateManager.Play(null);
            return;
        }
    }
    protected override void LoadMain(object data)
    {
        base.LoadMain(data);

    }
    public override void IdleGame(object data)
    {
        Time.timeScale = 1;
        Debug.Log("Game Core goto IdleGame");
    }

    public override void InitGame(object data)
    {
        Debug.Log("Game Core goto InitGame");
    }

    public override void LoadGame(object data)
    {
       
    }

    public override void NextGame(object data)
    {
        Debug.Log("Game Core goto NextGame");
    }


    float timeScaleAtPause = 0;
    public override void PauseGame(object data)
    {
        Debug.Log("Game Core goto PauseGame");
        timeScaleAtPause = Time.timeScale;
        Time.timeScale = 0;
    }

    public override void PlayGame(object data)
    {
        Debug.Log("Game Core goto PlayGame");
    }

    public override void RestartGame(object data)
    {
        Debug.Log("Game Core goto RestartGame");
    }

    public override void ResumeGame(object data)
    {
        Debug.Log("Game Core goto ResumeGame");
        Time.timeScale = timeScaleAtPause;
    }

    protected override void CompleteGame(object data)
    {
        Debug.Log("Game Core goto CompleteGame");
    }

    protected override void GameOver(object data)
    {
        Debug.Log("Game Core goto GameOver");
    }

    protected override void ReadyGame(object data)
    {
        Debug.Log("Game Core goto ReadyGame");
    }

    protected override void RebornCheckPointGame(object data)
    {
        Debug.Log("Game Core goto RebornCheckPointGame");
    }

    protected override void RebornContinueGame(object data)
    {
        Debug.Log("Game Core goto RebornContinueGame");
    }

    protected override void WaitingGameComplete(object data)
    {
        float timeWaitDie = 2f;
        DOVirtual.DelayedCall(timeWaitDie, () =>
        {
            if (GameStateManager.CurrentState == GameState.WaitComplete)
                GameStateManager.Complete(null);
        }).SetUpdate(false).SetId(this);
    }

    protected override void WaitingGameOver(object data)
    {
        Debug.Log("Game Core goto WaitingGameOver");
    }
    private void OnDestroy()
    {
        Util.timeOut_farm = Util.timeNow;
    }
    private void OnApplicationQuit()
    {
        Util.timeOut_farm = Util.timeNow;
    }
    private void OnApplicationPause(bool pause)
    {
        Util.timeOut_farm = Util.timeNow;
    }
}