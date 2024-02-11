using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObject
{
    public Vector3 pos;
    public Action callBack;
    public int numTargetZoomCam;

    public ObjectMouseDown type;
    public string name;

    //old tree
    public GameObject wiltedTree;

    //crops
    public bool isHarvest;
    public bool isRaising;
    public int timeCount;
    public ProductData data;

    //animals
    public Sprite spRaising;
    public string nameKey;
    public string nameCage;

    //factory
    public int idFactory;

    //maplock
    public int idMap;

    public Building building;
}
