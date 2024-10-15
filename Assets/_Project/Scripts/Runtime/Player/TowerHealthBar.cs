using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHealthBar : MonoBehaviour
{
    #region FIELDS

    public GameObject bar;
    public float MaxScale = 10.0f;
    public PlayerManager tower;
    private float MaxHealth = 100;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        tower = FindObjectOfType<PlayerManager>();
        MaxHealth = tower.MaxTowerHealth;
    }

    private void Update()
    {
        bar.transform.localScale = new Vector3((tower.TowerHealth / MaxHealth) * MaxScale, bar.transform.localScale.y, bar.transform.localScale.z);
    }

    #endregion UNITY METHODS
}