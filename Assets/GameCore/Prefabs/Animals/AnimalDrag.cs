using UnityEngine;

public class AnimalDrag : MonoBehaviour
{
    public bool isTrigger;
    public string nameProduct;
    public CageController cageController;
    public GameObject animalPrefab;
    public ShopData shopData;
    public void FillData(ShopData shopData)
    {
        this.shopData = shopData;
        isTrigger = false;
    }

    private void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
        if (Input.GetMouseButtonUp(0))
        {
            this.PostEvent((int)EventID.OnLockCamera, false);

            if (isTrigger)
            {
                if(cageController != null)
                {
                    cageController.AddCage(this);
                }
            }
            else
            {
                if (PlayerPrefSave.stepTutorial == 2)
                {
                    switch (PlayerPrefSave.stepTutorialCurrent)
                    {
                        case 3:
                            PlayerPrefSave.stepTutorialCurrent = 0;
                            this.PostEvent((int)EventID.OnLoadTutorial);
                            break;
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
