using UnityEngine;
using UnityEngine.EventSystems;

public class DragUICage : MonoBehaviour, IPointerDownHandler
{
    public bool isLock;
    public GameObject iconPrefab;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isLock)
        {
            Instantiate(iconPrefab);
            this.PostEvent((int)EventID.OnHideUICage);
        }
    }
}
