
using UnityEngine;

public class CheckController : MonoBehaviour
{
    private Piece thisKing;

    void Start()
    {
        thisKing = this.GetComponent<Piece>();
    }

    public bool IsKingInCheck(BoardManager board)
    {
        if (thisKing == null)
        {
            Debug.LogError("Şah bulunamadı!");
            return false;
        }

        Vector2Int kingPosition = thisKing.currentPosition;
        foreach (var piece in board.GetAllPieces())
        {
            if (piece.pieceColor != thisKing.pieceColor)
            {
                // Use movement-only attack check to avoid recursive self-check simulation
                if (piece.CanAttackSquare(kingPosition))
                {
                    Debug.Log($"{thisKing.pieceColor} şahı tehdit altında!");
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsCheckmate(BoardManager board)
    {
        if (!IsKingInCheck(board))
            return false;

        var myPieces = board.GetAllPieces().FindAll(p => p.pieceColor == thisKing.pieceColor);
        foreach (var piece in myPieces)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int target = new Vector2Int(x, y);
                    if (piece.IsMoveValid(target))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
