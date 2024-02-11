using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaManager : MonoBehaviour
{
    [SerializeField] GameObject[] fxWWarter;
    [SerializeField] Camera camera;
    [SerializeField] float targetY = -27;
    [SerializeField] float targetX = -10;
    [SerializeField] float timeDelayLoop = 1.5f;
    Coroutine coroutine;
    bool isActive = false;
    private void Start()
    {
        camera = Camera.main;
        ActiveFxWarter(false);
    }
    void ActiveFxWarter(bool isActive)
    {
        this.isActive = isActive;
        for (int i = 0; i < fxWWarter.Length; i++)
        {
            fxWWarter[i].SetActive(isActive);
        }
    }
    private void Update()
    {
        if (camera != null)
        {
            if (camera.transform.position.y <= targetY && camera.transform.position.x <= targetX)
            {
                if (!isActive)
                {
                    ActiveFxWarter(true);
                    StopCoroutine();
                    coroutine = StartCoroutine(LoopSound());
                }
            }
            else
            {
                if (isActive)
                {
                    ActiveFxWarter(false);
                    StopCoroutine();
                }
            }
        }
    }
    void StopCoroutine()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
    IEnumerator LoopSound()
    {
        SoundManager.PlayFade("SingleWaveLoop");
        yield return new WaitForSeconds(timeDelayLoop);
        coroutine = StartCoroutine(LoopSound());
    }
}
