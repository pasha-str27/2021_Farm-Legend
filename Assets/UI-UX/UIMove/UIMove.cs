using BitBenderGames;
using UnityEngine;

public class UIMove : MonoBehaviour
{
    [SerializeField] UIAnimation uIAnimation;

    public void Show()
    {
        uIAnimation.Show();

        this.PostEvent((int)EventID.OnClickObject, new MessageObject
        {
            pos = GridBuildingSystem.TempBuilding.transform.position
        }); ;

        if (!PlayerPrefSave.IsTutorial)
            return;
        if (PlayerPrefSave.stepTutorial == 1|| PlayerPrefSave.stepTutorial == 6)
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 2:
                    PlayerPrefSave.stepTutorialCurrent = 3;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
        if (PlayerPrefSave.stepTutorial == 4)
        {
            switch (PlayerPrefSave.stepTutorialCurrent)
            {
                case 3:
                    PlayerPrefSave.stepTutorialCurrent = 4;
                    this.PostEvent((int)EventID.OnLoadTutorial);
                    break;
            }
        }
    }
    public void Hide()
    {
        uIAnimation.Hide();
        if(GridBuildingSystem.instance!= null)
        if(GridBuildingSystem.TempBuilding == null)
            this.PostEvent((int)EventID.OnLockCamera, false);
    }

    public void OkayHandle()
    {
        this.PostEvent((int)EventID.OnUIMoveOkay);
    }

    public void RotateHandle()
    {
        if (PlayerPrefSave.IsTutorial)
            return;
        this.PostEvent((int)EventID.OnUIMoveRotate);
    }

    public void CancelHandle()
    {
        if (PlayerPrefSave.IsTutorial)
            return;
        this.PostEvent((int)EventID.OnUIMoveCancel);
    }
}
