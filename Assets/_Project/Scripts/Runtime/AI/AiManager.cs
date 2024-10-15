using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class AiManager : MonoBehaviour
{
    #region FIELDS

    public EnemyManager enemyManager;
    public MiniMax miniMax;
    public float AiDifficulty = 1;
    public float AiBudget = 1;
    public int miniMaxDepth = 4;

    #endregion FIELDS

    #region UNITY METHODS

    private void OnEnable()
    {
        EventManager.OnWaveCompleted += UpdateBudget;
    }

    private void OnDisable()
    {
        EventManager.OnWaveCompleted -= UpdateBudget;
    }

    #endregion UNITY METHODS

    #region METHODS

    public void UpdateBudget()
    {
        AiBudget = Mathf.Log(AiDifficulty * enemyManager.CurrentWave + 35);
    }

    public SpawnList MakeMove(List<BuildingTile> path)
    {
        //Debug.Log(path.Count);

        var bestMove = miniMax.MinimaxFunction(path, miniMaxDepth, true, float.MinValue, float.MaxValue);
        Debug.Log(bestMove.Item2.EnemySpawns.Count);
        if (bestMove.Item2 == null)
        {
            return new SpawnList();
        }

        return bestMove.Item2;
    }

    #endregion METHODS
}