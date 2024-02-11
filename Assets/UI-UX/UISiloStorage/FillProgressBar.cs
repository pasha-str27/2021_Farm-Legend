using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FillProgressBar : MonoBehaviour
{
    [SerializeField] Image imgFill;
    [SerializeField] Text txtCount;
    [SerializeField] Text txtCountUpgrade;
    [SerializeField] RectTransform objMax;
    [SerializeField] RectTransform objUpgrade;
    float tempFill = 0;
    int tempCount = 0;
    int updateNext = 0;
    public void UpdateFillBar(ObjectMouseDown objectMouseDown)
    {
        txtCount.text = DataManager.ProductAsset.GetTotal(objectMouseDown) + "/" + PlayerPrefSave.GetMaxStore(objectMouseDown);
        tempFill = (float)PlayerPrefSave.GetMaxStore(objectMouseDown) / (PlayerPrefSave.GetMaxStore(objectMouseDown)+ DataManager.GameConfig.GetStoreUpgrade(objectMouseDown));
        txtCountUpgrade.text = "+"+ DataManager.GameConfig.GetStoreUpgrade(objectMouseDown);
        imgFill.DOFillAmount(tempFill, .2f);
    }

    private void Update()
    {
        FillFolow(objMax);
        FillFolow(objUpgrade);
    }

    void FillFolow(RectTransform rect)
    {
        rect.anchorMin = new Vector2(imgFill.fillAmount, rect.anchorMin.y);
        rect.anchorMax = new Vector2(imgFill.fillAmount, rect.anchorMin.y);
    }
}
