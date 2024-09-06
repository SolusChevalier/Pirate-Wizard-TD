using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[Serializable]
public struct CoordStruct
{
    #region FIELDS

    public int x;
    public int y;

    #endregion FIELDS

    #region METHODS

    public CoordStruct(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public static Vector3 CoordToPosition(int2 mapSize, int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    public override bool Equals(object obj)
    {
        return obj is CoordStruct coord &&
               x == coord.x &&
               y == coord.y;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return x + ", " + y;
    }

    public static bool operator ==(CoordStruct c1, CoordStruct c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(CoordStruct c1, CoordStruct c2)
    {
        return !(c1 == c2);
    }

    #endregion METHODS
}