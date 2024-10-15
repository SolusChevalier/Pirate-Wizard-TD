using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    #region FIELDS

    public Camera mainCam;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        this.transform.LookAt(mainCam.transform.position);
    }

    #endregion UNITY METHODS
}