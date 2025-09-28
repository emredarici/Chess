using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, IPointerClickHandler
{
    public BoardManager board;
    public PieceType pieceType;
    public PieceColor pieceColor;
    public Vector2Int currentPosition;
    public bool hasMoved = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameManager.Instance.IsModeSet)
            return;
        Debug.Log($"{pieceType} tıklandı: {currentPosition}");

        var manager = PieceSelectionManager.Instance;
        if (manager != null && manager.selectedPiece != null && manager.selectedPiece != this)
        {
            if (manager.selectedPiece.pieceColor != TurnManager.currentTurn)
            {
                manager.selectedPiece = null;
                manager.ClearIndicators();
                return;
            }

            manager.TryMoveSelectedPiece(currentPosition);
            return;
        }

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
