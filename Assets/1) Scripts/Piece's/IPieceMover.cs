using UnityEngine;

public interface IPieceMover
{
    bool IsValidMove(Vector2Int currentPosition, Vector2Int targetPosition, BoardManager board);
    // Movement-only attack check (pseudo-legal): does this piece's movement pattern attack targetPosition
    // without considering whether the move would leave its own king in check.
    bool CanAttack(Vector2Int currentPosition, Vector2Int targetPosition, BoardManager board);
}

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }
public enum PieceColor { White, Black }