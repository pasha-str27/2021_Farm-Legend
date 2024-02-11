using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemAchievement : MonoBehaviour
{
    //[SerializeField] Image icon;
    [SerializeField] Text txtName;
    [SerializeField] Text txtDes;
    [SerializeField] Text txtExp;
    [SerializeField] Text txtDiamond;
    [SerializeField] Button btnClaim;
    [SerializeField] GameObject objProgress;
    [SerializeField] Text txtCount;
    [SerializeField] Image fillProgress;
    [ReadOnly] [SerializeField] AchievementData achievementData;

    float tempFill = 0;
    public void FillData(AchievementData achievementData)
    {
        this.achievementData = achievementData;
        //icon.sprite = achievementData.icon;
        txtName.text = achievementData.GetName;
        txtDes.text = achievementData.GetDes;
        txtExp.text = achievementData.exp + "";
        LoadUiItem();
    }

    public void Btn_Claim_Click()
    {
        if (achievementData.countAchire >= achievementData.maxAchire)
        {
            achievementData.level++;
            achievementData.countAchire = 0;
            CoinManager.AddExp(achievementData.getExp, transform, null, "ads");
            CoinManager.AddDiamond(achievementData.getDiamond, transform, null, "ads");
            LoadUiItem();

            AnalyticsManager.LogEvent("Claim_achiement", new Dictionary<string, object> {
            { "name", achievementData.name },
            { "level", achievementData.level } });
        }
    }

    public void LoadUiItem()
    {
        btnClaim.gameObject.SetActive(false);
        txtDes.gameObject.SetActive(true);
        objProgress.SetActive(true);

        if (achievementData.description == "")
            txtDes.gameObject.SetActive(false);

        int tempCount = achievementData.countAchire;

        txtCount.text = tempCount + "/" + achievementData.maxAchire;
        tempFill = (float)tempCount / achievementData.maxAchire;
        fillProgress.fillAmount = tempFill;

        txtExp.text = achievementData.getExp + "";
        txtDiamond.text = achievementData.getDiamond + "";

        if (tempCount >= achievementData.maxAchire)
        {
            btnClaim.gameObject.SetActive(true);
            objProgress.SetActive(false);
            txtDes.gameObject.SetActive(false);
        }
    }
}
