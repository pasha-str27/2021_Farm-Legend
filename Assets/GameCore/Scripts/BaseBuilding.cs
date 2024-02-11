using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseBuilding : MonoBehaviour
{
    public int idBuilding;
    public ProductData data = null;
    public int tempTime;
    public virtual void SetNewID(int id)
    {

    }
    public virtual void SetOldID(int id)
    {

    }
    public virtual void HandleEvent()
    {

    }

    public virtual void UpdateOrderLayer()
    {

    }
}
