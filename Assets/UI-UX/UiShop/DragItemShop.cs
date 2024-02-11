using UnityEngine;
using UnityEngine.EventSystems;

public class DragItemShop : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform dragRectTranform;
    private Canvas canvas;
    private bool allowInstantiate;
    private Vector2 startPos;

    public ItemShop item;
    private void Start()
    {
        dragRectTranform = GetComponent<RectTransform>();
        canvas = GameUIManager.Canvas;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PlayerPrefSave.Coin >= item.shopData.GetPrice)
        {
            if (allowInstantiate)
            {
                dragRectTranform.anchoredPosition += eventData.delta / canvas.scaleFactor;
                if (dragRectTranform.localPosition.y > 50)
                {
                    allowInstantiate = false;
                    Vector3 posDragObject = Camera.main.ScreenToWorldPoint(dragRectTranform.localPosition);
                    posDragObject.z = 0;

                    this.PostEvent((int)EventID.OnDragItem, new MessagerDragItem
                    {
                        obj = item.prefab,
                        pos = posDragObject,
                        item = item,
                        typeObject = ObjectMouseDown.Building
                    });

                    this.PostEvent((int)EventID.OnLockCamera, true);
                    dragRectTranform.anchoredPosition = startPos;
                }

                if (!PlayerPrefSave.IsTutorial)
                    return;
                if (PlayerPrefSave.stepTutorial == 1)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 1:
                            PlayerPrefSave.stepTutorialCurrent = 2;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
                if (PlayerPrefSave.stepTutorial == 2 || PlayerPrefSave.stepTutorial == 4)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 2:
                            PlayerPrefSave.stepTutorialCurrent = 3;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
                if (PlayerPrefSave.stepTutorial == 6)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 2:
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
            }
        }
        else
        {
            UIToast.Show("Not enough coin to buy!", null, ToastType.Notification, 1.5f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = dragRectTranform.anchoredPosition;
        allowInstantiate = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragRectTranform.anchoredPosition = startPos;
    }
}

public class MessagerDragItem
{
    public GameObject obj;
    public Vector3 pos;
    public ItemShop item;
    public ProductData productData;
    public ObjectMouseDown typeObject;
}