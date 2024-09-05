using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.Playables;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region FIELDS

    public Vector2 mapSize = new Vector2(10, 10);

    //public GameObject MapContainer;
    public GameObject[] TilePrefabs;

    public GameObject obstaclePrefab;
    public GameObject EmptySpawnPrefab;
    //public int[,] ObsMap;

    [Range(0, 1)]
    public float obstaclePercent;

    private int _NumberOfTilePrefabs => TilePrefabs.Length;
    public List<CoordStruct> allTileCoords;
    public Queue<CoordStruct> shuffledTileCoords;
    public int seed = 10;
    public CoordStruct mapCentre;
    public bool autoUpdate;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        //ObsMap = new int[(int)mapSize.x, (int)mapSize.y];
        GenerateMap();
    }

    #endregion UNITY METHODS

    #region METHODS

    public void ClearMap()
    {
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
    }

    public void GenerateEmptyMap()
    {
        string holderName = "Map Container";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                GameObject newTile = Instantiate(EmptySpawnPrefab, new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y), Quaternion.identity);
                newTile.transform.SetParent(mapHolder);
            }
        }
    }

    public void GenerateMap()
    {
        allTileCoords = new List<CoordStruct>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new CoordStruct(x, y));
            }
        }
        shuffledTileCoords = new Queue<CoordStruct>(MapUtils.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCentre = new CoordStruct((int)mapSize.x / 2, (int)mapSize.y / 2);

        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = CoordStruct.CoordToPosition(mapSize, x, y);
                GameObject newTile = Instantiate(TilePrefabs[0], tilePosition, Quaternion.identity);
                newTile.transform.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            CoordStruct randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordStruct.CoordToPosition(mapSize, randomCoord.x, randomCoord.y);
                //ObsMap[randomCoord.x, randomCoord.y] = 1;
                GameObject newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * .5f, Quaternion.identity);
                newObstacle.transform.parent = mapHolder;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                //ObsMap[randomCoord.x, randomCoord.y] = 0;
                currentObstacleCount--;
            }
        }

        //printObsMap();
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<CoordStruct> queue = new Queue<CoordStruct>();
        queue.Enqueue(mapCentre);
        mapFlags[mapCentre.x, mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            CoordStruct tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new CoordStruct(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    public CoordStruct GetRandomCoord()
    {
        CoordStruct randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    /*public void printObsMap()
    {
        string row = "";
        for (int i = 0; i <= (int)mapSize.x - 1; i++)
        {
            row = "";
            for (int j = 0; j <= (int)mapSize.y - 1; j++)
            {
                row += ObsMap[i, j] + " ";
            }
            Debug.Log(row);
        }
    }*/

    #endregion METHODS
}