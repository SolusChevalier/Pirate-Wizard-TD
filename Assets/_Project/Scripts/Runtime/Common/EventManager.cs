using System;

public static class EventManager
{
    public static Action OnGamePaused;
    public static Action OnGameResumed;
    public static Action OnGameStarted;
    public static Action OnGameEnded;
    public static Action<Tile> OnTileClicked;
    public static Action<int> AddMoney;
    public static Action<int> RemoveMoney;
    public static Action OnMapGenerated;
    public static Action OnWaveCompleted;
    public static Action<Enemy> OnEnemyDestroyed;
    public static Action<Defender> OnDefenderDestroyed;
}