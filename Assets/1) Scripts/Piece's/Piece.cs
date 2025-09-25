using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

//This class is must be attached to all chess pieces
public class Piece : MonoBehaviour, IPointerClickHandler
{
    public BoardManager board;
    public PieceType pieceType;
    public PieceColor pieceColor;
    public Vector2Int currentPosition;
    // Tracks whether this piece has moved at least once. Used for pawn double-step and castling logic.
    public bool hasMoved = false; // default false

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{pieceType} tıklandı: {currentPosition}");

        // If another piece is already selected and it is not this piece,
        // attempt to move the selected piece to this piece's square (capture).
        var manager = PieceSelectionManager.Instance;
        if (manager != null && manager.selectedPiece != null && manager.selectedPiece != this)
        {
            // If it's not the selected piece's turn, clear selection and indicators.
            if (manager.selectedPiece.pieceColor != TurnManager.currentTurn)
            {
                manager.selectedPiece = null;
                manager.ClearIndicators();
                return;
            }

            // Try to move the selected piece to the clicked piece's position.
            manager.TryMoveSelectedPiece(currentPosition);
            return;
        }

        // Otherwise, perform the normal selection behavior.
        PieceSelectionManager.Instance.SelectedPiece(this);
    }

    private IPieceMover pieceMover;

    public void SetMover(IPieceMover mover)
    {
        pieceMover = mover;
    }

    public bool IsMoveValid(Vector2Int targetPosition)
    {
        if (pieceMover == null)
        {
            Debug.LogError("Piece mover not set for " + pieceType);
            return false;
        }
        bool result = pieceMover.IsValidMove(currentPosition, targetPosition, board);
        if (BoardManager.VerboseLogging)
            Debug.Log($"[IsMoveValid] {pieceColor} {pieceType} {currentPosition} -> {targetPosition} = {result}");
        return result;
    }

    // Checks whether this piece's movement pattern can attack the given square
    // This intentionally does NOT simulate for self-check; it's movement-only (pseudo-legal).
    public bool CanAttackSquare(Vector2Int targetPosition)
    {
        if (pieceMover == null)
        {
            Debug.LogError("Piece mover not set for " + pieceType);
            return false;
        }
        return pieceMover.CanAttack(currentPosition, targetPosition, board);
    }

    public bool CanAttack(Vector2Int targetPosition, BoardManager board)
    {
        if (pieceMover == null)
        {
            Debug.LogError("Piece mover not set for " + pieceType);
            return false;
        }
        return pieceMover.CanAttack(currentPosition, targetPosition, board);
    }
}
