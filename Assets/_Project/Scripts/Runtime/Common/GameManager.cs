using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void Awake()
    {
        EventManager.OnMapGenerated += GameStart;
    }

    public void GameStart()
    {
        EventManager.OnGameStarted?.Invoke();
    }
}