using BitBenderGames;
using UnityEngine;

public class DragCageThuhoach : MonoBehaviour
{
    private void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
        if (Input.GetMouseButtonUp(0))
        {
            this.PostEvent((int)EventID.OnLockCamera, false);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cage"))
        {
            collision.GetComponent<CageController>().Harvest();
        }
    }
}
