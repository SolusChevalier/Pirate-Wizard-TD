using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    #region UNITY METHODS

    private void OnMouseDown()
    {
        EventManager.PlayBtnClicked?.Invoke();
        Debug.Log("Play Button Clicked");
    }

    #endregion UNITY METHODS
}