using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region FIELDS

    public float NormalSpeed = 1f;
    public float FastSpeed = 2f;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
    }

    #endregion UNITY METHODS

    #region METHODS

    public void SwitchToNormalSpeed()
    {
        Time.timeScale = NormalSpeed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void SwitchToFastSpeed()
    {
        Time.timeScale = FastSpeed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    #endregion METHODS
}