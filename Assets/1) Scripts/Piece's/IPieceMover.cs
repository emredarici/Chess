using UnityEngine;

public interface IPieceMover
{
    bool IsValidMove(Vector2Int currentPosition, Vector2Int targetPosition, BoardManager board);
}

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }
public enum PieceColor { White, Black }