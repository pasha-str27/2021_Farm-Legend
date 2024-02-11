using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour
{
    public bool isNotMove;
    public bool placed;
    public BoundsInt area;

    private float timeMouse;
    private bool isMouseDowm;
    private bool instaceToShop;

    float defaultRotateX = 1;
    [SerializeField] Animator animMove;
    Vector3 vitricu;
    private void Awake()
    {
        if (transform.GetChild(0).GetComponent<Animator>() != null)
        {
            animMove = transform.GetChild(0).GetComponent<Animator>();
            animMove.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animMove.enabled = false;
        }
    }
    private void Start()
    {
        if (placed)
        {
            if (CanBePlaced())
                Place();
        }
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnCompliteUpMove, OnCompliteUpMoveHanlde);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompliteUpMove, OnCompliteUpMoveHanlde);

    }
    private void OnCompliteUpMoveHanlde(object obj)
    {
        if (GetComponent<BaseBuilding>() == null)
            return;

        var msg = (MessagerUpMove)obj;
        if (msg.idBuilding != GetComponent<BaseBuilding>().idBuilding)
            return;
        placed = false;
        SetLayerMove();
        GridBuildingSystem.instance.BeginMove(this);
        GridBuildingSystem.instance.DeleteArea(area);
        if (animMove != null)
        {
            animMove.enabled = true;
            animMove.SetBool("dungyen", false);
        }
        this.PostEvent((int)EventID.OnLockCamera, true);
    }
    public bool CanBePlaced()
    {
        Vector3Int posInt = GridBuildingSystem.instance.grid.LocalToCell(transform.position);
        //Debug.Log("=> posInt "+ posInt);
        BoundsInt areaTemp = area;
        areaTemp.position = posInt;

        if (GridBuildingSystem.instance.CanTakeArea(areaTemp))
        {
            return true;
        }
        return false;
    }
    public void SetPositon(Vector3Int pos, float rotate)
    {
        area.position = pos;
        transform.localScale = new Vector3(rotate, 1, 1);

        if (rotate < 0)
        {
            area.size = new Vector3Int(area.size.y, area.size.x, 1);
        }

        if (CanBePlaced())
        {
            Place();
        }
    }
    public void Place()
    {
        Vector3Int posInt = GridBuildingSystem.instance.grid.LocalToCell(transform.position);
        area.position = posInt;
        BoundsInt areaTemp = area;
        areaTemp.position = posInt;
        placed = true;

        defaultRotateX = transform.localScale.x;

        GridBuildingSystem.instance.TakeArea(areaTemp);
        if (GetComponent<GarbageController>())
        {
            GetComponent<GarbageController>().LoadObjectInMap();
        }
        GetComponent<BaseBuilding>()?.UpdateOrderLayer();
        if (animMove != null)
        {
            animMove.SetBool("datxuong", false);
            animMove.SetBool("dungyen", true);
            StartCoroutine(DelayDatXuong());
        }
    }

    IEnumerator DelayDatXuong()
    {
        yield return new WaitForSeconds(0.3f);
        if (animMove != null)
            animMove.SetBool("datxuong", true);
        yield return new WaitForSeconds(0.1f);
        if (animMove != null)
            animMove.enabled = false;
        if (GetComponent<SpriteOrder>() != null)
            GetComponent<SpriteOrder>().LoadOrder("Default");
    }

    public void SetLayerMove()
    {
        if (GetComponent<SpriteOrder>() != null)
            GetComponent<SpriteOrder>().LoadOrder("Middle");
    }
    public void Rotate()
    {
        if (transform.localScale.x > 0)
        {
            area.size = new Vector3Int(area.size.y, area.size.x, 1);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            area.size = new Vector3Int(area.size.y, area.size.x, 1);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    public void ResetRotate()
    {
        if (transform.localScale.x != defaultRotateX)
            area.size = new Vector3Int(area.size.y, area.size.x, 1);
        transform.localScale = new Vector3(defaultRotateX, 1, 1);
    }
    private void Update()
    {
        if (isNotMove)
            return;

        if (!instaceToShop)
        {
            isMouseDowm = false;
            animMove.enabled = true;
            GridBuildingSystem.instance.MoveBuilding(this);
            if (Input.GetMouseButtonUp(0))
            {
                instaceToShop = true;
                this.PostEvent((int)EventID.OnShowUIMove, true);
            }
        }
        if (Vector3.Distance(vitricu, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.2f)
        {
            if (isMouseDowm && !GameUIManager.IsShowUiMove && !Util.IsMouseOverUI)
            {
                timeMouse += Time.deltaTime;
                if (timeMouse > .5f)
                {
                    isMouseDowm = false;
                    this.PostEvent((int)EventID.OnShowUiUpMove, new MessagerUpMove { idBuilding = GetComponent<BaseBuilding>().idBuilding, pos = transform.position });
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (GridBuildingSystem.TempBuilding != null)
            {
                if (GridBuildingSystem.TempBuilding == this)
                    GridBuildingSystem.instance.EndDragMove(this);
            }
        }
    }

    private void OnMouseDown()
    {
        if (placed && !Util.IsMouseOverUI)
        {
            timeMouse = 0;
            isMouseDowm = true;
            vitricu = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        isMouseDowm = false;
        if (!placed)
        {

            this.PostEvent((int)EventID.OnClickObject, new MessageObject
            {
                pos = transform.position
            });
        }
    }

    private void OnMouseExit()
    {
        isMouseDowm = false;
    }

    private void OnMouseDrag()
    {
        if (!placed)
        {
            this.PostEvent((int)EventID.OnShowUIMove, false);
            GridBuildingSystem.instance.MoveBuilding(this);
        }
    }

    public void LoadingFirst()
    {
        instaceToShop = true;
    }
}
