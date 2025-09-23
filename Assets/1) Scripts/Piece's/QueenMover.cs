using UnityEngine;

public class QueenMover : IPieceMover
{
    public bool IsValidMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        if (BoardManager.VerboseLogging)
            Debug.Log($"[Queen.IsValidMove] Checking {from} -> {to}");
        if (from == to)
            return false;

        Piece piece = board.GetPiece(from);
        if (piece == null)
            return false;

        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        // Queen hem yatay/dikey hem de çapraz gidebilir
        if (!(dx == dy || from.x == to.x || from.y == to.y))
            return false;

        int stepX = (to.x - from.x) == 0 ? 0 : (to.x - from.x) > 0 ? 1 : -1;
        int stepY = (to.y - from.y) == 0 ? 0 : (to.y - from.y) > 0 ? 1 : -1;
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

        // Açmaz kontrolü: hamle kuralları ve hedefte kendi taşı kontrolünden sonra, EN SON
        bool selfCheck = board.SimulateMoveAndCheckSelfCheck(piece, to);
        if (BoardManager.VerboseLogging)
            Debug.Log($"[Queen.IsValidMove] Simulate result for {from}->{to} selfCheck={selfCheck}");
        if (selfCheck)
            return false;

        return true;
    }

    public bool CanAttack(Vector2Int from, Vector2Int to, BoardManager board)
    {
        if (from == to)
            return false;

        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);
        if (!(dx == dy || from.x == to.x || from.y == to.y))
            return false;

        int stepX = (to.x - from.x) == 0 ? 0 : (to.x - from.x) > 0 ? 1 : -1;
        int stepY = (to.y - from.y) == 0 ? 0 : (to.y - from.y) > 0 ? 1 : -1;
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
