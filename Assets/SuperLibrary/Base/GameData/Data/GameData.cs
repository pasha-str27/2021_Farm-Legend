using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public UserData user = new UserData();
    public List<StageSaveData> stages = new List<StageSaveData>();
    public List<SaveData> agricultural = new List<SaveData>();
}
