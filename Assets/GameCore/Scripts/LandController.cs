using UnityEngine;
using System.Collections;
using MyBox;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class LandController : BaseBuilding
{
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer plant;
    [SerializeField] CountDownTime countDownTime;
    [ReadOnly] [SerializeField] bool isHarvest = false;

    [SerializeField] bool sacleWhenClick;
    [SerializeField] Transform childScale;
    [SerializeField] Vector2 vtScale = new Vector2(.85f, .85f);
    [SerializeField] float scaleTime = .3f;

    float mouseTime;
    Vector3 oldPos;
    ObjFollow objFollow;

    private void Start()
    {
        anim.speed = UnityEngine.Random.Range(0.8f, 1.2f);

        if (PlayerPrefSave.IsTutorial)
        {
            if (PlayerPrefSave.stepTutorial == 0 && PlayerPrefSave.stepTutorialCurrent == 0)
            {
                Planting(DataManager.ProductAsset.GetProductByName("Wheat"));
            }
        }
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnSpeedUp, OnSpeedUpHandle);
        this.RegisterListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnSpeedUp, OnSpeedUpHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle);
    }

    private void OnViewCamTutorialHandle(object obj)
    {
        if (!PlayerPrefSave.IsTutorial)
            return;
        if ((PlayerPrefSave.stepTutorial == 0 && idBuilding == 1))
        {
            Vector3 temp = transform.position;
            temp.y += 1.5f;

            this.PostEvent((int)EventID.OnLockCamera, true);

            //do somthing
            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                pos = temp,
            });
        }
    }

    private void OnSpeedUpHandle(object obj)
    {
        if (data == null)
            return;
        var msg = (MessagerCountDown)obj;
        if (!msg.keyId.Equals(idBuilding + data.name))
            return;
        //Debug.Log("=> OnSpeedUpHandle " + idBuilding + data.name);
        this.PostEvent((int)EventID.OnFxPutDiamond, new MessageFx { pos = transform.position + new Vector3(0,1f,0)});
        countDownTime.SpeedUp();
    }

    public override void HandleEvent()
    {
        tempTime = data.time / 3;
        if (countDownTime.timeLife >= tempTime * 2)
        {
            if (plant.sprite != data.spStage1)
            {
                plant.sprite = data.spStage1;
                this.PostEvent((int)EventID.OnFxStagesCrops, new MessageFx { pos = transform.position });
            }
        }
        else
            if (countDownTime.timeLife > 0)
        {
            if (plant.sprite != data.spStage2)
            {
                plant.sprite = data.spStage2;
                this.PostEvent((int)EventID.OnFxStagesCrops, new MessageFx { pos = transform.position });
            }
        }

        else
        {
            isHarvest = true;
            countDownTime.isComplete = 1;
            countDownTime.timeLife = 0;
            if (plant.sprite != data.spStage3)
            {
                plant.sprite = data.spStage3;
                this.PostEvent((int)EventID.OnFxStagesCrops, new MessageFx { pos = transform.position });
            }
        }
        this.PostEvent((int)EventID.OnUpdateProgress, new MessagerCountDown { keyId = countDownTime.keyId, timeLife = countDownTime.timeLife });
    }

    private void OnMouseDown()
    {
        if (!Util.IsMouseOverUI)
        {
            mouseTime = Time.time;
            oldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    IEnumerator DoScale()
    {
        childScale.DOScale(vtScale, scaleTime / 2);
        yield return new WaitForSeconds(.1f);
        childScale.DOScale(Vector2.one, scaleTime);
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
                    type = ObjectMouseDown.Crops,
                    isHarvest = isHarvest,
                    timeCount = countDownTime.timeLife,
                    data = data
                });
                Util.objClick = gameObject;
                if (sacleWhenClick)
                    StartCoroutine(DoScale());

                if (!PlayerPrefSave.IsTutorial)
                    return;
                if (PlayerPrefSave.stepTutorial == 0)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 0:
                            PlayerPrefSave.stepTutorialCurrent = 1;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                        case 3:
                            PlayerPrefSave.stepTutorialCurrent = 4;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
            }
        }
    }

    public void Planting(ProductData data)
    {
        if (PlayerPrefSave.GetSeed(idBuilding) == "")
        {
            this.data = data;
            if (CoinManager.totalCoin >= data.price)
            {
                CoinManager.AddCoin(-data.price, null, null, "nosound");

            }
            else
            {
                UIToast.Show("Not enough coin to planting!", null, ToastType.Notification, 1.5f);
                this.PostEvent((int)EventID.OnShowVideoReward);
                objFollow.Recycle();
                return;
            }
            PlayerPrefSave.SetSeed(idBuilding, data.name);
            plant.sprite = data.spStage1;
            this.PostEvent((int)EventID.OnFxStagesCrops, new MessageFx { pos = transform.position });
            if (PlayerPrefSave.IsTutorial && PlayerPrefSave.stepTutorial == 0 && PlayerPrefSave.stepTutorialCurrent == 0)
            {
                countDownTime.Init(idBuilding + data.name, 0);
            }
            else
                countDownTime.Init(idBuilding + data.name, data.time);

            this.PostEvent((int)EventID.OnFxPutIn, new MessageFx { data = data, typePut = TypePut.PutIn, pos = transform.position });

            if (objFollow != null)
                objFollow.isTrigger = true;

            if (!PlayerPrefSave.IsTutorial)
            {
                if (objFollow != null)
                    objFollow.isTrigger = true;
                return;
            }

        }
    }

    public void Harvest()
    {
        if (isHarvest)
        {
            this.PostEvent((int)EventID.OnAddProduct,
                new MessagerAddProduct
                {
                    data = data,
                    onDone = () =>
                    {
                        this.PostEvent((int)EventID.OnFxPutIn, new MessageFx { data = data, typePut = TypePut.Collect, pos = transform.position });
                        CoinManager.AddExp(data.exp, transform);
                        PlayerPrefSave.SetTimeGrowing(idBuilding, 0);

                        data.total += data.cropYields;
                        data = null;
                        isHarvest = false;
                        PlayerPrefSave.SetSeed(idBuilding, "");
                        plant.sprite = null;
                        countDownTime.isComplete = 0;
                        countDownTime.timeLife = 0;
                        this.PostEvent((int)EventID.OnUpdateAchie, 0);
                        SoundManager.Play("sfxHarvest");
                        RandomMaterial();
                    },
                    onFail = () =>
                    {
                        UIToast.Show("The warehouse is full!", null, ToastType.Notification, 1.5f);
                    }
                }
                );
            if (!PlayerPrefSave.IsTutorial)
            {
                if (objFollow != null)
                    objFollow.isTrigger = true;
                return;
            }
            if (PlayerPrefSave.stepTutorial == 0 && idBuilding == 1)
            {
                if (objFollow != null)
                    objFollow.isTrigger = true;
            }
        }
    }
    int tempRd = 0;
    int rdIdTem = 0;
    void RandomMaterial()
    {
        tempRd = UnityEngine.Random.Range(0, 101);
        if (tempRd <= DataManager.GameConfig.percentRandomMaterial)
        {
            rdIdTem = UnityEngine.Random.Range(0, 4);
            ProductData data = DataManager.ProductAsset.GetProductByName("Hammer");
            switch (rdIdTem)
            {
                case 1:
                    data = DataManager.ProductAsset.GetProductByName("Shovel");
                    break;
                case 2:
                    data = DataManager.ProductAsset.GetProductByName("Saw");
                    break;
                case 3:
                    data = DataManager.ProductAsset.GetProductByName("Gunpowder");
                    break;
            }
            data.total++;
            //Debug.Log("=> add material " + data.name);
            this.PostEvent((int)EventID.OnFxMaterial, new MessageFx { data = data, pos = transform.position });
            AnalyticsManager.LogEvent("Bonus_Random_Material", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "harvest", "crops land" },
            { "name_material", data.name },
            { "item_total", data.total } });
        }

    }
    public override void SetNewID(int id)
    {
        base.SetNewID(id);
        idBuilding = id;
        PlayerPrefSave.SetSeed(idBuilding, "");
    }

    public override void SetOldID(int id)
    {
        base.SetOldID(id);
        idBuilding = id;
        if (PlayerPrefSave.GetSeed(idBuilding) != "")
        {
            data = DataManager.ProductAsset.GetProductByName(PlayerPrefSave.GetSeed(idBuilding));
            countDownTime.Init(id + data.name, data.time);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("ObjFollow"))
        {
            objFollow = collision.GetComponent<ObjFollow>();
            if (objFollow.type == ObjectMouseDown.Crops)
            {
                if (Util.objClick != gameObject && !objFollow.isTrigger && !PlayerPrefSave.IsTutorial)
                    return;

                if (objFollow.productData == null)
                {
                    //thu họach
                    Harvest();
                }
                else
                {
                    Planting(objFollow.productData);
                }
            }
        }
    }

    private void OnBecameVisible()
    {
        Debug.Log("=> OnBecameVisible" + name);
    }
    private void OnBecameInvisible()
    {
        Debug.Log("=> OnBecameInvisible" + name);
    }
}
