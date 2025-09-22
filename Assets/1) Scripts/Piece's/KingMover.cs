using UnityEngine;

public class KingMover : IPieceMover
{
    public bool IsValidMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        // Aynı kareye gitmek yasak
        if (from == to)
            return false;

        // King sadece 1 kare sağa-sola-yukarı-aşağı-çapraz gidebilir
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        if (dx > 1 || dy > 1)
            return false;

        // Başlangıçta taş yoksa veya yanlışlıkla null referans varsa
        Piece piece = board.GetPiece(from);
        if (piece == null)
            return false;

        // Hedef karede kendi taşı varsa gidemez
        Piece target = board.GetPiece(to);
        if (target != null && target.pieceColor == piece.pieceColor)
            return false;

        // (Castling burada eklenebilir)

        return true;
    }
}
