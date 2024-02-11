using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiRequirements : MonoBehaviour
{
    [SerializeField] Text txtName;
    [SerializeField] Transform content;
    [SerializeField] ItemShowRequirement itemShowRequirement;

    public void Show(ProductData data)
    {
        content.RecycleChild();
        txtName.text = data.GetName;
        for (int i = 0; i < data.requirements.Count; i++)
        {
            var item = itemShowRequirement.Spawn(content);
            item.FillData(data.requirements[i]);
        }
    }
    public void ShowCoin(ProductData data)
    {
        content.RecycleChild();
        txtName.text = data.GetName;
        var item = itemShowRequirement.Spawn(content);
        item.FillData(data.price);
    }
    
}
