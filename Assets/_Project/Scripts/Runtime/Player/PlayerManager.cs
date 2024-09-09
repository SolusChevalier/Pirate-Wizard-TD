using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region FIELDS

    public int Money;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        EventManager.AddMoney += AddMoney;
        EventManager.RemoveMoney += RemoveMoney;
    }

    #endregion UNITY METHODS

    #region METHODS

    public void AddMoney(int amount)
    {
        Money += amount;
    }

    public void RemoveMoney(int amount)
    {
        Money -= amount;
    }

    #endregion METHODS
}