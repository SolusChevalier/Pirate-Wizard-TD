using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public static class MapUtils
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }

    public static T[,] Shuffle2DArray<T>(T[,] array, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                int randomIndexX = prng.Next(i, array.GetLength(0));
                int randomIndexY = prng.Next(j, array.GetLength(1));
                T tempItem = array[randomIndexX, randomIndexY];
                array[randomIndexX, randomIndexY] = array[i, j];
                array[i, j] = tempItem;
            }
        }

        return array;
    }

    public static bool IsInMap(int x, int y)
    {
        return x >= 0 && x < MapRepresentation.Width && y >= 0 && y < MapRepresentation.Height;
    }

    public static List<CoordStruct> GetNeighbors(CoordStruct Coord)
    {
        List<CoordStruct> neighbors = new List<CoordStruct>();
        if (IsInMap(Coord.x, Coord.y + 1))
        {
            CoordStruct Up = new CoordStruct(Coord.x, Coord.y + 1);
            neighbors.Add(Up);
        }
        if (IsInMap(Coord.x + 1, Coord.y))
        {
            CoordStruct Right = new CoordStruct(Coord.x + 1, Coord.y);
            neighbors.Add(Right);
        }
        if (IsInMap(Coord.x, Coord.y - 1))
        {
            CoordStruct Down = new CoordStruct(Coord.x, Coord.y - 1);
            neighbors.Add(Down);
        }
        if (IsInMap(Coord.x - 1, Coord.y))
        {
            CoordStruct Left = new CoordStruct(Coord.x - 1, Coord.y);
            neighbors.Add(Left);
        }
        return neighbors;
    }

    public static float CalculateDistance(int x1, int y1, int x2, int y2)
    {
        float xDistance = Mathf.Abs(x1 - x2);
        float yDistance = Mathf.Abs(y1 - y2);

        return xDistance + yDistance;
    }

    public static float CalculateHeuristic(int x1, int y1)
    {
        float xDistance = Mathf.Abs(x1 - MapRepresentation.Width / 2);
        float yDistance = Mathf.Abs(y1 - MapRepresentation.Height / 2);
        float dist = Mathf.Sqrt(xDistance * xDistance + yDistance * yDistance);

        return dist;
    }
}