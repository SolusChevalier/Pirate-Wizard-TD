using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMax : MonoBehaviour
{
    public UtilFunc utilFunc;

    public (float, SpawnList) MinimaxFunction(List<BuildingTile> path, int depth, bool isMaximizingPlayer, float alpha, float beta)
    {
        //Debug.Log(path.Count);
        if (depth == 0 || IsWinCondition(path))
        {
            return (utilFunc.EvaluateBoard(path), null);
        }

        List<SpawnList> possibleSpawns = utilFunc.GetAllMoves();
        float bestEval = isMaximizingPlayer ? float.MinValue : float.MaxValue;
        SpawnList bestSpawnList = new SpawnList();
        //Debug.Log(possibleSpawns.Count);
        foreach (SpawnList spawns in possibleSpawns)
        {
            List<BuildingTile> NewPath = ApplyMove(path, bestSpawnList);
            float eval = MinimaxFunction(NewPath, depth - 1, !isMaximizingPlayer, alpha, beta).Item1;

            if (isMaximizingPlayer)
            {
                if (eval > bestEval)
                {
                    bestEval = eval;
                    bestSpawnList = spawns;
                }
                alpha = Mathf.Max(alpha, eval);
            }
            else
            {
                if (eval < bestEval)
                {
                    bestEval = eval;
                    bestSpawnList = spawns;
                }
                beta = Mathf.Min(beta, eval);
            }

            if (beta <= alpha)
            {
                break; // Alpha-beta pruning
            }
        }

        return (bestEval, bestSpawnList);
    }

    public List<BuildingTile> ApplyMove(List<BuildingTile> path, SpawnList spawnList)
    {
        List<BuildingTile> newPath = new List<BuildingTile>();
        float health = 0;

        foreach (BuildingTile tile in path)
        {
            newPath.Add(tile.Clone());
            if (tile.properties.Occupied)
            {
                health += tile.properties.OccupyingUnit.Health;
            }
            //health += tile.properties.OccupyingUnit.Health;
        }
        foreach (EnemyType spawn in spawnList.EnemySpawns)
        {
            if (spawn == EnemyType.Troll)
            {
                health -= 50;
            }
            else if (spawn == EnemyType.AlquaedaGoblin)
            {
                health -= 20;
            }
            else if (spawn == EnemyType.Goblin)
            {
                health -= 10;
            }
        }
        return newPath;
    }

    private bool IsWinCondition(List<BuildingTile> path)
    {
        float count = 0;
        foreach (BuildingTile tile in path)
        {
            if (tile.properties.Occupied)
            {
                count++;
            }
        }
        return count == 0;
    }
}