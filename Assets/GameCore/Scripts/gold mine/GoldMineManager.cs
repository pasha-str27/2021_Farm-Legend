using DG.Tweening;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoldMineManager : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] CountDownTime countDownTime;
    [SerializeField] GameObject objLock;
    [SerializeField] GameObject objActive;
    [SerializeField] ParticleSystem fxExpoit;
    [SerializeField] ItemProductComplite[] itemComplites;
    [SerializeField] string[] nameProducts;
    [SerializeField] string KEY_COUNT_DOWN = "gold_mine";
    [ReadOnly] [SerializeField] List<ProductData> productsHarvest;

    [SerializeField] Transform childScale;
    [SerializeField] float scaleTime = .3f;
    [SerializeField] Vector3 vtScale = new Vector3(.95f, .95f, 1);
    [SerializeField] bool sacleWhenClick;

    [SerializeField] GameObject[] partical;
    float mouseTime;
    Vector3 vitricu;
    private void Start()
    {
        ActivePartical(false);
        ActiveComplite(false);
        Invoke("DelayLoad", 2.5f);
    }
    void DelayLoad()
    {
        if (PlayerPrefSave.Level >= DataManager.GameConfig.LevelUnlockGoldMine)
        {
            Active(true);
            if (isNewUnlock)
            {
                isNewUnlock = false;

                DataManager.ProductAsset.SetBaseMaterial("Gunpowder", DataManager.GameConfig.numMaterialGoldmine);
                DataManager.ProductAsset.SetBaseMaterial("Hammer", DataManager.GameConfig.numMaterialGoldmine);
                DataManager.ProductAsset.SetBaseMaterial("Gunpowder", DataManager.GameConfig.numMaterialGoldmine);
                ActivePartical(true);
                this.PostEvent((int)EventID.OnClickObject, new MessageObject
                {
                    pos = childScale.position,
                    callBack = () => { this.PostEvent((int)EventID.OnShowHandTutorial, true); }
                });
            }

        }
        else
            Active(false);

    }

    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnExploitGoldMine, OnExploitGoldMineHandle);
        this.RegisterListener((int)EventID.OnHarvestGoldMine, OnHarvestGoldMineHandle);
        this.RegisterListener((int)EventID.OnSpeedUp, OnSpeedUpHandle);
        this.RegisterListener((int)EventID.OnClickObject, OnClickObjectHandle);
        this.RegisterListener((int)EventID.OnHidePopupLevelUp, OnHidePopupLevelUpHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnExploitGoldMine, OnExploitGoldMineHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnHarvestGoldMine, OnHarvestGoldMineHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnSpeedUp, OnSpeedUpHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnClickObject, OnClickObjectHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnHidePopupLevelUp, OnHidePopupLevelUpHandle);
    }

    private void OnHidePopupLevelUpHandle(object obj)
    {
        if (GameUIManager.BuildMarketing == BuildMarketing.Farm)
            DelayLoad();
    }

    private void OnClickObjectHandle(object obj)
    {
        var msg = (MessageObject)obj;
        if (msg.type == ObjectMouseDown.GoldMine)
            this.PostEvent((int)EventID.OnShowHandTutorial, false);
    }
    private void OnSpeedUpHandle(object obj)
    {
        var msg = (MessagerCountDown)obj;
        if (msg.keyId != KEY_COUNT_DOWN)
            return;
        this.PostEvent((int)EventID.OnFxPutDiamond, new MessageFx { pos = childScale.position });
        countDownTime.isComplete = 1;
        countDownTime.timeLife = 0;
        countDownTime.Init(KEY_COUNT_DOWN, DataManager.GameConfig.timeGoldMine);
        isExploiting = false;
    }

    private void OnHarvestGoldMineHandle(object obj)
    {
        if (countDownTime.isComplete == 1)
        {
            HarvestProduct();
        }
    }

    private void OnExploitGoldMineHandle(object obj)
    {
        //active
        Debug.Log("=> OnExploitGoldMineHandle");
        anim.SetBool("exploit", true);
        isExploiting = true;
        countDownTime.Init(KEY_COUNT_DOWN, DataManager.GameConfig.timeGoldMine);
    }

    public void EventCoundown()
    {
        this.PostEvent((int)EventID.OnUpdateProgress, new MessagerCountDown { keyId = KEY_COUNT_DOWN, timeLife = countDownTime.timeLife });
        if (countDownTime.timeLife <= 0)
        {
            anim.SetBool("exploit", false);
            countDownTime.isComplete = 1;
            ActiveProduct();
            isExploiting = false;
        }
        else
        {
            anim.SetBool("exploit", true);
        }
    }
    void ActiveProduct()
    {
        List<string> tempList = new List<string>();
        for (int i = 0; i < nameProducts.Length; i++)
        {
            tempList.Add(nameProducts[i]);
        }
        int rd = 0;
        ProductData product = null;
        productsHarvest = new List<ProductData>();

        for (int i = 0; i < itemComplites.Length; i++)
        {
            rd = UnityEngine.Random.Range(0, tempList.Count);
            product = DataManager.ProductAsset.GetProductByName(tempList[rd]);
            itemComplites[i].gameObject.SetActive(true);
            itemComplites[i].FillData(product.icon, 0);
            productsHarvest.Add(product);
            tempList.RemoveAt(rd);
        }
    }
    void ActiveComplite(bool isActive)
    {
        for (int i = 0; i < itemComplites.Length; i++)
        {
            itemComplites[i].gameObject.SetActive(isActive);
        }
    }
    void ActivePartical(bool isActive)
    {
        for (int i = 0; i < partical.Length; i++)
        {
            partical[i].SetActive(isActive);
        }
    }
    void HarvestProduct()
    {
        countDownTime.isComplete = 0;
        countDownTime.timeLife = 0;
        for (int i = 0; i < productsHarvest.Count; i++)
        {
            productsHarvest[i].total++;
            this.PostEvent((int)EventID.OnFxPutIn, new MessageFx { data = productsHarvest[i], typePut = TypePut.Collect, pos = childScale.position });
            CoinManager.AddExp(productsHarvest[i].exp, childScale);
        }
        ActiveComplite(false);
    }
    void Active(bool isActive)
    {
        objLock.SetActive(!isActive);
        objActive.SetActive(isActive);
        if (isActive)
        {
            countDownTime.SetKey(KEY_COUNT_DOWN);
            if (isExploiting)
                countDownTime.Init(KEY_COUNT_DOWN, DataManager.GameConfig.timeGoldMine);
        }
    }
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
            if (EventSystem.current.IsPointerOverGameObject() == false && 
                Vector3.Distance(vitricu, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.2f
                && Time.time - mouseTime < 0.2f)
            {
                if (sacleWhenClick)
                {
                    StartCoroutine(DoScale());
                }
                this.PostEvent((int)EventID.OnClickObject, new MessageObject
                {
                    type = ObjectMouseDown.GoldMine,
                    pos = childScale.position,
                    timeCount = countDownTime.timeLife,
                    isHarvest = countDownTime.isComplete == 1,
                    name = DataManager.LanguegesAsset.GetName("Mining Mines"),
                    nameKey = KEY_COUNT_DOWN,
                });
                Util.objClick = gameObject;
            }
        }
    }

    IEnumerator DoScale()
    {
        childScale.DOScale(vtScale, scaleTime / 2);
        yield return new WaitForSeconds(.1f);
        childScale.DOScale(Vector3.one, scaleTime);
    }
    public void EventAnimFxExploit()
    {
        fxExpoit.Play();
    }
    bool isNewUnlock
    {
        get { return PlayerPrefs.GetInt("isNewUnlock_goldMine", 0) == 0; }
        set { PlayerPrefs.SetInt("isNewUnlock_goldMine", value == true ? 0 : 1); }
    }
    bool isExploiting
    {
        get { return PlayerPrefs.GetInt("isExploiting", 0) == 1; }
        set { PlayerPrefs.SetInt("isExploiting", value == true ? 1 : 0); }
    }
}
