using UnityEngine;

public class KnightMover : IPieceMover
{
    public bool IsValidMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        if (!((dx == 2 && dy == 1) || (dx == 1 && dy == 2)))
            return false;

        if (from == to)
            return false;

        Piece piece = board.GetPiece(from);
        if (piece == null)
            return false;


        Piece target = board.GetPiece(to);
        if (target != null && target.pieceColor == piece.pieceColor)
            return false;

        return true;
    }
}
