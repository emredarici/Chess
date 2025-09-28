using UnityEngine;

public class KingMover : IPieceMover
{
    public bool IsValidMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        if (board == null)
        {
            board = BoardManager.Instance;
        }

        if (IsCastlingMove(from, to, board))
            return true;

        if (from == to)
            return false;

        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        if (dx > 1 || dy > 1)
            return false;

        Piece piece = board.GetPiece(from);
        if (piece == null)
            return false;

        Piece target = board.GetPiece(to);
        if (target != null && target.pieceColor == piece.pieceColor)
            return false;

        Piece enemyKing = board.GetKingOfColor(piece.pieceColor == PieceColor.White ? PieceColor.Black : PieceColor.White);
        if (enemyKing != null)
        {
            int enemyKingX = Mathf.Abs(to.x - enemyKing.currentPosition.x);
            int enemyKingY = Mathf.Abs(to.y - enemyKing.currentPosition.y);
            if (enemyKingX <= 1 && enemyKingY <= 1)
            {
                return false;
            }
        }

        if (board.SimulateMoveAndCheckSelfCheck(piece, to))
            return false;

        return true;
    }

    public bool CanAttack(Vector2Int from, Vector2Int to, BoardManager board)
    {
        if (from == to)
            return false;
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);
        if (dx > 1 || dy > 1)
            return false;
        return true;
    }

    public bool IsCastlingMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        Piece piece = board.GetPiece(from);
        if (piece == null || piece.pieceType != PieceType.King)
            return false;

        if (piece.hasMoved)
            return false;

        int direction = to.x - from.x;
        if (Mathf.Abs(direction) != 2 || from.y != to.y)
            return false;

        int rookX = direction > 0 ? 7 : 0;
        Vector2Int rookPosition = new Vector2Int(rookX, from.y);
        Piece rook = board.GetPiece(rookPosition);
        if (rook == null || rook.pieceType != PieceType.Rook || rook.hasMoved)
            return false;

        int step = direction > 0 ? 1 : -1;
        for (int x = from.x + step; x != rookX; x += step)
        {
            if (board.GetPiece(new Vector2Int(x, from.y)) != null)
                return false;
        }

        PieceColor opponent = (piece.pieceColor == PieceColor.White) ? PieceColor.Black : PieceColor.White;
        for (int x = from.x; x != to.x + step; x += step)
        {
            if (board.IsSquareUnderAttack(new Vector2Int(x, from.y), opponent))
                return false;
        }

        return true;
    }
}
