using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    #region FIELDS

    public NavMeshAgent agent;
    public Transform point;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    #endregion UNITY METHODS

    #region METHODS

    private void Update()
    {
        agent.SetDestination(point.position);
    }

    #endregion METHODS
}