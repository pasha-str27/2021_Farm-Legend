﻿using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UILoadGame : MonoBehaviour
{
    [SerializeField]
    private Text statusLabel = null;
    [SerializeField]
    private Text percentLabel = null;

    [Header("Icon logo")]
    [SerializeField] Image logo;
    [SerializeField] Sprite logo_android;
    [SerializeField] Sprite logo_ios;

    [Header("Circle")]
    [SerializeField]
    private Image imageTimer = null;

    [Header("Slider")]
    [SerializeField]
    private Slider processSlider = null;

    [SerializeField]
    private UIAnimation anim = null;
    public static UILoadGame instance = null;
    public static float currentProcess;
    public static float lastProcess;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
#if UNITY_ANDROID
        logo.sprite = logo_android;

#elif UNITY_IOS
        logo.sprite = logo_ios;
#endif
    }
    public static void Process(float start = 0, float end = 1, float process = -1, string status = "")
    {
        if (string.IsNullOrEmpty(status))
            status = "Processing... please wait!";

        if (process == -1)
        {
            currentProcess += 0.03f;
        }
        else
        {
            //0.05 -> 0.7 -> 0.7-> 0.8 -> 1.0
            currentProcess = start + (end - start) * process;
        }

        if (currentProcess >= 0)
        {
            if (instance.imageTimer)
                instance.imageTimer.fillAmount = currentProcess;
            if (instance.percentLabel && Mathf.FloorToInt(currentProcess * 100) < 100)
                instance.percentLabel.text = Mathf.FloorToInt(currentProcess * 100).ToString("F0");

            if (instance.statusLabel)
            {
#if UNITY_EDITOR
                //Debug.Log(currentProcess.ToString("#0.0") + " " + process);
#else
                instance.statusLabel.text = status;
#endif
            }
            if (instance.processSlider)
                instance.processSlider.value = currentProcess;
        }
    }

    public static string Status
    {
        set
        {
            if (instance.statusLabel)
                instance.statusLabel.text = value;
        }
    }

    public static void Init(bool show, TweenCallback actionOnDone)
    {
        if (!show)
        {
            instance.anim.Hide(actionOnDone);
        }
        else
        {
            instance.transform.DOScale(1f, .25f);
            ResetView();
            instance.anim.Show(null, actionOnDone);
        }
    }

    public static void ResetView()
    {
        currentProcess = 0;
        lastProcess = 0;
        if (instance.imageTimer)
            instance.imageTimer.fillAmount = 0;
        if (instance.percentLabel)
            instance.percentLabel.text = "0";
        if (instance.statusLabel)
            instance.statusLabel.text = "";
        if (instance.processSlider)
            instance.processSlider.value = 0;
    }

    public static IEnumerator DoRollBack(Action actionOnDone, bool autoHide = true, string status = "")
    {
        while (currentProcess > 0)
        {
            currentProcess -= Math.Max(currentProcess * 0.1f, 0.01f);

            if (currentProcess * 100 >= 0)
            {
                if (instance.imageTimer)
                    instance.imageTimer.fillAmount = currentProcess;
                if (instance.percentLabel)
                    instance.percentLabel.text = (currentProcess * 100).ToString("F0");
                if (instance.statusLabel && !string.IsNullOrEmpty(status))
                    instance.statusLabel.text = status;
                if (instance.processSlider)
                    instance.processSlider.value = currentProcess;
            }
            else
            {
                if (instance.percentLabel)
                    instance.percentLabel.text = "!?";
            }

            yield return null;
        }

        if (instance.statusLabel)
            instance.statusLabel.text = status;

        if (autoHide)
            Init(false, () => { actionOnDone?.Invoke(); });
    }

    public static void Hide()
    {
        instance.anim.Hide();
    }
}