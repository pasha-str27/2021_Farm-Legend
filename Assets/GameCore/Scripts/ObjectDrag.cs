using BitBenderGames;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    public ProductData data;

    private void OnMouseUp()
    {
        //this.PostEvent((int)EventID.OnLockCamera, false);
    }

    private void OnMouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LandUmbrella"))
        {
            collision.GetComponent<LandController>().Planting(data);
        }
    }
}
