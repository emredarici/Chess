using UnityEngine;

public class RookMover : IPieceMover
{
    public bool IsValidMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        if (BoardManager.VerboseLogging)
            Debug.Log($"[Rook.IsValidMove] Checking {from} -> {to}");
        if (from.x != to.x && from.y != to.y)
            return false;

        if (from == to)
            return false;

        Piece piece = board.GetPiece(from);
        if (piece == null)
            return false;

        int dx = to.x - from.x;
        int dy = to.y - from.y;

        int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
        int stepY = dy == 0 ? 0 : (dy > 0 ? 1 : -1);

        int x = from.x + stepX;
        int y = from.y + stepY;
        while (x != to.x || y != to.y)
        {
            if (board.GetPiece(new Vector2Int(x, y)) != null)
                return false;
            x += stepX;
            y += stepY;
        }

        Piece target = board.GetPiece(to);
        if (target != null && target.pieceColor == piece.pieceColor)
            return false;

        bool selfCheck = board.SimulateMoveAndCheckSelfCheck(piece, to);
        if (BoardManager.VerboseLogging)
            Debug.Log($"[Rook.IsValidMove] Simulate result for {from}->{to} selfCheck={selfCheck}");
        if (selfCheck)
            return false;

        return true;
    }

    public bool CanAttack(Vector2Int from, Vector2Int to, BoardManager board)
    {
        if (from == to)
            return false;
        if (from.x != to.x && from.y != to.y)
            return false;

        int dx = to.x - from.x;
        int dy = to.y - from.y;

        int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
        int stepY = dy == 0 ? 0 : (dy > 0 ? 1 : -1);

        int x = from.x + stepX;
        int y = from.y + stepY;
        while (x != to.x || y != to.y)
        {
            if (board.GetPiece(new Vector2Int(x, y)) != null)
                return false;
            x += stepX;
            y += stepY;
        }
        return true;
    }
}
