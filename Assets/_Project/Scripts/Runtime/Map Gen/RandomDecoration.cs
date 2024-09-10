using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDecoration : MonoBehaviour
{
    #region FIELDS

    public Transform SpawnPoint;
    public GameObject[] DecorationPrefabs;
    public int SpawnChance = 15;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        int randomIndex = Random.Range(0, 100);
        if (randomIndex < SpawnChance)
            SpawnDecoration();
    }

    #endregion UNITY METHODS

    #region METHODS

    private void SpawnDecoration()
    {
        int randomIndex = Random.Range(0, DecorationPrefabs.Length);
        float randomRotation = Random.Range(0, 360);
        GameObject decoration = Instantiate(DecorationPrefabs[randomIndex], SpawnPoint.position, Quaternion.Euler(0, randomRotation, 0));
        decoration.transform.SetParent(SpawnPoint);
    }

    #endregion METHODS
}