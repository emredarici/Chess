using UnityEngine;

public class PawnMover : IPieceMover
{
    public bool IsValidMove(Vector2Int currentPosition, Vector2Int targetPosition, BoardManager board)
    {
        if (targetPosition.x < 0 || targetPosition.x > 7 || targetPosition.y < 0 || targetPosition.y > 7)
            return false;

        Piece piece = board.GetPiece(currentPosition);
        if (piece == null)
            return false;

        int direction = (piece.pieceColor == PieceColor.White) ? 1 : -1;

        //Only forward move
        if (targetPosition.x == currentPosition.x)
        {
            // Move one square forward
            if (targetPosition.y == currentPosition.y + direction && board.GetPiece(targetPosition) == null)
                return true;

            // Move two squares forward if the pawn has not moved yet
            if (!piece.hasMoved && targetPosition.y == currentPosition.y + 2 * direction && board.GetPiece(new Vector2Int(currentPosition.x, currentPosition.y + direction)) == null && board.GetPiece(targetPosition) == null)
                return true;
        }

        // A pawn captures diagonally
        if (Mathf.Abs(targetPosition.x - currentPosition.x) == 1 && targetPosition.y == currentPosition.y + direction)
        {
            Piece targetPiece = board.GetPiece(targetPosition);
            if (targetPiece != null && targetPiece.pieceColor != piece.pieceColor)
            {
                Debug.Log("Taş siliniyor:" + targetPiece);
                return true;
            }
        }

        // Other rules can be added here (en passant, promotion, etc.)

        return false;
    }
}
