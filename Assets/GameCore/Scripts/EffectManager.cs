using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class EffectManager : MonoBehaviour
{
    [SerializeField] FxPool prPanting;
    [SerializeField] FxPool prHarvest;
    [SerializeField] FxPool prMaterial;
    [SerializeField] ParticleSystem particleCollect, prCoinCrop;
    [SerializeField] ParticleSystem prPutDiamond;
    [SerializeField] ParticleSystem prFxDestroy;
    [SerializeField] UIUpMove uiUpMove;

    private void Start()
    {
        prPanting.CreatePool(5);
        prHarvest.CreatePool(5);
        prMaterial.CreatePool(3);
        particleCollect.CreatePool(5);
        prCoinCrop.CreatePool(5);
        prPutDiamond.CreatePool(5);
        uiUpMove.CreatePool(1);
        prFxDestroy.CreatePool(2);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnFxPutIn, OnFxPutInHandle);
        this.RegisterListener((int)EventID.OnFxMaterial, OnFxMaterialHandle);
        this.RegisterListener((int)EventID.OnFxStagesCrops, OnFxStagesCropsHandle);
        this.RegisterListener((int)EventID.OnFxPutCoin, OnFxPutCoinHandle);
        this.RegisterListener((int)EventID.OnFxPutDiamond, OnFxPutDiamondHandle);
        this.RegisterListener((int)EventID.OnShowUiUpMove, OnShowUiUpMoveHandle);
        this.RegisterListener((int)EventID.OnShowFxDestroy, OnShowFxDestroyHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnFxPutIn, OnFxPutInHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnFxMaterial, OnFxMaterialHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnFxStagesCrops, OnFxStagesCropsHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnFxPutCoin, OnFxPutCoinHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnFxPutDiamond, OnFxPutDiamondHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowUiUpMove, OnShowUiUpMoveHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowFxDestroy, OnShowFxDestroyHandle);
    }

    private void OnShowFxDestroyHandle(object obj)
    {
        var msg = (MessageFx)obj;
        ParticleSystem fx1 = prFxDestroy.Spawn(transform);
        fx1.transform.position = msg.pos;
    }

    private void OnShowUiUpMoveHandle(object obj)
    {
        var msg = (MessagerUpMove)obj;
        var temp = uiUpMove.Spawn(transform);
        temp.Init(msg);
    }
    private void OnFxPutDiamondHandle(object obj)
    {
        var msg = (MessageFx)obj;
        ParticleSystem fx1 = null;
        fx1 = prPutDiamond.Spawn(transform);
        fx1.transform.position = msg.pos;
    }

    private void OnFxPutCoinHandle(object obj)
    {
        var msg = (Vector3)obj;
        ParticleSystem fx1 = null;
        fx1 = prCoinCrop.Spawn(transform);
        fx1.transform.position = msg + new Vector3(0, 1f, 0);
    }

    private void OnFxStagesCropsHandle(object obj)
    {
        var msg = (MessageFx)obj;
        ParticleSystem fx1 = particleCollect.Spawn(transform);
        msg.pos.y += .5f;
        fx1.transform.position = msg.pos;
    }

    private void OnFxMaterialHandle(object obj)
    {
        var msg = (MessageFx)obj;
        FxPool fx = prMaterial.Spawn(transform);
        fx.FillData(msg.data.icon, msg.pos);
    }

    private void OnFxPutInHandle(object obj)
    {
        var msg = (MessageFx)obj;
        FxPool fx = null;

        switch (msg.typePut)
        {
            case TypePut.PutIn:
                fx = prPanting.Spawn(transform);
                if (msg.data.price > 0)
                    this.PostEvent((int)EventID.OnFxPutCoin, msg.pos);
                break;
            case TypePut.Collect:
                fx = prHarvest.Spawn(transform);
                break;
        }
        fx.FillData(msg.data.icon, msg.pos);
    }
}

public class MessageFx
{
    public ProductData data;
    public TypePut typePut;
    public Vector3 pos;
}
public enum TypePut
{
    PutIn, Collect
}
