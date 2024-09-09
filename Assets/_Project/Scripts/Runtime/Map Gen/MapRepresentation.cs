using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapRepresentation : MonoBehaviour
{
    public static int Width;
    public static int Height;
    public static TileType[,] MapRep;
    public static Tile[,] TileMap;
    public int Seed;
    public static List<CoordStruct> Path1;
    public static List<CoordStruct> Path2;
    public static List<CoordStruct> Path3;

    public void InitMapRep(int width, int height, int seed)
    {
        Width = width;
        Height = height;
        Seed = seed;
        MapRep = new TileType[width, height];
        TileMap = new Tile[width, height];
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

    public void SaveMap(int[,] intMapRep, List<CoordStruct> path1, List<CoordStruct> path2, List<CoordStruct> path3)
    {
        ConvertIntMapToTileTypeMap(intMapRep);
        Path1 = path1;
        Path2 = path2;
        Path3 = path3;
    }

    private void ConvertIntMapToTileTypeMap(int[,] intMap)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                MapRep[x, y] = (TileType)intMap[x, y];
            }
        }
    }

    public void LoadTileMap(Tile[,] LoadedTileMap)
    {
        TileMap = LoadedTileMap;
    }

    public CoordStruct[] GetNeighbors(CoordStruct coord)
    {
        CoordStruct[] neighbors = new CoordStruct[4];
        neighbors[0] = new CoordStruct(coord.x + 1, coord.y);
        neighbors[1] = new CoordStruct(coord.x, coord.y - 1);
        neighbors[2] = new CoordStruct(coord.x - 1, coord.y);
        neighbors[3] = new CoordStruct(coord.x, coord.y + 1);
        return neighbors;
    }
}