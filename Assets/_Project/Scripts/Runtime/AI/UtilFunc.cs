using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilFunc : MonoBehaviour
{
    #region FIELDS

    public AiManager aiManager;
    public PlayerManager PlayerManager;

    [Header("Weights")]
    [SerializeField, Tooltip("Weight for Defender count.")]
    private float weightDefenderCount = 1f;

    [SerializeField, Tooltip("Weight for defender health.")]
    private float weightDefenderHealth = 1.5f;

    [SerializeField, Tooltip("Weight for defender total attack.")]
    private float weightDefenderAttack = 3f;

    [SerializeField, Tooltip("Weight for Tower health.")]
    private float weightTowerHealth = 1.2f;

    #endregion FIELDS

    #region METHODS

    public float EvaluateBoard(List<BuildingTile> path)
    {
        return DefenderCount(path) * weightDefenderCount + DefenderHealth(path) * weightDefenderHealth + DefenderAttack(path) * weightDefenderAttack + TowerHealth() * weightTowerHealth;
    }

    private float DefenderCount(List<BuildingTile> path)
    {
        float count = 0;
        foreach (var tile in path)
        {
            if (tile.properties.Occupied)
            {
                count += tile.properties.OccupyingUnit.weighting;
            }
        }
        return count;
    }

    private float DefenderHealth(List<BuildingTile> path)
    {
        float healthweight = 0;
        foreach (var tile in path)
        {
            if (tile.properties.Occupied)
            {
                healthweight += (tile.properties.OccupyingUnit.Health / tile.properties.OccupyingUnit.MaxHealth) * 100;
            }
        }
        return healthweight;
    }

    private float DefenderAttack(List<BuildingTile> path)
    {
        float attackweight = 0;
        foreach (var tile in path)
        {
            if (tile.properties.Occupied)
            {
                attackweight += tile.properties.OccupyingUnit.attackDamage;
            }
        }
        return attackweight;
    }

    private float TowerHealth()
    {
        return PlayerManager.TowerHealth;
    }

    public List<SpawnList> GetAllMoves()
    {
        List<SpawnList> spawnLists = new List<SpawnList>();
        float tmpBudget = aiManager.AiBudget;
        for (int i = 0; i < 25; i++)
        {
            SpawnList spawnList = new SpawnList();
            while (tmpBudget > 10)
            {
                int rand = Random.Range(0, 3);
                Debug.Log("Budget: " + tmpBudget);

                switch (rand)
                {
                    case 0:
                        if (tmpBudget >= 10)
                        {
                            spawnList.AddSpawn(EnemyType.Goblin);
                            tmpBudget -= 10;
                        }
                        break;

                    case 1:
                        if (tmpBudget >= 20)
                        {
                            spawnList.AddSpawn(EnemyType.AlquaedaGoblin);
                            tmpBudget -= 20;
                        }
                        break;

                    case 2:
                        if (tmpBudget >= 30)
                        {
                            spawnList.AddSpawn(EnemyType.Troll);
                            tmpBudget -= 30;
                        }
                        break;
                }
                spawnLists.Add(spawnList);
            }
            tmpBudget = aiManager.AiBudget;
        }
        return spawnLists;
    }

    #endregion METHODS
}