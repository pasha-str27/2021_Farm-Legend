using UnityEngine;
using MyBox;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Spine.Unity;
using DG.Tweening;
using System.Collections;

public class CageController : BaseBuilding
{
    [ReadOnly] [SerializeField] bool isHarvest = false;
    float mouseTime;
    Vector3 oldPos;

    [SerializeField] List<GameObject> listAnimal = new List<GameObject>();
    [SerializeField] Transform animalParent;
    [SerializeField] string nameProduct;
    [SerializeField] string nameFeed;
    [SerializeField] string nameSound;

    [SerializeField] Transform childScale;
    [SerializeField] float scaleTime = .3f;
    [SerializeField] Vector2 vtScale = new Vector2(.85f, .85f);
    [SerializeField] bool sacleWhenClick;
    [SerializeField] bool isRaising;

    CountDownTime countDownTime;
    private Coroutine coroutine;
    private void Awake()
    {
        countDownTime = GetComponent<CountDownTime>();
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
        if (PlayerPrefSave.stepTutorial == 1 || PlayerPrefSave.stepTutorial == 2 || PlayerPrefSave.stepTutorial == 3)
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
        countDownTime.SpeedUp();
        this.PostEvent((int)EventID.OnFxPutDiamond, new MessageFx { pos = childScale.position +new Vector3(0,2,0)});
    }

    public override void HandleEvent()
    {
        if (countDownTime.timeLife <= 0)
        {
            isHarvest = true;
            countDownTime.isComplete = 1;
            countDownTime.timeLife = 0;
            for (int i = 0; i < listAnimal.Count; i++)
            {
                if (PlayerPrefSave.GetAnimalEating(idBuilding, i) == 1)
                {
                    listAnimal[i].GetComponent<Cage>().Thuhoach();
                }
                else
                {
                    listAnimal[i].GetComponent<Cage>().Doi();
                }
            }
        }
        else
        {
            isHarvest = false;
            for (int i = 0; i < listAnimal.Count; i++)
            {
                if (PlayerPrefSave.GetAnimalEating(idBuilding, i) == 1)
                {
                    listAnimal[i].GetComponent<Cage>().An();
                }
                else
                {
                    listAnimal[i].GetComponent<Cage>().Doi();
                }
            }
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

                if (PlayerPrefSave.GetTotalAnimal(idBuilding) <= 0)
                {
                    this.PostEvent((int)EventID.OnShowShop, TypeShop.Animals);
                }
                else
                {
                    Vector3 temp = transform.position;
                    temp.y += 2.5f;
                    for (int i = 0; i < PlayerPrefSave.GetTotalAnimal(idBuilding); i++)
                    {
                        if (PlayerPrefSave.GetAnimalEating(idBuilding, i) == 0)
                            isRaising = true;
                        else
                            isRaising = false;
                    }
                    //do somthing
                    this.PostEvent((int)EventID.OnClickObject, new MessageObject
                    {
                        timeCount = countDownTime.timeLife,
                        name = nameFeed,
                        nameKey = idBuilding + data.name,
                        pos = temp,
                        type = ObjectMouseDown.Cage,
                        isHarvest = isHarvest,
                        isRaising = isRaising,
                        data = data,
                        nameCage = this.name
                    });

                    Util.objClick = gameObject;

                    if (sacleWhenClick && GetComponent<Building>().placed)
                    {
                        StartCoroutine(DoScale());
                    }
                    SoundManager.Play("tap");

                    SoundManager.Play(nameSound);

                    if (!PlayerPrefSave.IsTutorial)
                        return;
                    if (PlayerPrefSave.stepTutorial == 3)
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
                            case 5:
                                PlayerPrefSave.stepTutorialCurrent = 6;
                                this.PostEvent((int)EventID.OnLoadTutorial);
                                break;
                        }
                    }
                }
            }
        }
    }

    public void AddCage(AnimalDrag animalDrag)
    {
        if (PlayerPrefSave.GetTotalAnimal(idBuilding) < 6)
        {
            GameObject animal = listAnimal[PlayerPrefSave.GetTotalAnimal(idBuilding)];
            animal.SetActive(true);
            animal.GetComponent<Cage>().Doi();
            PlayerPrefSave.SetTotalAnimal(idBuilding, PlayerPrefSave.GetTotalAnimal(idBuilding) + 1);
            if (PlayerPrefSave.GetTimeCage(idBuilding) <= 0)
            {
                PlayerPrefSave.SetTimeCage(idBuilding, DataManager.ProductAsset.GetProductByName(animalDrag.nameProduct).time);
            }
            CoinManager.AddCoin(-animalDrag.shopData.price);
            CoinManager.AddExp(animalDrag.shopData.exp, animal.transform);
            this.PostEvent((int)EventID.OnFxPutCoin, animal.transform.position);

            animalDrag.shopData.countBuild++;

            if (PlayerPrefSave.stepTutorial == 2)
            {
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 3:
                        PlayerPrefSave.stepTutorialCurrent = 4;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
            }

            return;
        }
        else
        {
            UIToast.Show("Cage is full!", null, ToastType.Notification, 1.5f);
        }
    }

    public void Harvest()
    {
        if (isHarvest)
        {
            int tempTotal = 0;
            int tempCountHarvest = 0;
            for (int i = 0; i < listAnimal.Count; i++)
            {
                if (PlayerPrefSave.GetAnimalEating(idBuilding, i) == 1)
                {
                    tempTotal++;
                }
            }

            this.PostEvent((int)EventID.OnAddProduct,
               new MessagerAddProduct
               {
                   data = data,
                   cropYields = tempTotal,
                   onDone = () =>
                   {
                       //thu huoạch
                       for (int i = 0; i < listAnimal.Count; i++)
                       {
                           if (PlayerPrefSave.GetAnimalEating(idBuilding, i) == 1)
                           {
                               this.PostEvent((int)EventID.OnFxPutIn, new MessageFx { data = data, typePut = TypePut.Collect, pos = listAnimal[i].transform.position });
                               data.total++;
                               listAnimal[i].GetComponent<Cage>().Doi();
                               CoinManager.AddExp(data.exp, transform);
                               PlayerPrefSave.SetTimeCage(idBuilding, 0);
                               PlayerPrefSave.SetAnimalEating(idBuilding, i, 0);

                               if (nameProduct.Equals("Egg"))
                               {
                                   this.PostEvent((int)EventID.OnUpdateAchie, 4);
                               }
                               if (nameProduct.Equals("Milk"))
                               {
                                   this.PostEvent((int)EventID.OnUpdateAchie, 5);
                               }
                               if (nameProduct.Equals("Bacon"))
                               {
                                   this.PostEvent((int)EventID.OnUpdateAchie, 6);
                               }
                               if (nameProduct.Equals("Fleece"))
                               {
                                   this.PostEvent((int)EventID.OnUpdateAchie, 7);
                               }
                           }
                       }
                       for (int i = 0; i < listAnimal.Count; i++)
                       {
                           if (PlayerPrefSave.GetAnimalEating(idBuilding, i) == 0)
                           {
                               tempCountHarvest++;
                           }
                       }

                       if (tempCountHarvest >= listAnimal.Count - 1)
                       {
                           isHarvest = false;
                           countDownTime.isComplete = 0;
                           countDownTime.timeLife = 0;
                       }
                       SoundManager.Play("sfxHarvest");

                   },
                   onFail = () =>
                   {
                       UIToast.Show("The warehouse is full!", null, ToastType.Notification, 1.5f);
                   }
               }
               );

            if (!PlayerPrefSave.IsTutorial)
                return;
            if (PlayerPrefSave.stepTutorial == 3)
            {
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 6:
                        PlayerPrefSave.stepTutorialCurrent = 7;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
            }
        }
    }

    public void Raising()
    {
        // tim thuc an cua no va sau do cho no a
        ProductData _thucan = DataManager.ProductAsset.GetProductByName(nameFeed);

        if (_thucan.total > 0)
        {
            for (int i = 0; i < PlayerPrefSave.GetTotalAnimal(idBuilding); i++)
            {
                if (PlayerPrefSave.GetAnimalEating(idBuilding, i) == 0)
                {
                    this.PostEvent((int)EventID.OnFxPutIn, new MessageFx { data = DataManager.ProductAsset.GetProductByName(nameFeed), typePut = TypePut.PutIn, pos = listAnimal[i].transform.position });
                    listAnimal[i].GetComponent<Cage>().ChoAn();
                    PlayerPrefSave.SetAnimalEating(idBuilding, i, 1);
                }
            }
            _thucan.total -= 1;
            countDownTime.Init(idBuilding + data.name, data.time);

            if (!PlayerPrefSave.IsTutorial)
                return;
            if (PlayerPrefSave.stepTutorial == 3)
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
            UIToast.Show("Not enough food to feed!", null, ToastType.Notification, 1.5f);
        }
    }

    public override void SetNewID(int id)
    {
        base.SetNewID(id);
        idBuilding = id;
        isHarvest = false;
        DataManager.ProductAsset.GetProductByName(nameFeed).total += 1;

        for (int i = 0; i < 6; i++)
        {
            PlayerPrefSave.SetAnimalEating(idBuilding, i, 0);
            listAnimal[i].SetActive(false);
        }
        data = DataManager.ProductAsset.GetProductByName(nameProduct);
    }

    public override void SetOldID(int id)
    {
        base.SetOldID(id);
        idBuilding = id;
        data = DataManager.ProductAsset.GetProductByName(nameProduct);
        for (int i = 0; i < 6; i++)
        {
            if (i < PlayerPrefSave.GetTotalAnimal(idBuilding))
            {
                listAnimal[i].SetActive(true);
            }
            else
            {
                listAnimal[i].SetActive(false);
            }
        }

        Invoke("DelayLoadTime", 1f);

    }
    void DelayLoadTime()
    {
        bool isRun = false;
        for (int i = 0; i < 6; i++)
        {
            if (PlayerPrefSave.GetAnimalEating(idBuilding, i) == 1)
            {
                isRun = true;
            }else
                listAnimal[i].GetComponent<Cage>().Doi();
        }
        if(isRun)
            countDownTime.Init(idBuilding + data.name, data.time);
    }
    public override void UpdateOrderLayer()
    {
        float tempY = transform.position.y * 100;
        if (tempY > 0)
        {
            for (int i = listAnimal.Count - 1; i >= 0; i--)
            {
                listAnimal[i].transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = (int)(tempY * -1) + i;
            }
        }
        else
        {
            for (int i = 0; i < listAnimal.Count; i++)
            {
                listAnimal[i].transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = (int)(tempY * -1) + i;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("ObjFollow"))
        {
            ObjFollow objFollow = collision.GetComponent<ObjFollow>();

            if (Util.objClick != gameObject && !objFollow.isTrigger)
                return;

            if (objFollow.type == ObjectMouseDown.Cage)
            {
                if (objFollow.productData == null)
                {
                    //thu họach
                    Harvest();
                    objFollow.isTrigger = true;
                }
                else
                if (objFollow.productData.name.Equals(nameFeed))
                {
                    //cho ăn
                    Raising();
                    objFollow.isTrigger = true;
                }
            }
        }
        else
        {
            if (collision.CompareTag("AnimalDrag"))
            {
                AnimalDrag animalDrag = collision.GetComponent<AnimalDrag>();
                if (animalDrag.nameProduct == nameProduct)
                {
                    //AddCage(animalDrag);
                    //Destroy(collision.gameObject);
                    animalDrag.cageController = this;
                    animalDrag.isTrigger = true;

                    //this.PostEvent((int)EventID.OnLockCamera, false);
                }
            }
        }
    }
}
