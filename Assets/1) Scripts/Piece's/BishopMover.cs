using UnityEngine;

public class BishopMover : IPieceMover
{
    public bool IsValidMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        if (BoardManager.VerboseLogging)
            Debug.Log($"[Bishop.IsValidMove] Checking {from} -> {to}");
        if (Mathf.Abs(to.x - from.x) != Mathf.Abs(to.y - from.y))
            return false;

        if (from == to)
            return false;

        Piece piece = board.GetPiece(from);
        if (piece == null)
            return false;

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

        bool selfCheck = board.SimulateMoveAndCheckSelfCheck(piece, to);
        if (BoardManager.VerboseLogging)
            Debug.Log($"[Bishop.IsValidMove] Simulate result for {from}->{to} selfCheck={selfCheck}");
        if (selfCheck)
            return false;

        return true;
    }

    public bool CanAttack(Vector2Int from, Vector2Int to, BoardManager board)
    {
        if (from == to)
            return false;
        if (Mathf.Abs(to.x - from.x) != Mathf.Abs(to.y - from.y))
            return false;

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
        return true;
    }
}
