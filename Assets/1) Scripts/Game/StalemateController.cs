using System.Collections.Generic;
using UnityEngine;

public class StalemateController : MonoBehaviour
{
    private Piece thisKing;
    private CheckController checkController;
    private List<Piece> myPieces = new List<Piece>();

    private BoardManager boardManager;

    private void OnEnable()
    {
        TurnManager.onTurnChanged += IsStalemate;
    }
    private void OnDisable()
    {
        TurnManager.onTurnChanged -= IsStalemate;
    }

    void Start()
    {
        thisKing = GetComponent<Piece>();
        checkController = GetComponent<CheckController>();
        boardManager = BoardManager.Instance;
        myPieces = new List<Piece>();
    }

    public void IsStalemate(PieceColor color)
    {
        if (thisKing == null || thisKing.pieceColor != color)
            return;

        if (boardManager == null)
            boardManager = BoardManager.Instance;
        if (checkController == null)
            checkController = GetComponent<CheckController>();

        if (checkController != null && checkController.IsKingInCheck(boardManager))
            return;

        var piecesOfColor = boardManager.GetAllPieces().FindAll(p => p.pieceColor == color);
        foreach (var piece in piecesOfColor)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int target = new Vector2Int(x, y);
                    if (piece.IsMoveValid(target))
                    {
                        return;
                    }
                }
            }
        }

        Debug.Log("Stalemate (pat) detected for " + color);
        GameManager.Instance.TriggerNoLegalMoves();
    }
}
