using UnityEngine;

public class BishopMover : IPieceMover
{
    public bool IsValidMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        if (Mathf.Abs(to.x - from.x) != Mathf.Abs(to.y - from.y))
            return false; // Sadece çapraz hareket

        if (from == to)
            return false;

        Piece piece = board.GetPiece(from);
        if (piece == null)
            return false;

        // Diagonal hareket için, aradaki karelerin boş olup olmadığını kontrol et.
        int dx = to.x > from.x ? 1 : -1;
        int dy = to.y > from.y ? 1 : -1;
        Vector2Int direction = new Vector2Int(dx, dy);
        Vector2Int current = from + direction;

        while (current != to)
        {
            if (board.GetPiece(current) != null)
                return false;
            current += direction;
        }

        Piece target = board.GetPiece(to);
        if (target != null && target.pieceColor == piece.pieceColor)
            return false;

        return true;
    }
}
