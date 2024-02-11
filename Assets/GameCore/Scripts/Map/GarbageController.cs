using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageController : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] PolygonCollider2D polygon;
    [SerializeField] Building building;
    [SerializeField] MouseDownObject mouseDownObject;
    [SerializeField] bool isObjectInMap;
    string key = "";
    bool isLockMap;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnViewCamTutorial, OnViewCamTutorialHandle);
    }

    private void OnViewCamTutorialHandle(object obj)
    {
        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 8 && name.Equals("tree_tutorial"))
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

    private void Start()
    {
        if (GetComponent<Animator>() != null)
        {
            anim = GetComponent<Animator>();
            anim.enabled = false;
            anim.speed = Random.Range(0.8f, 1.2f);
        }
    }
    public void LoadObjectInMap()
    {
        if (isObjectInMap)
        {
            Init(name + building.area.position.x + building.area.position.y, false);
        }
    }
    public void Init(string key, bool isLockMap)
    {
        this.key = key;
        this.isLockMap = isLockMap;
        if (isDestroy)
        {
            GridBuildingSystem.instance.DeleteArea(building.area);
            gameObject.Recycle();
        }
        else
        {
            polygon.enabled = !isLockMap;
            anim.enabled = false;
            building.enabled = !isLockMap;
            mouseDownObject.enabled = !isLockMap;
        }
    }

    void HandleDestroyObject(ObjFollow objFollow)
    {
        if (!polygon.enabled)
        {
            objFollow.productData.total--;
            isDestroy = true;

            StartCoroutine(DelayDestroy());
        }
    }

    IEnumerator DelayDestroy()
    {
        anim.enabled = true;
        anim.Play("stonePickaxe", -1, 0);
        yield return new WaitForSeconds(2);
        anim.Play("stoneDestroy", -1, 0);
        yield return new WaitForSeconds(.5f);

        if (name.Contains("tree"))
        {
            ProductData data = DataManager.ProductAsset.GetProductByName("Plank");
            data.total++;
            this.PostEvent((int)EventID.OnFxMaterial, new MessageFx { pos = transform.position, data = data });

            if (PlayerPrefSave.stepTutorial == 8)
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
        if (name.Contains("stone"))
        {
            ProductData data = DataManager.ProductAsset.GetProductByName("Stone");
            data.total++;
            this.PostEvent((int)EventID.OnFxMaterial, new MessageFx { pos = transform.position, data = data });
        }
        anim.enabled = false;
        CoinManager.AddExp(DataManager.GameConfig.expGarbage, transform);
        Init(key, isLockMap);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("ObjFollow"))
        {
            ObjFollow objFollow = collision.GetComponent<ObjFollow>();

            if (Util.objClick != gameObject && !objFollow.isTrigger)
                return;

            if (objFollow.type == ObjectMouseDown.Garbage)
            {
                //check
                //string tempName = objFollow.spIcon.GetComponent<SpriteRenderer>().name.ToUpper();
                //Debug.Log("=> OnTriggerEnter2D " + tempName);
                if (name.Contains("tree"))
                {
                    if (objFollow.productData.name.Contains("Saw"))
                    {
                        if (DataManager.ProductAsset.GetProduct(TabName.Material, "Saw").total > 0)
                        {
                            //cua
                            polygon.enabled = false;
                            HandleDestroyObject(objFollow);
                            objFollow.isTrigger = true;
                        }
                        else
                        {
                            //UIToast.Show("Not enought Saw", null, ToastType.Notification, 1.5f);
                            this.PostEvent((int)EventID.OnNotEnought, new MessagerUiNotEnought { productData = objFollow.productData, need = 1 });
                        }
                    }
                }
                else
                {
                    if (name.Contains("stone"))
                    {
                        if (objFollow.productData.name.Contains("Hammer"))
                        {
                            if (DataManager.ProductAsset.GetProduct(TabName.Material, "Hammer").total > 0)
                            {
                                polygon.enabled = false;
                                HandleDestroyObject(objFollow);
                                objFollow.isTrigger = true;
                            }
                            else
                            {
                                //UIToast.Show("Not enought Hammer", null, ToastType.Notification, 1.5f);
                                this.PostEvent((int)EventID.OnNotEnought, new MessagerUiNotEnought { productData = objFollow.productData, need = 1 });
                            }
                        }
                    }
                    else
                    {
                        //xeng
                        if (objFollow.productData.name.Contains("Shovel"))
                        {
                            if (DataManager.ProductAsset.GetProduct(TabName.Material, "Shovel").total > 0)
                            {
                                polygon.enabled = false;
                                HandleDestroyObject(objFollow);
                                objFollow.isTrigger = true;
                            }
                            else
                            {
                                //UIToast.Show("Not enought Shovel", null, ToastType.Notification, 1.5f);
                                this.PostEvent((int)EventID.OnNotEnought, new MessagerUiNotEnought { productData = objFollow.productData, need = 1 });
                            }
                        }
                    }
                }
            }
        }
    }

    public bool isDestroy
    {
        private set { PlayerPrefs.SetString(key, value.ToString()); }
        get { return PlayerPrefs.GetString(key, "") != ""; }
    }
}
