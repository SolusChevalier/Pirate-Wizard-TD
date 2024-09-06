using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapRepresentation : MonoBehaviour
{
    public static MapRepresentation Instance;

    private int Width;
    private int Height;
    public TileType[,] MapRep;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitMapRep(int width, int height)
    {
        Width = width;
        Height = height;
        MapRep = new TileType[width, height];
        IntializeMapRep();
    }

    private void IntializeMapRep()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                MapRep[x, y] = TileType.Empty;
            }
        }
    }

    public void ConvertIntMapToTileTypeMap(int[,] intMap)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                MapRep[x, y] = (TileType)intMap[x, y];
            }
        }
    }
}