using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class MapGenerator : MonoBehaviour
{
    #region FIELDS

    public int2 mapSize = new int2(10, 10);

    public GameObject[] TilePrefabs;
    private int[,] _obsMap;
    private int[,] _intMapRep;
    public GameObject pathSphere;
    public GameObject obstaclePrefab;
    public GameObject EmptySpawnPrefab;
    public GameObject TowerEven;
    public GameObject TowerOdd;
    public GameObject PortPrefab;
    public float MinObs = 0.35f;
    public float MaxObs = 0.65f;
    private float obstaclePercent => UnityEngine.Random.Range(MinObs, MaxObs);
    private int EvenOdd => ((mapSize.x % 2) == 0) ? 2 : 3;
    private int _NumberOfTilePrefabs => TilePrefabs.Length;
    public List<CoordStruct> allTileCoords;
    public Queue<CoordStruct> shuffledTileCoords;
    public int seed => UnityEngine.Random.Range(0, 1000000);
    public CoordStruct mapCentre;
    private string holderName = "Generated Map";
    private Transform mapHolder;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        _obsMap = new int[mapSize.x, mapSize.y];
        _intMapRep = new int[mapSize.x, mapSize.y];
        MapRepresentation.Instance.InitMapRep(mapSize.x, mapSize.y);
        GenerateMap();
    }

    #endregion UNITY METHODS

    #region METHODS

    public void ClearMap()
    {
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
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
        GameObject TowerObj;
        if (mapSize.x % 2 == 0)
        {
            TowerObj = TowerEven;
        }
        else
        {
            TowerObj = TowerOdd;
        }

        mapCentre = new CoordStruct((int)mapSize.x / 2, (int)mapSize.y / 2);
        Vector3 TowerPos = CoordStruct.CoordToPosition(mapSize, mapCentre.x, mapCentre.y);
        GameObject newTower = Instantiate(TowerObj, TowerPos, Quaternion.identity);
        _intMapRep[mapCentre.x, mapCentre.y] = (int)TileType.WizardTower;

        ClearMap();

        /*for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = CoordStruct.CoordToPosition(mapSize, x, y);
                GameObject newTile = Instantiate(TilePrefabs[0], tilePosition, Quaternion.identity);
                newTile.transform.parent = mapHolder;
            }
        }*/

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];
        SquareGrid squareGrid = new SquareGrid((int)mapSize.x, (int)mapSize.y);
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            CoordStruct randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                //Vector3 obstaclePosition = CoordStruct.CoordToPosition(mapSize, randomCoord.x, randomCoord.y);
                _obsMap[randomCoord.x, randomCoord.y] = 1;
                squareGrid.walls.Add(randomCoord);
                //GameObject newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * .5f, Quaternion.identity);
                //newObstacle.transform.parent = mapHolder;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
                _obsMap[randomCoord.x, randomCoord.y] = 0;
                try
                {
                    squareGrid.walls.Remove(randomCoord);
                }
                catch (Exception)
                {
                    Debug.Log("Tried to remove a wall that didnt exist");
                    throw;
                }
            }
        }

        CoordStruct CoordPort1;
        CoordStruct CoordPort2;
        CoordStruct CoordPort3;

        int TopThird = (int)mapSize.x / 3;
        int SideThird = (int)mapSize.y / 3;

        List<CoordStruct> PosssiblePort1Spawn = new List<CoordStruct>();
        for (int i = TopThird; i <= mapSize.x - TopThird - 1; i++)
        {
            if (_obsMap[i, (int)mapSize.y - 1] == 0)
            {
                PosssiblePort1Spawn.Add(new CoordStruct(i, (int)mapSize.y - 1));
            }
        }

        List<CoordStruct> PosssiblePort2Spawn = new List<CoordStruct>();
        for (int i = 0; i <= SideThird - 1; i++)
        {
            if (_obsMap[0, i] == 0)
            {
                PosssiblePort2Spawn.Add(new CoordStruct(0, i));
            }
        }

        List<CoordStruct> PosssiblePort3Spawn = new List<CoordStruct>();
        for (int i = 0; i <= SideThird - 1; i++)
        {
            if (_obsMap[(int)mapSize.x - 1, i] == 0)
            {
                PosssiblePort3Spawn.Add(new CoordStruct((int)mapSize.x - 1, i));
            }
        }
        CoordPort1 = PosssiblePort1Spawn[UnityEngine.Random.Range(0, PosssiblePort1Spawn.Count)];
        CoordPort2 = PosssiblePort2Spawn[UnityEngine.Random.Range(0, PosssiblePort2Spawn.Count)];
        CoordPort3 = PosssiblePort3Spawn[UnityEngine.Random.Range(0, PosssiblePort3Spawn.Count)];
        var astar = new AStarSearch(squareGrid, CoordPort1, mapCentre);
        for (int i = 0; i < astar.path.Count - EvenOdd; i++)
        {
            Vector3 tilePosition = CoordStruct.CoordToPosition(mapSize, astar.path[i].x, astar.path[i].y);
            //GameObject newObstacle = Instantiate(pathSphere, tilePosition + Vector3.up * .5f, Quaternion.identity);
            //newObstacle.transform.parent = mapHolder;
            GameObject newTile = Instantiate(TilePrefabs[1], tilePosition, Quaternion.identity);
            newTile.transform.parent = mapHolder;
            _intMapRep[astar.path[i].x, astar.path[i].y] = (int)TileType.Path;
            if (i == 0)
            {
                GameObject newPort = Instantiate(PortPrefab, tilePosition, Quaternion.Euler(0, 90, 0));
                newPort.transform.parent = mapHolder;
            }
        }

        var astar2 = new AStarSearch(squareGrid, CoordPort2, mapCentre);
        for (int i = 0; i < astar2.path.Count - EvenOdd; i++)
        {
            Vector3 tilePosition = CoordStruct.CoordToPosition(mapSize, astar2.path[i].x, astar2.path[i].y);
            //GameObject newObstacle = Instantiate(pathSphere, tilePosition + Vector3.up * .5f, Quaternion.identity);
            //newObstacle.transform.parent = mapHolder;
            GameObject newTile = Instantiate(TilePrefabs[1], tilePosition, Quaternion.identity);
            newTile.transform.parent = mapHolder;
            _intMapRep[astar2.path[i].x, astar2.path[i].y] = (int)TileType.Path;
            if (i == 0)
            {
                GameObject newPort = Instantiate(PortPrefab, tilePosition, Quaternion.identity);
                newPort.transform.parent = mapHolder;
            }
        }
        var astar3 = new AStarSearch(squareGrid, CoordPort3, mapCentre);
        for (int i = 0; i < astar3.path.Count - EvenOdd; i++)
        {
            Vector3 tilePosition = CoordStruct.CoordToPosition(mapSize, astar3.path[i].x, astar3.path[i].y);
            //GameObject newObstacle = Instantiate(pathSphere, tilePosition + Vector3.up * .5f, Quaternion.identity);
            //newObstacle.transform.parent = mapHolder;
            GameObject newTile = Instantiate(TilePrefabs[1], tilePosition, Quaternion.identity);
            _intMapRep[astar3.path[i].x, astar3.path[i].y] = (int)TileType.Path;
            newTile.transform.parent = mapHolder;
            if (i == 0)
            {
                GameObject newPort = Instantiate(PortPrefab, tilePosition, Quaternion.Euler(0, 180, 0));
                newPort.transform.parent = mapHolder;
            }
        }
        MapRepresentation.Instance.ConvertIntMapToTileTypeMap(_intMapRep);
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

    #endregion METHODS
}