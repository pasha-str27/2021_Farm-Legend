using DG.Tweening;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipController : MonoBehaviour
{
    [SerializeField] Animator[] anims;
    [SerializeField] GameObject[] thunghang;
    [SerializeField] GameObject[] rewardShip;
    [SerializeField] GameObject[] iconGiftReward;
    [SerializeField] GameObject objShipGo;
    [SerializeField] GameObject objShipBack;
    private DOTweenPath path;
    [SerializeField] ParticleSystem particle;
    [ReadOnly] [SerializeField] bool IsRun;
    OrderHarborData order;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnShipStar, OnShipStarHandle);
        this.RegisterListener((int)EventID.OnShipBack, OnShipBackHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShipStar, OnShipStarHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShipBack, OnShipBackHandle);
    }

    private void Start()
    {
        path = GetComponent<DOTweenPath>();
        ShipIdle();
        //if (isRun)
        //{
        //    if (isCompliteOrder)
        //        GiftReady();
        //    else
        //        path.DOComplete();
        //}
        //else
        //{
        //    ShipIdle();
        //}
    }

    private void OnShipBackHandle(object obj)
    {
        if (isRun)
        {
            ShipBack();
        }
    }
    private void OnShipStarHandle(object obj)
    {
        var msg = (OrderHarborData)obj;
        if (isCompliteOrder)
        {
            ClaimGift();
        }
        if (!isRun)
        {
            order = msg;
            coinOrder = msg.coin;
            expOrder = msg.exp;

            DataManager.OrderHarborAsset.PustOrder(msg);
            this.PostEvent((int)EventID.OnResetOrderHarbor, msg);
            ActiveGoods(true);
            Invoke("ShipGo", 1f);
            //ShipGo();
        }
        else
        {
            UIToast.Show(DataManager.LanguegesAsset.GetName("Ship is shipping orders!"), null, ToastType.Notification, 1.5f);
        }
    }
    void ActiveGoods(bool isActive)
    {
        for (int i = 0; i < thunghang.Length; i++)
        {
            thunghang[i].SetActive(isActive);
        }
    }
    void ActiveRewardShip(bool isActive)
    {
        if (iconGiftReward == null)
            return;
        for (int i = 0; i < rewardShip.Length; i++)
        {
            rewardShip[i].SetActive(isActive);
        }
    }
    void ActiveIconGift(bool isActive)
    {
        for (int i = 0; i < iconGiftReward.Length; i++)
        {
            iconGiftReward[i].SetActive(isActive);
        }
    }
    public void GiftReady()
    {
        isCompliteOrder = true;
        objShipGo.SetActive(false);
        objShipBack.SetActive(true);
        ActiveIconGift(true);
        PlayAnimGo(false);
    }
    public void Shiping()
    {
        path.DOComplete();
    }
    void ClaimGift()
    {
        Debug.Log("=> ClaimGift ");
        isRun = false;
        isCompliteOrder = false;
        particle.Play();
        ActiveRewardShip(false);
        ShipIdle();
        if (GameUIManager.IsTest)
        {
            CoinManager.AddCoin(10, transform);
            CoinManager.AddExp(10, transform);
            return;
        }
        CoinManager.AddCoin(coinOrder, transform);
        CoinManager.AddExp(expOrder, transform);
    }
    void ShipIdle()
    {
        objShipGo.SetActive(true);
        objShipBack.SetActive(false);
        ActiveGoods(false);
        ActiveIconGift(false);
        ActiveRewardShip(false);
        PlayAnimGo(false);
    }
    void PlayAnimGo(bool isPlay)
    {
        for (int i = 0; i < anims.Length; i++)
        {
            if (anims[i].gameObject.activeInHierarchy)
            {
                anims[i].speed = UnityEngine.Random.Range(.8f, 1.2f);
                anims[i].SetBool("go", isPlay);
            }
        }
    }
    void ShipGo()
    {
        Debug.Log("=> ShipGo ");
        isRun = true;
        path.DORestart();
        objShipGo.SetActive(true);
        objShipBack.SetActive(false);
        ActiveRewardShip(false);
        PlayAnimGo(true);
        this.PostEvent((int)EventID.OnStartCountDownHarbor, order.timeSend);
    }
    void ShipBack()
    {
        path.DOPlayBackwards();
        objShipGo.SetActive(false);
        objShipBack.SetActive(true);
        ActiveGoods(false);
        ActiveRewardShip(true);
        PlayAnimGo(true);
    }

    float mouseTime;
    Vector3 vitricu;
    [SerializeField] Vector2 vtScale = new Vector2(.85f, .85f);
    [SerializeField] float scaleTime = .3f;
    private void OnMouseDown()
    {
        if (!Util.IsMouseOverUI)
        {
            mouseTime = Time.time;
            vitricu = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if (!Util.IsMouseOverUI)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false && Vector3.Distance(vitricu, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.2f
                && Time.time - mouseTime < 0.2f)
            {
                //StartCoroutine(DoScale());
                SoundManager.Play("tap");
                //do somthing
                if (isCompliteOrder)
                {
                    ClaimGift();
                }
                else
                {
                    this.PostEvent((int)EventID.OnClickObject, new MessageObject
                    {
                        type = ObjectMouseDown.Harbor,
                        pos = HarborManager.Instance.harbor.position
                    });
                }
            }
        }
    }
    IEnumerator DoScale()
    {
        transform.DOScale(vtScale, scaleTime / 2);
        yield return new WaitForSeconds(.1f);
        transform.DOScale(Vector2.one, scaleTime);
    }

    #region save

    public bool isRun
    {
        set
        {
            IsRun = value;
            PlayerPrefs.SetInt("isRun_ship"+name, value ? 1 : 0);
        }
        get
        {
            IsRun = PlayerPrefs.GetInt("isRun_ship" + name, 0) == 1;
            return PlayerPrefs.GetInt("isRun_ship" + name, 0) == 1;
        }
    }
    public bool isCompliteOrder
    {
        set
        {
            IsRun = value;
            PlayerPrefs.SetInt("isCompliteOrder_ship" + name, value ? 1 : 0);
        }
        get
        {
            IsRun = PlayerPrefs.GetInt("isCompliteOrder_ship" + name, 0) == 1;
            return PlayerPrefs.GetInt("isCompliteOrder_ship" + name, 0) == 1;
        }
    }
    int coinOrder
    {
        set
        {
            PlayerPrefs.SetInt("coinOrder_ship" + name, value);
        }
        get
        {
            return PlayerPrefs.GetInt("coinOrder_ship" + name, 0);
        }
    }
    int expOrder
    {
        set
        {
            PlayerPrefs.SetInt("expOrder_ship" + name, value);
        }
        get
        {
            return PlayerPrefs.GetInt("expOrder_ship" + name, 0);
        }
    }
    #endregion
}
