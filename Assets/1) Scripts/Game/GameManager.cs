using UnityEngine;
using System;
using System.Collections;
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

    private MinimaxAI ai;
    public PieceColor aiColor = PieceColor.Black;
    public int aiDepth = 3;

    protected override void Awake()
    {
        base.Awake();
        TurnManager.ResetTurn();
        ai = new MinimaxAI(PieceColor.Black, aiDepth);
    }

    void OnEnable()
    {
        TurnManager.onTurnChanged += OnTurnChanged;
        BoardManager.PromoteRequested += OnPromoteRequested;
    }

    private void OnDisable()
    {
        TurnManager.onTurnChanged -= OnTurnChanged;
        BoardManager.PromoteRequested -= OnPromoteRequested;
    }

    private void OnTurnChanged(PieceColor color)
    {
        if (currentGameMode == GameMode.PlayervsAI && color == aiColor)
        {
            StartCoroutine(HandleAITurn());
        }
    }

    private IEnumerator HandleAITurn()
    {
        yield return new WaitForSeconds(0.25f);

        var move = ai.GetBestMove(BoardManager.Instance);

        var piece = BoardManager.Instance.GetPiece(move.from);
        if (piece != null)
        {
            BoardManager.Instance.OnPieceDropped(piece, move.to);
            TurnManager.SwitchTurn();

        }
        else
        {
            Debug.LogWarning($"AI seçtiği karede taş bulamadı: {move.from}");
        }
    }

    private void OnPromoteRequested(Piece pawn)
    {
        if (pawn == null) return;
        if (pawn.pieceColor != aiColor) return;

        var pos = pawn.currentPosition;

        Destroy(pawn.gameObject);
        if (BoardManager.Instance.pieces[pos.x, pos.y] == pawn)
            BoardManager.Instance.pieces[pos.x, pos.y] = null;

        BoardManager.Instance.PlacePiece(PieceType.Queen, pawn.pieceColor, pos);

        Debug.Log($"AI otomatik promoteyapıldı: {pawn.pieceColor} -> Queen at {pos}");
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

    public void GitHubLink()
    {
        Application.OpenURL("https://github.com/emredarici");
    }

    public void LinkedInLink()
    {
        Application.OpenURL("https://www.linkedin.com/in/emredarici0/");
    }
}
