using UnityEngine;

public class QueenMover : IPieceMover
{
    public bool IsValidMove(Vector2Int from, Vector2Int to, BoardManager board)
    {
        return new RookMover().IsValidMove(from, to, board) || new BishopMover().IsValidMove(from, to, board);
    }
}
