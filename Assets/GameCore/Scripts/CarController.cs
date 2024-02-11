using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MyBox;

public class CarController : MonoBehaviour
{
    public GameObject CarGold;
    public GameObject Cargoods;
    public GameObject CarIdle;
    private DOTweenPath path;
    public float timeSendOrder = 15;
    public GameObject[] objectBackDisble;
    [SerializeField] ParticleSystem particle;
    [ReadOnly] [SerializeField] bool IsClick;
    [ReadOnly] [SerializeField] bool IsCarback;
    [ReadOnly] [SerializeField] bool IsRun;

    #region save
    bool isClick { 
        set {
            IsClick = value;
            PlayerPrefs.SetInt("isClick", value ? 1 : 0); }
        get {
            IsClick = PlayerPrefs.GetInt("isClick", 0) == 1;
            return PlayerPrefs.GetInt("isClick", 0) == 1; }
    }
    bool isCarback
    {
        set {
            IsCarback = value;
            PlayerPrefs.SetInt("isCarback", value ? 1 : 0); }
        get {
            IsCarback = PlayerPrefs.GetInt("isCarback", 0) == 1;
            return PlayerPrefs.GetInt("isCarback", 0) == 1; }
    }
    bool isRun
    {
        set {
            IsRun = value;
            PlayerPrefs.SetInt("isRun", value ? 1 : 0); }
        get {
            IsRun = PlayerPrefs.GetInt("isRun", 0) == 1;
            return PlayerPrefs.GetInt("isRun", 0) == 1; }
    }
    int coinOrder
    {
        set
        {
            PlayerPrefs.SetInt("coinOrder", value);
        }
        get
        {
            return PlayerPrefs.GetInt("coinOrder", 0);
        }
    }
    int expOrder
    {
        set
        {
            PlayerPrefs.SetInt("expOrder", value);
        }
        get
        {
            return PlayerPrefs.GetInt("expOrder", 0);
        }
    }
    #endregion
    [ReadOnly] [SerializeField] OrderData orderData;
    private void Start()
    {
        path = GetComponent<DOTweenPath>();
        CarIdle.SetActive(true);
        Cargoods.SetActive(false);
        CarGold.SetActive(false);
        if (isRun || isClick)
        {
            CarGold.SetActive(true);
            CarIdle.SetActive(false);
            TruckGoldEnd();
        }
        //Debug.Log("=> CarController " + isRun+ isClick);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.CarStart, TruckGoods);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.CarStart, TruckGoods);
    }
    /// <summary>
    /// ô tô chở hàng đi bán
    /// </summary>
    /// <param name="obj"></param>
    void TruckGoods(object obj)
    {
        var msg = (OrderData)obj;
        if (!isRun)
        {
            orderData = msg;
            coinOrder = orderData.coin;
            expOrder = orderData.exp;
            if (isClick)
                CarRceiveGold();
            SoundManager.Play("sfxCar");
            isRun = true;
            Cargoods.transform.localScale = new Vector3(-1, 1, 1);
            CarIdle.SetActive(false);
            Cargoods.SetActive(true);
            CarGold.SetActive(false);
            path.DORestart();

            //resset order current
            DataManager.OrderAsset.PustOrder(orderData.name);
            this.PostEvent((int)EventID.OnResetOrder, orderData);
            this.PostEvent((int)EventID.OnLoadUiOrderMap);
            Debug.Log("===============start Car");
            this.PostEvent((int)EventID.OnUpdateBonus);

            this.PostEvent((int)EventID.OnUpdateAchie, 10);

            if (!PlayerPrefSave.IsTutorial)
                return;
            if (PlayerPrefSave.stepTutorial == 9)
            {
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 2:
                        PlayerPrefSave.stepTutorialCurrent = 3;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
            }
        }
        else
        {
            UIToast.Show("Car is shipping orders!", null, ToastType.Notification, 1.5f);
        }
    }
    /// <summary>
    /// khi ô tô chở hàng đến nơi và quay đầu về chở vàng
    /// </summary>
    public void TruckGold()
    {
        CarGold.transform.localScale = new Vector3(-1, 1, 1);
        CarIdle.SetActive(false);
        Cargoods.SetActive(false);
        CarGold.SetActive(true);
        Debug.Log("===============gold Car");
        path.DOPlayBackwards();
        isCarback = true;
    }
    /// <summary>
    /// khi ô tô chở vàng về đến nhà
    /// </summary>
    public void TruckGoldEnd()
    {
        isClick = true;
        isCarback = false;
        isRun = false;

        SoundManager.Play("beep");
        foreach (GameObject obj in objectBackDisble)
            obj.SetActive(false);

        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 4)
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 3:
                    PlayerPrefSave.stepTutorialCurrent = 4;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
    }
    private void OnMouseUp()
    {
        if (isClick)
            CarRceiveGold();
        //click nhận tiền
    }
    /// <summary>
    /// dọn xe cho đỡ chật
    /// </summary>
    public void CarRceiveGold()
    {
        CarIdle.SetActive(true);
        Cargoods.SetActive(false);
        CarGold.SetActive(false);
        isClick = false;
        AddCoin();
        particle.Play();
        RandomMaterial();
        foreach (GameObject obj in objectBackDisble)
            obj.SetActive(true);

        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 4)
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 4:
                    PlayerPrefSave.stepTutorialCurrent = 5;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
    }
    int tempRd = 0;
    int rdIdTem = 0;
    void RandomMaterial()
    {
        tempRd = UnityEngine.Random.Range(0, 101);
        ProductData data = DataManager.ProductAsset.GetProductByName("Rope");
        if (tempRd <= DataManager.GameConfig.percentRandomMaterial)
        {
            rdIdTem = UnityEngine.Random.Range(0, 3);
            switch (rdIdTem)
            {
                case 1:
                    data = DataManager.ProductAsset.GetProductByName("Roof tiles");
                    break;
                case 2:
                    data = DataManager.ProductAsset.GetProductByName("Axe");
                    break;
            }
            data.total++;
            this.PostEvent((int)EventID.OnFxMaterial, new MessageFx { data = data, pos = transform.position });
            AnalyticsManager.LogEvent("Bonus_Random_Material", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "harvest", "Car claim" },
            { "name_material", data.name },
            { "item_total", data.total } });
        }
        
    }
    void AddCoin()
    {
        CoinManager.AddCoin(coinOrder, transform);
        CoinManager.AddExp(expOrder, transform);
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, path.wps[0]) < 0.1f)
        {
            if (!isCarback)
                Cargoods.transform.localScale = new Vector3(1, 1, 1);
            else
                CarGold.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
