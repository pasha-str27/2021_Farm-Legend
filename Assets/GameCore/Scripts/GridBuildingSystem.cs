using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System;
using BitBenderGames;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem instance;
    public static Building TempBuilding => instance.buildingMove;

    public Grid grid;
    public Tilemap mainmap;
    public Tilemap tempmap;

    private Building buildingMove;
    private Vector3 prevPos;
    private BoundsInt prevArea;
    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    private bool isNew;
    private ItemShop itemShop;

    private void Awake()
    {
        instance = this;
    }
    
    private void OnEnable()
    {
        string tilePath = @"Tiles\";
        try
        {
            tileBases.Add(TileType.Empty, null);
            tileBases.Add(TileType.White, Resources.Load<TileBase>(tilePath + "White"));
            tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "Green"));
            tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "Red"));
        }
        catch { Debug.Log("=====> catch GridBuildingSystem"); }
        

        this.RegisterListener((int)EventID.OnUIMoveOkay, OnUIMoveOkayHandle);
        this.RegisterListener((int)EventID.OnUIMoveRotate, OnUIMoveRotateHandle);
        this.RegisterListener((int)EventID.OnUIMoveCancel, OnUIMoveCancelHandle);
        this.RegisterListener((int)EventID.OnDragItem, OnDragItemHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUIMoveOkay, OnUIMoveOkayHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUIMoveRotate, OnUIMoveRotateHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUIMoveCancel, OnUIMoveCancelHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnDragItem, OnDragItemHandle);
    }
    private void OnDragItemHandle(object obj)
    {
        var msg = (MessagerDragItem)obj;
        if (msg.typeObject != ObjectMouseDown.Building)
            return;

        GameObject item = Instantiate(msg.obj, msg.pos, Quaternion.identity);
        item.transform.SetParent(transform);

        if (msg.item.shopData.typeShop == TypeShop.Animals)
        {
            item.GetComponent<AnimalDrag>().FillData(msg.item.shopData);
            return;
        }
        itemShop = msg.item;
        isNew = true;
        buildingMove = item.GetComponent<Building>();
    }

    private void Start()
    {
        Invoke("LoadMap", 1f);
    }

    private void LoadMap()
    {
        if (PlayerPrefSave.FirstGame == 0)
        {
            PlayerPrefSave.FirstGame = 1;

            PlayerPrefSave.TotalObject++;
            PlayerPrefSave.SetPositionX(PlayerPrefSave.TotalObject, 0f);
            PlayerPrefSave.SetPositionY(PlayerPrefSave.TotalObject, 1.5f);
            PlayerPrefSave.SetAreaX(PlayerPrefSave.TotalObject,2);
            PlayerPrefSave.SetAreaY(PlayerPrefSave.TotalObject, 2);
            PlayerPrefSave.SetTypeObject(PlayerPrefSave.TotalObject, 0);
            PlayerPrefSave.SetSeed(PlayerPrefSave.TotalObject, "");
            PlayerPrefSave.SetLocalScaleX(PlayerPrefSave.TotalObject, 1);

            PlayerPrefSave.TotalObject++;
            PlayerPrefSave.SetPositionX(PlayerPrefSave.TotalObject, 1.3f);
            PlayerPrefSave.SetPositionY(PlayerPrefSave.TotalObject, 2.25f);
            PlayerPrefSave.SetAreaX(PlayerPrefSave.TotalObject, 4);
            PlayerPrefSave.SetAreaY(PlayerPrefSave.TotalObject, 2);
            PlayerPrefSave.SetTypeObject(PlayerPrefSave.TotalObject, 0);
            PlayerPrefSave.SetSeed(PlayerPrefSave.TotalObject, "");
            PlayerPrefSave.SetLocalScaleX(PlayerPrefSave.TotalObject, 1);

            PlayerPrefSave.TotalObject++;
            PlayerPrefSave.SetPositionX(PlayerPrefSave.TotalObject, 0f);
            PlayerPrefSave.SetPositionY(PlayerPrefSave.TotalObject, 3f);
            PlayerPrefSave.SetAreaX(PlayerPrefSave.TotalObject, 4);
            PlayerPrefSave.SetAreaY(PlayerPrefSave.TotalObject, 4);
            PlayerPrefSave.SetTypeObject(PlayerPrefSave.TotalObject, 0);
            PlayerPrefSave.SetSeed(PlayerPrefSave.TotalObject, "");
            PlayerPrefSave.SetLocalScaleX(PlayerPrefSave.TotalObject, 1);

            PlayerPrefSave.TotalObject++;
            PlayerPrefSave.SetPositionX(PlayerPrefSave.TotalObject, -1.3f);
            PlayerPrefSave.SetPositionY(PlayerPrefSave.TotalObject, 2.25f);
            PlayerPrefSave.SetAreaX(PlayerPrefSave.TotalObject, 2);
            PlayerPrefSave.SetAreaY(PlayerPrefSave.TotalObject, 4);
            PlayerPrefSave.SetTypeObject(PlayerPrefSave.TotalObject, 0);
            PlayerPrefSave.SetSeed(PlayerPrefSave.TotalObject, "");
            PlayerPrefSave.SetLocalScaleX(PlayerPrefSave.TotalObject, 1);

            PlayerPrefSave.TotalObject++;
            PlayerPrefSave.SetAreaX(PlayerPrefSave.TotalObject, 8);
            PlayerPrefSave.SetAreaY(PlayerPrefSave.TotalObject, -4);
            PlayerPrefSave.SetTypeObject(PlayerPrefSave.TotalObject, 16);
            PlayerPrefSave.SetLocalScaleX(PlayerPrefSave.TotalObject, 1);
            for (int i = 0; i < 7; i++)
            {
                PlayerPrefSave.SetProductFactory(PlayerPrefSave.TotalObject, i, -1);
            }

            DataManager.ShopAsset.list[0].countBuild = 4;
            DataManager.ShopAsset.list[16].countBuild = 1;
        }

        Debug.Log("Total Object: " + PlayerPrefSave.TotalObject);

        for (int i = 1; i <= PlayerPrefSave.TotalObject; i++)
        {
            if (PlayerPrefSave.IsDeleteBuilding(i))
                continue;
            Vector3Int _areaPos = new Vector3Int(PlayerPrefSave.GetAreaX(i), PlayerPrefSave.GetAreaY(i), 0);
            Vector3 _posNormal = grid.CellToWorld(_areaPos);
            GameObject cloneVIP = Instantiate(DataManager.ShopAsset.list[PlayerPrefSave.GetTypeObject(i)].prefabs, _posNormal, Quaternion.identity);
            cloneVIP.transform.SetParent(transform);
            cloneVIP.GetComponent<Building>().LoadingFirst();
            cloneVIP.GetComponent<BaseBuilding>().SetOldID(i);
            cloneVIP.GetComponent<Building>().SetPositon(_areaPos, PlayerPrefSave.GetLocalScaleX(i));
        }
    }
    public void SetObjectInMap(Building building)
    {
        if (building.CanBePlaced())
        {
            building.Place();
            PlayerPrefSave.TotalObject++;
            PlayerPrefSave.SetTypeObject(PlayerPrefSave.TotalObject, itemShop.indexBuilding);
            building.GetComponent<BaseBuilding>().SetNewID(PlayerPrefSave.TotalObject);

            PlayerPrefSave.SetPositionX(PlayerPrefSave.TotalObject, building.gameObject.transform.position.x);
            PlayerPrefSave.SetPositionY(PlayerPrefSave.TotalObject, building.gameObject.transform.position.y);
            PlayerPrefSave.SetAreaX(PlayerPrefSave.TotalObject, building.area.position.x);
            PlayerPrefSave.SetAreaY(PlayerPrefSave.TotalObject, building.area.position.y);
            Destroy(building.gameObject);
        }
    }
    public void OnUIMoveOkayHandle(object obj)
    {
        if (buildingMove == null)
            return;
        if (buildingMove.CanBePlaced())
        {
            buildingMove.Place();
            BaseBuilding baseBuilding = buildingMove.GetComponent<BaseBuilding>();
            if (isNew && itemShop != null)
            {
                if (PlayerPrefSave.Coin < itemShop.shopData.GetPrice)
                {
                    UIToast.Show("Not enought coin!", null, ToastType.Notification, 1.5f);
                        this.PostEvent((int)EventID.OnShowVideoReward);

                    return;
                }

                CoinManager.AddCoin(-itemShop.shopData.GetPrice);
                CoinManager.AddExp(itemShop.shopData.exp, buildingMove.transform);
                isNew = false;
                PlayerPrefSave.TotalObject++;
                PlayerPrefSave.SetTypeObject(PlayerPrefSave.TotalObject, itemShop.indexBuilding);
                baseBuilding.SetNewID(PlayerPrefSave.TotalObject);
                itemShop.shopData.countBuild++;
                
                this.PostEvent((int)EventID.OnFxPutCoin, buildingMove.transform.position);

                AnalyticsManager.LogEvent("build_object", new Dictionary<string, object> {
            { "level", PlayerPrefSave.Level },
            { "build_name", itemShop.shopData.name },
            { "build_cost", itemShop.shopData.GetPrice }
                });
            }

            PlayerPrefSave.SetPositionX(baseBuilding.idBuilding, buildingMove.gameObject.transform.position.x);
            PlayerPrefSave.SetPositionY(baseBuilding.idBuilding, buildingMove.gameObject.transform.position.y);
            PlayerPrefSave.SetAreaX(baseBuilding.idBuilding, buildingMove.area.position.x);
            PlayerPrefSave.SetAreaY(baseBuilding.idBuilding, buildingMove.area.position.y);
            PlayerPrefSave.SetLocalScaleX(baseBuilding.idBuilding, buildingMove.gameObject.transform.localScale.x);
            
            EndMove();

            if (!PlayerPrefSave.IsTutorial)
                return;
            if (PlayerPrefSave.stepTutorial == 1 || PlayerPrefSave.stepTutorial == 6)
            {
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 3:
                        PlayerPrefSave.stepTutorialCurrent = 4;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
            }

            if (PlayerPrefSave.stepTutorial == 4)
            {
                switch (PlayerPrefSave.stepTutorialCurrent)
                {
                    case 4:
                        PlayerPrefSave.stepTutorialCurrent = 5;
                        this.PostEvent((int)EventID.OnLoadTutorial);
                        break;
                }
            }
        }
        else
        {
            Debug.Log("=> không để được");
            UIToast.Show("Not enough space!", null, ToastType.Notification, 1.5f);
        }
    }


    public void OnUIMoveRotateHandle(object obj)
    {
        buildingMove.Rotate();
        CheckPlacement();
    }

    public void OnUIMoveCancelHandle(object obj)
    {
        if (isNew)
        {
            isNew = false;
            Destroy(buildingMove.gameObject);
        }
        else
        {
            buildingMove.ResetRotate();
            buildingMove.transform.localPosition = prevPos;
            buildingMove.placed = true;
            if (buildingMove.CanBePlaced())
            {
                buildingMove.Place();
            }
        }
        SetTilesBlock(prevArea, TileType.Empty, tempmap);
        EndMove();
    }

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;
        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }

    private static void FillTiles(TileBase[] arr, TileType type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }
    TileBase[] toClear;
    private void ClearArea()
    {
        toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        tempmap.SetTilesBlock(prevArea, toClear);
    }
    BoundsInt buildingArea;
    TileBase[] baseArray;
    TileBase[] tileArray;
    int size;
    private void CheckPlacement()
    {
        ClearArea();
        buildingMove.area.position = grid.WorldToCell(buildingMove.gameObject.transform.position);
        buildingArea = buildingMove.area;

        baseArray = GetTilesBlock(buildingArea, mainmap);

        size = baseArray.Length;
        tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType.White])
            {
                tileArray[i] = tileBases[TileType.Green];
            }
            else
            {
                tileArray[i] = tileBases[TileType.Red];
            }
        }

        tempmap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }

    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTilesBlock(area, mainmap);
        foreach (var b in baseArray)
        {
            if (b != tileBases[TileType.White])
            {
                return false;
            }
        }
        return true;
    }

    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, tempmap);
        SetTilesBlock(area, TileType.Green, mainmap);
        if (area != prevArea)
            ClearArea();
    }

    public void BeginMove(Building building)
    {
        this.buildingMove = building;
        prevPos = buildingMove.transform.localPosition;
        MobileTouchCamera.isMoveObject = true;
    }
    public void EndMove()
    {
        MobileTouchCamera.isMoveObject = false;
        this.PostEvent((int)EventID.OnClickObject, new MessageObject
        {
            pos = buildingMove.transform.position
        });
        buildingMove = null;
        this.PostEvent((int)EventID.OnShowUIMove, false);
    }
    public void EndDragMove(Building building)
    {
        CheckPlacement();
        this.PostEvent((int)EventID.OnShowUIMove, true);
    }
    public void DeleteArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.White, mainmap);
    }
    Vector3 mousePos;
    Vector2 touchPos;
    Vector3Int cellPos;
    public void MoveBuilding(Building building)
    {
        this.buildingMove = building;
        mousePos = Input.mousePosition;
        mousePos.z = 10;
        touchPos = Camera.main.ScreenToWorldPoint(mousePos);
        cellPos = grid.LocalToCell(touchPos);
        buildingMove.transform.localPosition = grid.CellToLocalInterpolated(cellPos);
        CheckPlacement();
    }

    public Vector3 GetPosCell(int areaX, int areaY)
    {
        Vector3Int _areaPos = new Vector3Int(areaX, areaY, 0);
        Vector3 _posNormal = grid.CellToWorld(_areaPos);
        _posNormal.y += grid.cellSize.y / 2;
        return _posNormal;
    }
}

public enum TileType
{
    Empty,
    White,
    Green,
    Red
}
