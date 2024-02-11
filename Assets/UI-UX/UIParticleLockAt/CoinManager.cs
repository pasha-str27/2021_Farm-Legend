using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinManager : MonoBehaviour
{
    [SerializeField] Image iconScaleCoin;
    [SerializeField] Image iconScaleDiamond;
    [SerializeField] Image iconScaleExp;
    [SerializeField]
    private UITextNumber numberCoin = null;
    [SerializeField]
    private UITextNumber numberDiamond = null;
    [SerializeField]
    private UITextNumber numberExp = null;
    public static UITextNumber NumberCoin { get => instance?.numberCoin; }
    public static UITextNumber NumberDiamond { get => instance?.numberDiamond; }
    public static UITextNumber NumberExp { get => instance?.numberExp; }

    [SerializeField]
    private ParticleLockAt particleCoin = null;
    public static ParticleLockAt ParticleCoin { get => instance?.particleCoin; }
    [SerializeField]
    private ParticleLockAt particleDiamond = null;
    public static ParticleLockAt ParticleDiamond { get => instance?.particleDiamond; }
    [SerializeField]
    private ParticleLockAt particleExp = null;
    public static ParticleLockAt ParticleExp { get => instance?.particleExp; }
    public Transform defaultTarget;
    public Transform targetCoin;
    public Transform targetDiamond;
    public Transform targetExp;
    public static int totalCoin
    {
        get => PlayerPrefSave.Coin;
        private set => PlayerPrefSave.Coin = value;
    }
    public static int totalDiamond
    {
        get => PlayerPrefSave.Diamond;
        private set => PlayerPrefSave.Diamond = value;
    }
    public static int totalExp
    {
        get => PlayerPrefSave.ExpLevel;
        private set => PlayerPrefSave.ExpLevel =value;
    }
    public static int CoinByAds => DataManager.GameConfig.coinByAds;

    private static CoinManager instance;

    private void Awake()
    {
        instance = this;
        DataManager.OnLoaded += DataManager_OnLoaded;
        GameStateManager.OnStateChanged += GameStateManager_OnStateChanged;
    }

    private void GameStateManager_OnStateChanged(GameState current, GameState last, object data)
    {
        if (current == GameState.Play)
        {
            NumberCoin.DOAnimation(0, totalCoin, 0);
            NumberExp.DOAnimation(0, totalExp, 0);
            NumberDiamond.DOAnimation(0, totalDiamond, 0);
        }
    }

    private void DataManager_OnLoaded(GameData gameData)
    {
        NumberCoin.DOAnimation(0, totalCoin, 0);
        NumberExp.DOAnimation(0, totalExp, 0);
        NumberDiamond.DOAnimation(0, totalDiamond, 0);
    }

    public static void AddCoin(int numb, Transform fromTrans = null, Transform toTrans = null, string ads = null)
    {
        var current = totalCoin;
        totalCoin += numb;

        if (numb > 0)
            SoundManager.Play("sfxClickRewardVideo");
        else
        if (numb < 0 && ads == null)
        {
            SoundManager.Play("sell");
        }
        if (NumberCoin != null)
        {
            if (numb > 0)
            {
                var fx = ParticleCoin.Spawn(instance.transform);
                fx.transform.eulerAngles = Vector3.zero;
                if (ads != null)
                    fx.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Canvas";
                if (fromTrans)
                {
                    fx.Emit(Mathf.Clamp(numb + 1, 0, 10), fromTrans, toTrans ?? instance.targetCoin);
                }
                NumberCoin.DOAnimation(current, totalCoin, fx.StartLifetime, 0, "{0}", () =>
                {
                    instance.iconScaleCoin.DOScale(.25f, 1f, 1.1f, .1f, () =>
                    {
                        instance.iconScaleCoin.DOScale(.1f, 1.1f, 1f, .1f);
                    });
                });
            }
            else
            {
                NumberCoin.DOAnimation(current, totalCoin, 0);
            }
        }
    }
    public static void AddDiamond(int numb, Transform fromTrans = null, Transform toTrans = null, string ads = null)
    {
        var current = totalDiamond;
        totalDiamond += numb;
        if (NumberDiamond != null)
        {
            if (numb > 0)
            {
                var fx = ParticleDiamond.Spawn(instance.transform);
                fx.transform.eulerAngles = Vector3.zero;
                if (ads != null)
                    fx.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Canvas";
                if (fromTrans)
                {
                    fx.Emit(Mathf.Clamp(numb + 1, 0, 10), fromTrans, toTrans ?? instance.targetDiamond);
                }
                NumberDiamond.DOAnimation(current, totalDiamond, fx.StartLifetime, 0, "{0}", () =>
                {
                    instance.iconScaleDiamond.DOScale(.25f, 1f, 1.1f, .1f, () =>
                    {
                        instance.iconScaleDiamond.DOScale(.1f, 1.1f, 1f, .1f);
                    });
                });
            }
            else
            {
                NumberDiamond.DOAnimation(current, totalDiamond, 0);
            }
        }
    }
    public static void AddExp(int numb, Transform fromTrans = null, Transform toTrans = null, string ads = null)
    {
        var current = totalExp;
        totalExp += numb;
        instance.PostEvent((int)EventID.OnUpdateExp);
        if (NumberExp != null)
        {
            var fx = ParticleExp.Spawn(instance.defaultTarget);
            fx.transform.eulerAngles = Vector3.zero;
            if (numb > 0)
            {
                if (ads != null)
                    fx.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Canvas";
                if (fromTrans)
                {
                    fx.Emit(Mathf.Clamp(numb + 1, 0, 10), fromTrans, toTrans ?? instance.targetExp);
                }
                NumberExp.DOAnimation(current, totalExp, fx.StartLifetime, 0, "{0}", () =>
                {
                    instance.iconScaleExp.DOScale(.25f, 1f, 1.1f, .1f, () =>
                    {
                        instance.iconScaleExp.DOScale(.1f, 1.1f, 1f, .1f);
                    });
                });
            }
            else
            {
                NumberExp.DOAnimation(current, totalExp, 0);
            }
        }
    }
    public static void GetByAds(Transform transform, string placement, Action<AdEvent> status = null)
    {
#if USE_IRON || USE_MAX || USE_ADMOB
        //AdsManager.ShowVideoReward((onSuccess) =>
        //{
        //    if (onSuccess == AdEvent.Success)
        //    {
        //        //Add(CoinByAds, transform);
        //        AdsManager.ShowNotice(onSuccess);
        //    }
        //    else
        //    {
        //        AdsManager.ShowNotice(onSuccess);
        //    }
        //    status?.Invoke(onSuccess);
        //}, placement, "coin");
#endif
    }
    public static string CoinByAdsFormat(int fontSize = 16)
    {
        return "<size=" + fontSize + ">+</size>" + CoinByAds;
    }

    public void Ins_GetByAds(Transform transform)
    {
        GetByAds(transform, name);
    }

#if UNITY_EDITOR
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddCoin(1000);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddDiamond(100);
        }
    }
#endif
}
