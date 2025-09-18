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

    public void OnPointerClick(PointerEventData eventData)
    {
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
        return pieceMover.IsValidMove(currentPosition, targetPosition, board);
    }
}
