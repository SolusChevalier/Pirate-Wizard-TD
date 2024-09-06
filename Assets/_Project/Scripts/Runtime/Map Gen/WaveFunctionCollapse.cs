using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                entropyMap[x, y] = tileConstraints[tileMap[x, y]].Count;
            }
        }
    }

    private (int, int) FindTileWithLeastPossibilities()
    {
        return (0, 0); // Example
    }

    private void CollapseTile(int x, int y)
    {
        // Randomly collapse the tile at (x, y) into a valid TileType based on its neighbors
    }

    private void Propagate(int x, int y)
    {
        // Propagate constraints to neighboring tiles
    }

    private bool CheckIfAllCollapsed()
    {
        // Check if every tile in tileMap has been assigned a specific TileType
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