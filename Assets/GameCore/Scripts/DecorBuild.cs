using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorBuild : BaseBuilding
{
    public override void SetNewID(int id)
    {
        base.SetNewID(id);
        idBuilding = id;
    }
    public override void SetOldID(int id)
    {
        base.SetOldID(id);
        idBuilding = id;
    }
}
