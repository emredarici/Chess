using UnityEngine;

public enum GameMode
{
    None,
    PlayervsPlayer,
    PlayervsAI,
}

public class GameManager : Singeleton<GameManager>
{
    public GameMode currentGameMode = GameMode.None;
    public bool IsModeSet => currentGameMode != GameMode.None;


    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        Debug.Log("Game mode set to: " + mode);
    }

    protected override void Awake()
    {
        base.Awake();

    }
}
