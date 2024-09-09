using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region FIELDS

    private NavMeshAgent _navMeshAgent;
    public EnemyType Type;
    public Vector3 FinalDest;
    public Defender CurrentlyAttacking;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        EventManager.OnDefenderDestroyed += (defender) =>
        {
            if (CurrentlyAttacking == defender)
            {
                //_navMeshAgent.isStopped = false;
                //CurrentlyAttacking = null;
            }
        };
    }

    public void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, FinalDest) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        _navMeshAgent.isStopped = true;
        if (other.CompareTag("Defender"))
        {
            CurrentlyAttacking = other.GetComponent<Defender>();
            _navMeshAgent.isStopped = true;
            StartCoroutine(Attack());
        }
    }

    #endregion UNITY METHODS

    #region METHODS

    public void EnemyAttack(Defender other)
    {
        CurrentlyAttacking = other.GetComponent<Defender>();
        _navMeshAgent.isStopped = true;
        StartCoroutine(Attack());
    }

    public void MoveTo(Vector3 target)
    {
        _navMeshAgent.SetDestination(FinalDest);
    }

    public void MoveBy(Vector3 target)
    {
        _navMeshAgent.Move(target);
    }

    public void OnDestroy()
    {
        EventManager.OnEnemyDestroyed?.Invoke(this);
    }

    public IEnumerator Attack()
    {
        while (CurrentlyAttacking != null)
        {
            CurrentlyAttacking.GetComponent<Defender>().TakeDamage(1);
            yield return new WaitForSeconds(1);
        }
    }
}

#endregion METHODS