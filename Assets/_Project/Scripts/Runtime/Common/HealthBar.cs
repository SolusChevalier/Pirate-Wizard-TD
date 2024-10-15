using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region FIELDS

    public GameObject bar;
    public float MaxScale = 10.0f;
    public Enemy Enemy;
    private float MaxHealth = 100;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        MaxHealth = Enemy.Health;
    }

    private void Update()
    {
        bar.transform.localScale = new Vector3((Enemy.Health / MaxHealth) * MaxScale, bar.transform.localScale.y, bar.transform.localScale.z);
    }

    #endregion UNITY METHODS
}