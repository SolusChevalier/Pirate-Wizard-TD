using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceEnemy : MonoBehaviour
{
    #region FIELDS

    public Defender defender;
    public Vector3 target;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        defender = GetComponent<Defender>();
        target = this.transform.position + this.transform.forward;
    }

    private void Update()
    {
        if (defender.CurrentlyAttacking != null)
        {
            Vector3 target = defender.CurrentlyAttacking.transform.position;
            target.y = this.transform.position.y;
            this.transform.LookAt(target);
        }
    }

    #endregion UNITY METHODS
}