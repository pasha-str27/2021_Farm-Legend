using UnityEngine;
using UnityEngine.UI;

public class UICage : MonoBehaviour
{
    public GameObject ButtonHide;
    public GameObject UISpeedUp;
    public GameObject UIRasingAndHarvest;

    [Header("UISpeedUp")]
    [SerializeField] private Button btAds;
    [SerializeField] private Text textName;
    [SerializeField] private Image imgProcess2;
    [SerializeField] private Text textTime;
    [SerializeField] private Button btTime;

    [Header("UIRaisingAndHarvest")]
    [SerializeField] private Image iconThuhoach;
    [SerializeField] private Image iconChoAn;
    [SerializeField] private Text textAmount;

    private int idCage = -1;

    private void Start()
    {
        ButtonHide.SetActive(false);
        UISpeedUp.SetActive(false);
        UIRasingAndHarvest.SetActive(false);
    }

    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnShowUICage, OnShowUICageHandle);
        this.RegisterListener((int)EventID.OnHideUICage, OnHideUICageHandle);
        this.RegisterListener((int)EventID.OnSendTimeCage, OnSendTimeCageHandle);
    }

    private void OnSendTimeCageHandle(object obj)
    {

    }

    private void OnHideUICageHandle(object obj)
    {
        UISpeedUp.SetActive(false);
        UIRasingAndHarvest.SetActive(false);
        idCage = -1;
    }

    private void OnShowUICageHandle(object obj)
    {
        idCage = (int)obj;
        int indexCage = PlayerPrefSave.GetTypeObject(idCage);
        ButtonHide.SetActive(true);
        if (PlayerPrefSave.GetTimeCage(idCage) >= DataManager.ProductAsset.list[indexCage].time)
        {
            // Thu hoach
            LoadThuHoach();
        }
        else if (PlayerPrefSave.GetTimeCage(idCage) > 0 && PlayerPrefSave.GetTimeCage(idCage) < DataManager.ProductAsset.list[indexCage].time)
        {
            // Tang toc
            LoadSpeedUp();
        }
        else
        {
            // Cho an
            LoadChoAn();
        }
    }

    private void LoadThuHoach()
    {
        UIRasingAndHarvest.SetActive(true);
        iconThuhoach.color = Color.white;
        iconChoAn.color = Color.cyan;
        iconThuhoach.GetComponent<DragUICage>().isLock = false;
        iconChoAn.GetComponent<DragUICage>().isLock = true;
    }

    private void LoadSpeedUp()
    {
        UISpeedUp.SetActive(true);
    }

    private void LoadChoAn()
    {
        UIRasingAndHarvest.SetActive(true);
        iconChoAn.color = Color.white;
        iconThuhoach.color = Color.cyan;
        iconThuhoach.GetComponent<DragUICage>().isLock = true;
        iconChoAn.GetComponent<DragUICage>().isLock = false;
    }

    public void ButtonHideHandle()
    {
        ButtonHide.SetActive(false);
        UISpeedUp.SetActive(false);
        UIRasingAndHarvest.SetActive(false);
    }
}
