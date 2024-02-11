using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class OldTreeController : BaseBuilding
{
    [SerializeField] SpriteRenderer plant;
    [SerializeField] CountDownTime countDownTime;
    [ReadOnly] [SerializeField] bool isHarvest = false;
    [Header("tree old")]
    [SerializeField] Animator anim;
    [SerializeField] string nameProduct;
    [SerializeField] PolygonCollider2D box;
    [SerializeField] Building building;
    [SerializeField] GameObject tree;
    [SerializeField] GameObject objWilted;
    float mouseTime;
    Vector3 oldPos;
    private void Start()
    {
        if (objWilted)
        {
            objWilted.SetActive(false);
            if (countHarvest >= DataManager.GameConfig.NumHarvestOldTree)
            {
                WiltedTree();
            }
        }
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnSpeedUp, OnSpeedUpHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnSpeedUp, OnSpeedUpHandle);
    }
    private void OnSpeedUpHandle(object obj)
    {
        if (data == null)
            return;
        var msg = (MessagerCountDown)obj;
        if (!msg.keyId.Equals(idBuilding + data.name))
            return;
        this.PostEvent((int)EventID.OnFxPutDiamond, new MessageFx { pos = transform.position + new Vector3(0, 1.5f, 0) });
        countDownTime.SpeedUp();
    }

    public override void HandleEvent()
    {
        base.HandleEvent();
        tempTime = data.time / 3;
        if (countDownTime.timeLife >= tempTime * 2)
            plant.sprite = data.spStage1;
        else
            if (countDownTime.timeLife > 0)
            plant.sprite = data.spStage2;
        else
        {
            isHarvest = true;
            countDownTime.isComplete = 1;
            countDownTime.timeLife = 0;
            plant.sprite = data.spStage3;
        }

        this.PostEvent((int)EventID.OnUpdateProgress, new MessagerCountDown { keyId = idBuilding + data.name, timeLife = countDownTime.timeLife });
    }

    private void OnMouseDown()
    {
        if (!Util.IsMouseOverUI)
        {
            mouseTime = Time.time;
            oldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if (!Util.IsMouseOverUI)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false && Vector3.Distance(oldPos, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.2f
                && Time.time - mouseTime < 0.2f)
            {
                if (!GetComponent<Building>().placed) return;

                Vector3 temp = transform.position;
                temp.y += 1.5f;
                //do somthing
                this.PostEvent((int)EventID.OnClickObject, new MessageObject
                {
                    name = data != null ? idBuilding + data.name : "",
                    pos = temp,
                    type = ObjectMouseDown.OldTree,
                    isHarvest = isHarvest,
                    timeCount = countDownTime.timeLife,
                    data = data,
                    wiltedTree = objWilted
                });
            }
        }
    }

    public override void SetNewID(int id)
    {
        base.SetNewID(id);
        idBuilding = id;
        this.data = DataManager.ProductAsset.GetProductByName(nameProduct);
        plant.sprite = data.spStage1;
        countDownTime.Init(id + data.name, data.time);
    }
    public override void SetOldID(int id)
    {
        base.SetOldID(id);
        idBuilding = id;
        this.data = DataManager.ProductAsset.GetProductByName(nameProduct);
        plant.sprite = data.spStage1;
        countDownTime.Init(id + data.name, data.time);
    }

    void WiltedTree()
    {
        if (objWilted == null)
            return;
        isHarvest = false;
        countDownTime.timeLife = 0;
        objWilted.SetActive(true);
        tree.SetActive(false);
    }
    void HandleDestroyObject(ProductData data)
    {
        if(anim != null)
            StartCoroutine(DelayDestroy(data));
    }
    void Hartvest()
    {
        this.PostEvent((int)EventID.OnAddProduct,
                new MessagerAddProduct
                {
                    data = data,
                    onDone = () =>
                    {
                        //thu huoạch
                        isHarvest = false;
                        data.total += data.cropYields;
                        CoinManager.AddExp(data.exp, transform);
                        this.PostEvent((int)EventID.OnFxPutIn, new MessageFx { data = data, typePut = TypePut.Collect, pos = plant.transform.position });
                        SoundManager.Play("sfxHarvest");
                        if(objWilted != null)
                        {
                            if (countHarvest >= DataManager.GameConfig.NumHarvestOldTree)
                            {
                                WiltedTree();
                            }
                            else
                            {
                                countHarvest++;
                                plant.sprite = data.spStage1;
                                countDownTime.isComplete = 0;
                                countDownTime.Init(idBuilding + data.name, data.time);
                            }
                        }
                        else
                        {
                            plant.sprite = data.spStage1;
                            countDownTime.isComplete = 0;
                            countDownTime.Init(idBuilding + data.name, data.time);
                        }
                        RandomMaterial();
                    },
                    onFail = () =>
                    {
                        UIToast.Show("The warehouse is full!", null, ToastType.Notification, 1.5f);
                    }
                }
                );
    }
    IEnumerator DelayDestroy(ProductData data)
    {
        anim.Play("stonePickaxe", -1, 0);
        yield return new WaitForSeconds(2);
        try
        {
            anim.Play("stoneDestroy", -1, 0);
        }
        catch { }

        yield return new WaitForSeconds(.5f);
        try
        {
            data.total++;
            DataManager.ShopAsset.DestroyOldTre(nameProduct);
            this.PostEvent((int)EventID.OnFxMaterial, new MessageFx { pos = transform.position, data = data });
            if (data.name.Equals("Cherry"))
            {
                this.PostEvent((int)EventID.OnUpdateAchie, 14);
            }
            GridBuildingSystem.instance.DeleteArea(building.area);
            PlayerPrefSave.DeleteBuilding(idBuilding);

            gameObject.Recycle();
        }
        catch { }

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
            { "harvest", "Old tree" },
            { "name_material", data.name },
            { "item_total", data.total } });
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("ObjFollow"))
        {
            ObjFollow objFollow = collision.GetComponent<ObjFollow>();

            if (Util.objClick != gameObject && !objFollow.isTrigger)
                return;

            if (objFollow.type == ObjectMouseDown.OldTree)
            {
                if (objFollow.productData != null)
                {
                    if (objFollow.productData.name.Contains("Saw"))
                    {
                        if (DataManager.ProductAsset.GetProduct(TabName.Material, "Saw").total > 0)
                        {
                            //cua
                            box.enabled = false;
                            ProductData data = DataManager.ProductAsset.GetProductByName("Plank");
                            data.total++;
                            HandleDestroyObject(data);
                            objFollow.isTrigger = true;
                        }
                        else
                        {
                            //UIToast.Show("Not enought Saw", null, ToastType.Notification, 1.5f);
                            this.PostEvent((int)EventID.OnNotEnought, new MessagerUiNotEnought { productData = objFollow.productData, need = 1 });
                        }
                        return;
                    }
                }
                else
                if (objFollow.productData == null && isHarvest)
                {
                    Hartvest();
                    objFollow.isTrigger = true;
                }

            }
        }
    }

    int countHarvest
    {
        set { PlayerPrefs.SetInt("countHarvest" + idBuilding, value); }
        get { return PlayerPrefs.GetInt("countHarvest" + idBuilding, 0); }
    }
}
