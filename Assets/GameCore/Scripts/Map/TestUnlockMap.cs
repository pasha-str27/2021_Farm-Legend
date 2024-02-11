using UnityEngine;

public class TestUnlockMap : MonoBehaviour
{
    public BoundsInt area;

    private void Start()
    {
        Vector3Int posInt = GridBuildingSystem.instance.grid.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = posInt;
        Debug.Log("=> Area: " + areaTemp.position);
    }
}
