using UnityEngine;

public class RookMover : IPieceMover
{
    public bool IsValidMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        // Sadece yatay veya dikey hareket
        if (from.x != to.x && from.y != to.y)
            return false;

        // Hedef pozisyon aynıysa geçersiz
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
        // Hedef kareye kadar aradaki tüm karelerde taş var mı kontrol et
        while (x != to.x || y != to.y)
        {
            if (board.GetPiece(new Vector2Int(x, y)) != null)
                return false;
            x += stepX;
            y += stepY;
        }

        // Hedef karede kendi taşı varsa gidemez
        Piece target = board.GetPiece(to);
        if (target != null && target.pieceColor == piece.pieceColor)
            return false;

        // Tüm kontroller geçtiyse hamle geçerli
        return true;
    }
}
