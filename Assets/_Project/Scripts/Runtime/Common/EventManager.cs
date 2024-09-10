using System;
using UnityEngine.InputSystem;

public static class EventManager
{
    public static Action OnGamePaused;
    public static Action OnGameResumed;
    public static Action OnGameStarted;
    public static Action OnWaveStart;
    public static Action OnGameEnded;
    public static Action<BuildingTile> OnTileSelected;
    public static Action OnTileDeselect;
    public static Action<int> AddMoney;
    public static Action<int> RemoveMoney;
    public static Action<int> SetMoneyUI;
    public static Action UpdateMoneyUI;
    public static Action OnMapGenerated;
    public static Action OnWaveCompleted;
    public static Action<int> SetWaveUI;
    public static Action<Enemy> OnEnemyDestroyed;
    public static Action<int> SetEnemyUI;
    public static Action<Defender> OnDefenderDestroyed;
    public static Action<Defender> OnDefenderPlaced;
    public static Action PlayBtnClicked;
    public static Action PauseBtnClicked;
    public static Action FastBtnClicked;
    public static Action<BuildingTile> BuyDefender;
    public static Action<BuildingTile> SellDefender;
    public static Action<BuildingTile> UpgradeDefender;
}