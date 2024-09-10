using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameState gameState = GameState.BuildingPhase;

    private void OnEnable()
    {
        EventManager.OnMapGenerated += GameStart;
    }

    private void OnDisable()
    {
        EventManager.OnMapGenerated -= GameStart;
    }

    public void GameStart()
    {
        EventManager.OnGameStarted?.Invoke();
    }
}