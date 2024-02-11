using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Linq;
using MyBox;

public class FactoryController : BaseBuilding
{
    [SerializeField] string nameFactory;
    [SerializeField] FinishedProduct finishedProduct;
    [ReadOnly] public List<ProductData> listDataWaiting = new List<ProductData>();
    private Vector3 prevPos;
    private float timeMouse;

    private Coroutine waitingFactory;
    [SerializeField] GameObject effect;
    [SerializeField] GameObject effectSleep;
    [SerializeField] Animator anim;

    [SerializeField] Transform childScale;
    [SerializeField] float scaleTime = .3f;
    [SerializeField] Vector2 vtScale = new Vector2(.85f, .85f);
    [SerializeField] bool sacleWhenClick;

    ProductData tempProduct = null;
    int tempTimeOffline = 0;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle);
        this.RegisterListener((int)EventID.OnAddProductFactoryUi, OnAddProductFactoryUiHandle);
        this.RegisterListener((int)EventID.OnSpeedUp, OnSpeedUpHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnAddProductFactoryUi, OnAddProductFactoryUiHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnSpeedUp, OnSpeedUpHandle);
    }

    private void OnSpeedUpHandle(object obj)
    {
        var msg = (MessagerCountDown)obj;
        if (msg.keyId == idBuilding.ToString())
        {
            timeLife = 0;
            waitingFactory = StartCoroutine(Waiting());
            this.PostEvent((int)EventID.OnFxPutDiamond, new MessageFx { pos = transform.position +new Vector3(0,1.5f,0)});
        }
    }

    private void OnAddProductFactoryUiHandle(object obj)
    {
        if (Util.objClick != gameObject)
            return;
        AddProduct((ProductData)obj);
    }

    private void OnViewCamTutorialHandle(object obj)
    {
        if (!PlayerPrefSave.IsTutorial)
            return;
        if ((PlayerPrefSave.stepTutorial == 7 && idBuilding == 5) || (PlayerPrefSave.stepTutorial == 5 && idBuilding == 7))
        {
            Vector3 temp = transform.position;
            temp.y += 2f;

            this.PostEvent((int)EventID.OnLockCamera, true);

            //do somthing
            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                pos = temp,
            });

        }
    }

    private void Start()
    {
        if (effect != null)
        {
            effect.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (!Util.IsMouseOverUI)
        {
            prevPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            timeMouse = Time.time;
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
        if (!GetComponent<Building>().placed) return;
        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0)) return;

        Vector3 current = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector3.Distance(prevPos, current) < 0.5f && (Time.time - timeMouse) < 0.5f)
        {
            Vector3 temp = transform.position;
            temp.y += 1.5f;
            if (PlayerPrefSave.IsTutorial)
            {
                if (PlayerPrefSave.stepTutorial == 5 || nameFactory.Equals("Food Processor"))
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 0:
                            PlayerPrefSave.stepTutorialCurrent = 1;
                            this.PostEvent((int)EventID.OnLoadTutorial);

                            this.PostEvent((int)EventID.OnClickObject, new MessageObject
                            {
                                idFactory = idBuilding,
                                name = nameFactory,
                                pos = temp,
                                type = ObjectMouseDown.Factory,
                                isHarvest = finishedProduct.listData.Count > 0
                            });
                            Util.objClick = gameObject;
                            break;
                    }
                }
                if (PlayerPrefSave.stepTutorial == 7 || nameFactory.Equals("Bakery"))
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 0:
                            PlayerPrefSave.stepTutorialCurrent = 1;
                            this.PostEvent((int)EventID.OnLoadTutorial);

                            this.PostEvent((int)EventID.OnClickObject, new MessageObject
                            {
                                idFactory = idBuilding,
                                name = nameFactory,
                                pos = temp,
                                type = ObjectMouseDown.Factory,
                                isHarvest = finishedProduct.listData.Count > 0
                            });
                            Util.objClick = gameObject;
                            break;
                    }
                }
                return;
            }

            if (GameUIManager.IsShowUiMove)
                return;

            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                idFactory = idBuilding,
                name = nameFactory,
                pos = temp,
                type = ObjectMouseDown.Factory,
                isHarvest = finishedProduct.listData.Count > 0,
                timeCount = timeLife
            });
            Util.objClick = gameObject;

            if (sacleWhenClick && GetComponent<Building>().placed)
            {
                StartCoroutine(DoScale());
            }
            SoundManager.Play("tap");
        }
    }

    public void AddProduct(ProductData data)
    {
        for (int i = 0; i <= PlayerPrefSave.GetLevelFactory(idBuilding) + 1; i++)
        {
            if (PlayerPrefSave.GetProductFactory(idBuilding, i) == -1)
            {
                bool enoughRequirement = false;
                for (int k = 0; k < data.requirements.Count; k++)
                {
                    tempProduct = DataManager.ProductAsset.GetProductByName(data.requirements[k].name);
                    if (tempProduct == null)
                        continue;

                    if (tempProduct.total >= data.requirements[k].count)
                    {
                        enoughRequirement = true;
                    }
                    else
                    {
                        enoughRequirement = false;
                        break;
                    }
                }

                if (enoughRequirement)
                {
                    for (int k = 0; k < data.requirements.Count; k++)
                    {
                        tempProduct = DataManager.ProductAsset.GetProductByName(data.requirements[k].name);
                        if (tempProduct == null)
                            continue;
                        tempProduct.total -= data.requirements[k].count;
                    }

                    //if(data == null)
                    this.data = data;

                    listDataWaiting.Add(data);

                    PlayerPrefSave.SetProductFactory(idBuilding, i, data.index);
                    this.PostEvent((int)EventID.OnAddProductFactory, new MessageFactory { id = idBuilding, time = timeLife});
                    if (i == 0)
                    {
                        timeLife= data.time;
                        waitingFactory = StartCoroutine(Waiting());
                        anim.SetBool("active", true);
                        if (effect != null)
                        {
                            effect.SetActive(true);
                        }
                    }

                    if (data.requirements.FirstOrDefault(x => x.name.Equals("Milk")) != null)
                    {
                        this.PostEvent((int)EventID.OnUpdateAchie, 2);
                    }

                }
                else
                {
                    UIToast.Show("Not enough production materials!", null, ToastType.Notification, 1.5f);
                }
                break;
            }
        }
    }

    // BUg
    // - Khi có thể thu hoạch mà ấn vào thì thu hoạch
    // - Hết

    IEnumerator Waiting()
    {
        if (PlayerPrefSave.GetProductFactory(idBuilding, 0) > -1)
        {
            effectSleep?.SetActive(false);
            if (PlayerPrefSave.IDChoose == idBuilding)
                anim.SetBool("active", true);
            if (effect != null)
            {
                effect.SetActive(true);
            }
            if (PlayerPrefSave.IDChoose == idBuilding)
            {
                // Gui time len
                this.PostEvent((int)EventID.OnSendTimeFactory, timeLife);
            }

            while (timeLife >= 0)
            {
                if (effect != null)
                {
                    effect.SetActive(true);
                }
                anim.SetBool("active", true);
                yield return new WaitForSeconds(1);
                timeLife --;
                if (PlayerPrefSave.IDChoose == idBuilding)
                {
                    // Gui Time len
                    this.PostEvent((int)EventID.OnSendTimeFactory, timeLife);
                }
            }

            // Sau khi sản xuất xong thì hiện lên để ấn vào
            if (PlayerPrefSave.GetProductFactory(idBuilding, 0) > -1)
            {
                //Debug.Log("=> Complite " + data.name);
                //finishedProduct.LoadProductComplite(data, idBuilding);
                if (listDataWaiting.Count > 0)
                {
                    finishedProduct.ShowProductComplite(listDataWaiting[0], idBuilding);
                    listDataWaiting.RemoveAt(0);
                }
                //data = null;
            }

            // Chuyển sản phẩm tiếp theo lên để sản xuất
            PlayerPrefSave.SetProductFactory(idBuilding, 0, PlayerPrefSave.GetProductFactory(idBuilding, 1));
            PlayerPrefSave.SetProductFactory(idBuilding, 1, PlayerPrefSave.GetProductFactory(idBuilding, 2));
            PlayerPrefSave.SetProductFactory(idBuilding, 2, PlayerPrefSave.GetProductFactory(idBuilding, 3));
            PlayerPrefSave.SetProductFactory(idBuilding, 3, PlayerPrefSave.GetProductFactory(idBuilding, 4));
            PlayerPrefSave.SetProductFactory(idBuilding, 4, PlayerPrefSave.GetProductFactory(idBuilding, 5));
            PlayerPrefSave.SetProductFactory(idBuilding, 5, PlayerPrefSave.GetProductFactory(idBuilding, 6));
            PlayerPrefSave.SetProductFactory(idBuilding, 6, -1);

            //time next
            if (PlayerPrefSave.GetProductFactory(idBuilding, 0) > -1)
            {
                data = DataManager.ProductAsset.list[PlayerPrefSave.GetProductFactory(idBuilding, 0)];
                //listDataWaiting.Add(data);
                if (tempTimeOffline > 0)
                {
                    timeLife = data.time - tempTimeOffline;
                    if (data.time - tempTimeOffline < 0)
                        tempTimeOffline -= data.time;
                    else tempTimeOffline = 0;
                }
                else
                {
                    timeLife = data.time;
                }
            }
            this.PostEvent((int)EventID.OnAddProductFactory, new MessageFactory { id = idBuilding, time = timeLife });
            waitingFactory = StartCoroutine(Waiting());
        }
        else
        {
            effectSleep?.SetActive(true);
            data = null;
            waitingFactory = null;
            anim.SetBool("active", false);
            listDataWaiting = new List<ProductData>();
            if (effect != null)
            {
                effect.SetActive(false);
            }
        }
    }

    public override void SetNewID(int id)
    {
        base.SetNewID(id);
        idBuilding = id;
        for (int i = 0; i < 7; i++)
        {
            PlayerPrefSave.SetProductFactory(idBuilding, i, -1);
        }
        anim.SetBool("active", false);
        listDataWaiting.Clear();
        //thêm số lượng sp đầu tiên của nhà máy
        DataManager.FactoryAsset.SetFullRequiment(nameFactory);
    }

    public override void SetOldID(int id)
    {
        base.SetOldID(id);
        idBuilding = id;
        if (PlayerPrefSave.GetProductFactory(idBuilding, 0) != -1)
        {
            data = DataManager.ProductAsset.list[PlayerPrefSave.GetProductFactory(idBuilding, 0)];
            listDataWaiting.Add(data);
            if (timeLife > 0)
            {
                if (timeLife - Util.timeOffline < 0)
                {
                    tempTimeOffline = Util.timeOffline - timeLife;
                    timeLife = 0;
                }
                else
                    timeLife -= Util.timeOffline;
            }
            waitingFactory = StartCoroutine(Waiting());

            //idProduct = PlayerPrefSave.GetProductFactory(idFactory, id);
            for (int i = 1; i <= (PlayerPrefSave.GetLevelFactory(idBuilding) + 1); i++)
            {
                if (i == 6) break;
                int tempId = PlayerPrefSave.GetProductFactory(id, i);
                if (tempId > -1)
                    listDataWaiting.Add(DataManager.ProductAsset.list[tempId]);
            }
        }
        else
            effectSleep.SetActive(true);

        finishedProduct.StartLoadProductComplite(idBuilding);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("ObjFollow"))
        {
            ObjFollow objFollow = collision.GetComponent<ObjFollow>();
            if (Util.objClick != gameObject)
                return;

            if (objFollow.type == ObjectMouseDown.Factory)
            {
                if (objFollow.productData != null)
                {
                    objFollow.isTrigger = true;
                    AddProduct(objFollow.productData);
                }
            }
        }
    }
    #region save
    int timeLife {
        set { PlayerPrefs.SetInt("timelife" + nameFactory+idBuilding, value); }
        get { return PlayerPrefs.GetInt("timelife" + nameFactory + idBuilding, 0); }
    }
    #endregion
}
public class MessageFactory
{
    public int id;
    public int time;
}