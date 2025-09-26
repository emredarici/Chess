using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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

    protected override void Awake()
    {
        base.Awake();
        TurnManager.ResetTurn();
    }

    public static event Action<PieceColor> onCheckmate;

    public void TriggerCheckmate(PieceColor winner)
    {
        onCheckmate?.Invoke(winner);
    }

    public static event Action onOnly2Kings;

    public void TriggerOnly2Kings()
    {
        onOnly2Kings?.Invoke();
    }

    public static event Action onNoLegalMoves;

    public void TriggerNoLegalMoves()
    {
        onNoLegalMoves?.Invoke();
    }

    public static event Action onThirdRepetition;

    public void TriggerThirdRepetition()
    {
        onThirdRepetition?.Invoke();
    }

    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        Debug.Log("Game mode set to: " + mode);
    }

    public void SetPlayerVsPlayerMode() =>
        SetGameMode(GameMode.PlayervsPlayer);

    public void SetPlayerVsAIMode() =>
        SetGameMode(GameMode.PlayervsAI);


    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
