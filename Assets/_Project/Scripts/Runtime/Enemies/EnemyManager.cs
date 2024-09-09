using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region FIELDS

    public int CurrentWave = 1;
    public int EnemiesLeft;
    public GameObject GoblinPrefab;
    public Transform _Spawn1, _Spawn2, _Spawn3;
    public Transform _Destination1, _Destination2, _Destination3;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        EventManager.OnGameStarted += StartWave;
        EventManager.OnEnemyDestroyed += EnemyDestroyed;
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
        EnemiesLeft = CurrentWave * 2;
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
        if (EnemiesLeft == 0)
        {
            EventManager.OnWaveCompleted?.Invoke();
        }
    }

    private void SpawnEnemy(GameObject prefab, Transform spawnPoint, Transform destination)
    {
        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().FinalDest = destination.position;
        enemy.GetComponent<Enemy>().MoveTo(destination.position);
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < EnemiesLeft; i++)
        {
            SpawnEnemy(GoblinPrefab, _Spawn1, _Destination1);
            SpawnEnemy(GoblinPrefab, _Spawn2, _Destination2);
            SpawnEnemy(GoblinPrefab, _Spawn3, _Destination3);
            yield return new WaitForSeconds(3);
        }
    }

    #endregion METHODS
}