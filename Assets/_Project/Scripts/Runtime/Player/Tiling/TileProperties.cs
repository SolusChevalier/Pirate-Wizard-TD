using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileProperties
{
    public Vector3 StartPos;
    public Transform PlacementPoint;
    public bool _selected;
    public bool canHover;
    public bool hover;
    public bool highLow;
    public bool Occupied;
    public Defender OccupyingUnit;
    public CoordStruct Coord;

    public void startProps(Vector3 stPos, Transform ppPoint)
    {
        StartPos = stPos;
        _selected = false;
        canHover = true;
        hover = false;
        highLow = false;
        Occupied = false;
        OccupyingUnit = null;
        PlacementPoint = ppPoint;
        Coord = new CoordStruct();
    }
}