using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region FIELDS

    public int CurrentWave = 0;
    public int EnemiesLeft;
    public int NumEnemies = 0;
    public AiManager aiManager;
    public static int WaveCount = 0;
    public static int EnemiesKilled = 0;
    public GameObject GoblinPrefab, SapperPrefab, TrollPrefab;
    public Transform _Spawn1, _Spawn2, _Spawn3;
    public Transform _Destination1, _Destination2, _Destination3;
    public static List<Enemy> EnemyList = new List<Enemy>();
    public PlayerManager playerManager;

    #endregion FIELDS

    #region UNITY METHODS

    private void OnEnable()
    {
        EventManager.OnWaveStart += StartWave;
        EventManager.OnEnemyDestroyed += EnemyDestroyed;
    }

    private void OnDisable()
    {
        EventManager.OnWaveStart -= StartWave;
        EventManager.OnEnemyDestroyed -= EnemyDestroyed;
    }

    #endregion UNITY METHODS

    #region METHODS

    public void StartWave()
    {
        CoordStruct DestCoord1 = MapRepresentation.Path1[MapRepresentation.Path1.Count - 1];
        _Destination1 = MapRepresentation.TileMap[DestCoord1.x, DestCoord1.y].TopSpawnPoint;
        CoordStruct DestCoord2 = MapRepresentation.Path2[MapRepresentation.Path2.Count - 1];
        _Destination2 = MapRepresentation.TileMap[DestCoord2.x, DestCoord2.y].TopSpawnPoint;
        CoordStruct DestCoord3 = MapRepresentation.Path3[MapRepresentation.Path3.Count - 1];
        _Destination3 = MapRepresentation.TileMap[DestCoord3.x, DestCoord3.y].TopSpawnPoint;
        CoordStruct SpawnCoord1 = MapRepresentation.Path1[0];
        _Spawn1 = MapRepresentation.TileMap[SpawnCoord1.x, SpawnCoord1.y].TopSpawnPoint;
        CoordStruct SpawnCoord2 = MapRepresentation.Path2[0];
        _Spawn2 = MapRepresentation.TileMap[SpawnCoord2.x, SpawnCoord2.y].TopSpawnPoint;
        CoordStruct SpawnCoord3 = MapRepresentation.Path3[0];
        _Spawn3 = MapRepresentation.TileMap[SpawnCoord3.x, SpawnCoord3.y].TopSpawnPoint;
        CurrentWave++;

        NumEnemies = (int)(CurrentWave * 2f);
        EnemiesLeft = NumEnemies;
        EventManager.SetEnemyUI?.Invoke(EnemiesLeft);
        EventManager.SetWaveUI?.Invoke(CurrentWave);
        WaveCount++;
        //Debug.Log("Wave " + WaveCount + " Started");
        StartCoroutine(SpawnWave());
    }

    public void EnemyDestroyed(Enemy enemy)
    {
        switch (enemy.Type)
        {
            case EnemyType.Goblin:
                EventManager.AddMoney?.Invoke(10);
                break;

            case EnemyType.AlquaedaGoblin:
                EventManager.AddMoney?.Invoke(20);
                break;

            case EnemyType.Troll:
                EventManager.AddMoney?.Invoke(30);
                break;
        }
        EnemiesLeft--;
        EnemiesKilled++;
        EnemyList.Remove(enemy);
        if (enemy.ReachedTower)
            PlayerManager.AttackableEnemyList.Remove(enemy);
        EventManager.UpdateMoneyUI?.Invoke();
        EventManager.SetEnemyUI?.Invoke(EnemiesLeft);
        if (EnemiesLeft == 0)
        {
            EventManager.OnWaveCompleted?.Invoke();
        }
    }

    private void SpawnEnemy(GameObject prefab, Transform spawnPoint, Transform destination)
    {
        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().FinalDest = destination.position;
        enemy.GetComponent<Enemy>().PlayerManager = playerManager;
        enemy.GetComponent<Enemy>().MoveTo(destination.position);
        EnemyList.Add(enemy.GetComponent<Enemy>());
    }

    private IEnumerator SpawnWave()
    {
        Debug.Log("Spawning Wave");
        SpawnList spawnList1 = aiManager.MakeMove(MapRepresentation.BorderPath1);
        Debug.Log("SpawnList1: " + spawnList1.EnemySpawns.Count);
        SpawnList spawnList2 = aiManager.MakeMove(MapRepresentation.BorderPath2);
        Debug.Log("SpawnList2: " + spawnList2.EnemySpawns.Count);
        SpawnList spawnList3 = aiManager.MakeMove(MapRepresentation.BorderPath3);
        Debug.Log("SpawnList3: " + spawnList3.EnemySpawns.Count);
        foreach (EnemyType enemy in spawnList1.EnemySpawns)
        {
            if (enemy == EnemyType.Goblin)
            {
                SpawnEnemy(GoblinPrefab, _Spawn1, _Destination1);
            }
            else if (enemy == EnemyType.AlquaedaGoblin)
            {
                SpawnEnemy(SapperPrefab, _Spawn1, _Destination1);
            }
            else if (enemy == EnemyType.Troll)
            {
                SpawnEnemy(TrollPrefab, _Spawn1, _Destination1);
            }
            yield return new WaitForSeconds(0.5f);
        }
        foreach (EnemyType enemy in spawnList2.EnemySpawns)
        {
            if (enemy == EnemyType.Goblin)
            {
                SpawnEnemy(GoblinPrefab, _Spawn2, _Destination2);
            }
            else if (enemy == EnemyType.AlquaedaGoblin)
            {
                SpawnEnemy(SapperPrefab, _Spawn2, _Destination2);
            }
            else if (enemy == EnemyType.Troll)
            {
                SpawnEnemy(TrollPrefab, _Spawn2, _Destination2);
            }
            yield return new WaitForSeconds(0.5f);
        }
        foreach (EnemyType enemy in spawnList3.EnemySpawns)
        {
            if (enemy == EnemyType.Goblin)
            {
                SpawnEnemy(GoblinPrefab, _Spawn3, _Destination3);
            }
            else if (enemy == EnemyType.AlquaedaGoblin)
            {
                SpawnEnemy(SapperPrefab, _Spawn3, _Destination3);
            }
            else if (enemy == EnemyType.Troll)
            {
                SpawnEnemy(TrollPrefab, _Spawn3, _Destination3);
            }
            yield return new WaitForSeconds(0.5f);
        }
        /*for (int i = 0; i < NumEnemies; i++)
        {
            //Debug.Log("Spawning Enemy" + NumEnemies);
            int rand = UnityEngine.Random.Range(1, 4);

            switch (rand)
            {
                case 1:
                    SpawnEnemy(GoblinPrefab, _Spawn1, _Destination1);
                    break;

                case 2:
                    SpawnEnemy(SapperPrefab, _Spawn2, _Destination2);
                    break;

                case 3:
                    SpawnEnemy(TrollPrefab, _Spawn3, _Destination3);
                    break;
            }

            yield return new WaitForSeconds(3);
        }*/
    }

    #endregion METHODS
}