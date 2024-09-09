using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Defender : MonoBehaviour
{
    #region FIELDS

    public int Health = 10;

    #endregion FIELDS

    #region UNITY METHODS

    private void OnDestroy()
    {
        EventManager.OnDefenderDestroyed?.Invoke(this);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goblin"))
        {
            other.GetComponent<Enemy>().EnemyAttack(this);
        }
    }

    #endregion UNITY METHODS

    #region METHODS

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    #endregion METHODS
}