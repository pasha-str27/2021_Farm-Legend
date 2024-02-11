using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsSound : MonoBehaviour
{
    [SerializeField] string name_sound_hammer="";
    [SerializeField] string name_sound_Saw = "";
    [SerializeField] string name_sound_Shovel = "";
    string temp;
    public void EventSound()
    {
        temp = name_sound_hammer;
        if (name.Contains("Saw"))
            temp = name_sound_Saw;
        if (name.Contains("Shovel"))
            temp = name_sound_Shovel;
        SoundManager.Play(temp);
    }
}
