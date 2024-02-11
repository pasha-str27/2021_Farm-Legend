using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSuggestions : MonoBehaviour
{
    [SerializeField] Image[] toggles;
    [SerializeField] GameObject btnNext;
    [SerializeField] Sprite spOn, spOff;
    [SerializeField] int tempIndex;
    int tempCount = 0;
    private void OnEnable()
    {
        tempIndex = 0;
        SelectToggle(0);
    }
    public void FillData(int count)
    {
        btnNext.SetActive(false);
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].gameObject.SetActive(false);
        }
        tempCount = count / 5;
        if (count % 5 > 0)
            tempCount++;
        for (int i = 0; i < tempCount; i++)
        {
            if (i > 0)
                btnNext.SetActive(true);
            if (i < toggles.Length)
                toggles[i].gameObject.SetActive(true);
        }
    }
    void SelectToggle(int index)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].sprite = spOff;
        }
        toggles[index].sprite = spOn;
    }
    public void Btn_Next_Click()
    {
        //Debug.Log("=> Btn_Next_Click" + tempIndex);
        if (tempIndex < toggles.Length - 1)
        {
            tempIndex++;
            if (!toggles[tempIndex].gameObject.activeInHierarchy)
                tempIndex = 0;
        }
        else tempIndex = 0;
        SelectToggle(tempIndex);
        this.PostEvent((int)EventID.OnLoadToggleSuggestion, tempIndex);
    }
}
