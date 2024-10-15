using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;

public class WaveFunctionCollapse : MonoBehaviour
{
    public MapRepresentation MapRepresentation;
    private TileType[,] tileMap;
    private int[,] entropyMap;
    private Dictionary<TileType, List<TileType>> tileConstraints;

    public void Start()
    {
        MapRepresentation = GetComponent<MapRepresentation>();
    }

    public void WaveFunctionCollapseStart()
    {
        tileMap = MapRepresentation.MapRep;
        entropyMap = new int[MapRepresentation.Width, MapRepresentation.Height];
        System.Random prng = new System.Random(MapRepresentation.Seed);
        tileConstraints = TileConstraints.GetTileConstraints();
        tileMap = RunWaveFunctionCollapse();
        MapRepresentation.MapRep = tileMap;
    }

    public TileType[,] RunWaveFunctionCollapse()
    {
        InitializeEntropyMap();
        bool collapsed = false;

        while (!collapsed)
        {
            // Step 1: Find the tile with the least possibilities
            var tileToCollapse = FindTileWithLeastPossibilities();

            // Step 2: Collapse this tile to a specific TileType
            CollapseTile(tileToCollapse.Item1, tileToCollapse.Item2);

            // Step 3: Propagate constraints to neighboring tiles
            Propagate(tileToCollapse.Item1, tileToCollapse.Item2);

            // Check if all tiles are collapsed
            collapsed = CheckIfAllCollapsed();
        }

        return tileMap;
    }

    private void InitializeEntropyMap()
    {
        for (int x = 0; x < MapRepresentation.Width; x++)
        {
            for (int y = 0; y < MapRepresentation.Height; y++)
            {
                entropyMap[x, y] = tileConstraints[tileMap[x, y]].Count + (int)MapUtils.CalculateHeuristic(x, y);
            }
        }
    }

    private (int, int) FindTileWithLeastPossibilities()
    {
        int minEntropy = int.MaxValue;
        int Ex = 0;
        int Ey = 0;

        for (int x = 0; x < MapRepresentation.Width; x++)
        {
            for (int y = 0; y < MapRepresentation.Height; y++)
            {
                if (entropyMap[x, y] < minEntropy)
                {
                    minEntropy = entropyMap[x, y];
                }
            }
        }
        return (Ex, Ey);
    }

    private void CollapseTile(int x, int y)
    {
        // Step 1: Get the neighbors using the static method from MapUtils
        List<CoordStruct> neighbors = MapUtils.GetNeighbors(new CoordStruct(x, y));

        // Step 2: Get the valid tile types for the current tile based on its neighbors
        List<TileType> possibleTiles = new List<TileType>(tileConstraints.Keys);

        foreach (var neighbor in neighbors)
        {
            int neighborX = neighbor.x;
            int neighborY = neighbor.y;
            TileType neighborType = tileMap[neighborX, neighborY]; // Get the neighbor's tile type

            if (neighborType != TileType.Empty)
            {
                // Step 3: Filter possibleTiles based on the valid neighbors of this tile type
                possibleTiles = possibleTiles.Intersect(tileConstraints[neighborType]).ToList();
            }
        }

        // Step 4: Randomly collapse to one of the possible tiles
        if (possibleTiles.Count > 0)
        {
            TileType selectedTile = possibleTiles[UnityEngine.Random.Range(0, possibleTiles.Count)];
            tileMap[x, y] = selectedTile; // Set the tileMap at (x, y) to the collapsed tile
        }
        else
        {
            // In case no valid tile exists (which should be rare), fallback to a default tile
            tileMap[x, y] = TileType.Grass;
        }
    }

    private void Propagate(int x, int y)
    {
        // Queue for tiles that need to be processed (propagated to neighbors)
        Queue<(int, int)> toPropagate = new Queue<(int, int)>();
        toPropagate.Enqueue((x, y));

        while (toPropagate.Count > 0)
        {
            // Get the next tile to propagate from the queue
            var currentTile = toPropagate.Dequeue();
            int currentX = currentTile.Item1;
            int currentY = currentTile.Item2;
            TileType currentTileType = tileMap[currentX, currentY];

            // Get all neighbors of the current tile
            List<CoordStruct> neighbors = MapUtils.GetNeighbors(new CoordStruct(currentX, currentY));

            // Loop through each neighbor and reduce possibilities based on the current tile type
            foreach (var neighbor in neighbors)
            {
                int neighborX = neighbor.x;
                int neighborY = neighbor.y;

                // Skip if the neighbor tile is already collapsed
                if (tileMap[neighborX, neighborY] != TileType.Empty)
                    continue;

                // Get the valid options for this neighbor based on the current tile
                List<TileType> validNeighborTypes = tileConstraints[currentTileType];

                // If the neighbor has fewer options after applying constraints, propagate further
                List<TileType> neighborPossibleTiles = GetPossibleTiles(neighborX, neighborY);
                List<TileType> filteredNeighborTiles = neighborPossibleTiles.Intersect(validNeighborTypes).ToList();

                if (filteredNeighborTiles.Count < neighborPossibleTiles.Count)
                {
                    // If there was a change in possibilities, update the possibilities for the neighbor
                    SetPossibleTiles(neighborX, neighborY, filteredNeighborTiles);

                    // Add the neighbor to the propagation queue to propagate further
                    toPropagate.Enqueue((neighborX, neighborY));
                }
            }
        }
    }

    // Method to get possible tile types for a given tile
    private List<TileType> GetPossibleTiles(int x, int y)
    {
        return new List<TileType> { TileType.Grass, TileType.Sand, TileType.Path }; // Example possible tiles
    }

    // Method to set the possible tiles for a given tile
    private void SetPossibleTiles(int x, int y, List<TileType> possibleTiles)
    {
    }

    private bool CheckIfAllCollapsed()
    {
        return false;
    }
}

public class TileConstraints
{
    public static Dictionary<TileType, List<TileType>> GetTileConstraints()
    {
        return new Dictionary<TileType, List<TileType>>()
        {
            { TileType.Empty, new List<TileType> { TileType.Grass, TileType.Sand } },
            { TileType.WizardTower, new List<TileType> { TileType.Grass, TileType.Sand } },
            { TileType.Grass, new List<TileType> { TileType.Grass, TileType.Path, TileType.Sand, TileType.PathBorder } },
            { TileType.Sand, new List<TileType> { TileType.Sand, TileType.PathBorder, TileType.Path } },
            { TileType.Path, new List<TileType> { TileType.Path, TileType.PathBorder } },
            { TileType.PathBorder, new List<TileType> { TileType.Path, TileType.Grass, TileType.Sand } }
        };
    }
}