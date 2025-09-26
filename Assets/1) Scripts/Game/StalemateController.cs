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
        // Do not cache myPieces permanently; we'll query fresh lists during each check
        myPieces = new List<Piece>();
    }

    // This method is subscribed to TurnManager.onTurnChanged and must match Action<PieceColor>
    // It only performs the stalemate check for the king that this component is attached to.
    public void IsStalemate(PieceColor color)
    {
        // Only run this controller's check for the king color it belongs to
        if (thisKing == null || thisKing.pieceColor != color)
            return;

        // Ensure references are available (Start might not have run yet in some execution orders)
        if (boardManager == null)
            boardManager = BoardManager.Instance;
        if (checkController == null)
            checkController = GetComponent<CheckController>();

        // If the king is in check, it's not stalemate
        if (checkController != null && checkController.IsKingInCheck(boardManager))
            return;

        // Query fresh piece list for the color each time
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
                        // Found at least one legal move -> not stalemate
                        return;
                    }
                }
            }
        }

        // No legal moves and king not in check -> stalemate
        Debug.Log("Stalemate (pat) detected for " + color);
        GameManager.Instance.TriggerNoLegalMoves();
    }
}
