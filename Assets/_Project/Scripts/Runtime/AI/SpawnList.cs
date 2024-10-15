using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnList
{
    public List<EnemyType> EnemySpawns;

    public SpawnList()
    {
        EnemySpawns = new List<EnemyType>();
    }

    public void AddSpawn(EnemyType enemy)
    {
        EnemySpawns.Add(enemy);
    }
}