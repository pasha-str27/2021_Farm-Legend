using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDownObject : MonoBehaviour
{
    [SerializeField] bool highlightWhenClick;
    [SerializeField] SpriteRenderer spr;

    [SerializeField] bool sacleWhenClick;
    [SerializeField] Transform childScale;
    [SerializeField] Vector2 vtScale = new Vector2(.85f, .85f);
    [SerializeField] float scaleTime = .3f;
    [SerializeField] ParticleSystem particle;

    [SerializeField] ObjectMouseDown objectMouse;
    [Header("Fx store")]
    [SerializeField] ParticleSystem fx;
    Transform obj;
    float mouseTime;
    Vector3 vitricu;
    private void Start()
    {
        obj = gameObject.transform;
        if (childScale != null)
            obj = childScale;
        if (particle != null)
            particle.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnUpgradeComplite, OnUpgradeCompliteHandle,DispatcherType.Late);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUpgradeComplite, OnUpgradeCompliteHandle);
    }

    private void OnUpgradeCompliteHandle(object obj)
    {
        var msg = (ObjectMouseDown)obj;
        if (fx != null && msg == objectMouse)
        {
            fx.Play();
            SoundManager.Play("shine_short");
            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                pos = transform.position,
            });
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
    IEnumerator DoScale()
    {
        if (sacleWhenClick)
        {
            obj.DOScale(vtScale, scaleTime / 2);
        }
        yield return new WaitForSeconds(.1f);
        if (sacleWhenClick)
        {
            obj.DOScale(Vector2.one, scaleTime);
        }
    }

    private void OnMouseUp()
    {
        if (!Util.IsMouseOverUI)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false && Vector3.Distance(vitricu, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.2f
                && Time.time - mouseTime < 0.2f)
            {
                Vector3 temp = transform.position;
                temp.y += 1.5f;
                SoundManager.Play("tap");
                if (PlayerPrefSave.IsTutorial && name.Equals("tree_tutorial"))
                {
                    if (PlayerPrefSave.stepTutorial == 8)
                    {
                        switch (PlayerPrefSave.stepTutorialCurrent)
                        {
                            case 0:
                            case 2:
                                //do somthing
                                this.PostEvent((int)EventID.OnClickObject, new MessageObject
                                {
                                    type = objectMouse,
                                    name = this.name,
                                    pos = temp,
                                    idMap = GetComponent<MapLockController>() != null ? GetComponent<MapLockController>().idMap : -1
                                });
                                Util.objClick = gameObject;
                                PlayerPrefSave.stepTutorialCurrent = 1;
                                this.PostEvent((int)EventID.OnLoadTutorial);
                                break;
                        }
                    }
                    return;
                }
                if (PlayerPrefSave.IsTutorial && objectMouse == ObjectMouseDown.Garbage)
                    return;
                if (GameUIManager.IsShowUiMove)
                    return;

                //do somthing
                
                if (highlightWhenClick)
                {
                    MapLockController mapLock = GetComponent<MapLockController>();
                    if(mapLock != null)
                    {
                        mapLock.CheckMapRequiment(
                            ()=> {
                                this.PostEvent((int)EventID.OnClickObject, new MessageObject
                                {
                                    type = objectMouse,
                                    name = this.name,
                                    pos = temp,
                                    idMap = mapLock.idMap
                                });
                            },
                            ()=> {

                                Debug.Log("=> unlokc map fail");
                                UIToast.Show("Please open the previous map!",null, ToastType.Notification, 2f);
                            });
                    }
                }
                else
                {
                    this.PostEvent((int)EventID.OnClickObject, new MessageObject
                    {
                        type = objectMouse,
                        name = this.name,
                        pos = temp,
                        idMap = GetComponent<MapLockController>() != null ? GetComponent<MapLockController>().idMap : -1
                    });
                }
               
                Util.objClick = gameObject;

                StartCoroutine(DoScale());
                if(highlightWhenClick)
                    StartCoroutine(DoHighlight(.1f));
                if (particle)
                {
                    particle.gameObject.SetActive(true);
                    particle.Play();
                }
            }

        }
    }
    public void OnHighlight()
    {
        Vector3 temp = transform.position;
        temp.y += 1.5f;
        this.PostEvent((int)EventID.OnClickObject, new MessageObject
        {
            pos = temp,
        });
        StartCoroutine(DoHighlight(3));
    }

    IEnumerator DoHighlight(float time)
    {
        spr.DOColor(new Color(1, 1, 1, 1), .2f);
        yield return new WaitForSeconds(time);
        spr.DOColor(new Color(1, 1, 1, 0), .15f);
    }
}

public enum ObjectMouseDown
{
    Silo = 1, Storage = 2, MainHouse = 3, Market = 4, Orders = 5, Crops = 6,
    MapLock = 7, Garbage = 8, Cage = 9, Factory = 10, Building = 11, OldTree = 12, Gift = 13, Car = 14, Harbor = 15,
    GoldMine = 16,
}