using UnityEngine;

public class PawnMover : IPieceMover
{
    public bool IsValidMove(Vector2Int currentPosition, Vector2Int targetPosition, BoardManager board)
    {
        if (targetPosition.x < 0 || targetPosition.x > 8 || targetPosition.y < 0 || targetPosition.y > 8)
            return false; // 


        return true;
    }
}
