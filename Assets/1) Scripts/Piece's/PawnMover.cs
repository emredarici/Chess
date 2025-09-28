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

        if (targetPosition.x == currentPosition.x)
        {
            if (targetPosition.y == currentPosition.y + direction && board.GetPiece(targetPosition) == null)
            {
                if (board.SimulateMoveAndCheckSelfCheck(piece, targetPosition))
                    return false;
                return true;
            }

            if (!piece.hasMoved && targetPosition.y == currentPosition.y + 2 * direction && board.GetPiece(new Vector2Int(currentPosition.x, currentPosition.y + direction)) == null && board.GetPiece(targetPosition) == null)
            {
                if (board.SimulateMoveAndCheckSelfCheck(piece, targetPosition))
                    return false;
                return true;
            }
        }

        if (Mathf.Abs(targetPosition.x - currentPosition.x) == 1 && targetPosition.y == currentPosition.y + direction)
        {
            Piece targetPiece = board.GetPiece(targetPosition);
            if (targetPiece != null && targetPiece.pieceColor != piece.pieceColor)
            {
                if (board.SimulateMoveAndCheckSelfCheck(piece, targetPosition))
                    return false;
                Debug.Log("Taş siliniyor:" + targetPiece);
                return true;
            }

            if (targetPiece == null)
            {
                var lastMove = board.lastMove;
                if (lastMove.piece != null &&
                    lastMove.piece.pieceType == PieceType.Pawn &&
                    lastMove.wasDoublePawnMove &&
                    lastMove.to.y == currentPosition.y &&
                    lastMove.to.x == targetPosition.x &&
                    lastMove.piece.pieceColor != piece.pieceColor)
                {
                    if (board.SimulateMoveAndCheckSelfCheck(piece, targetPosition))
                        return false;
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanAttack(Vector2Int currentPosition, Vector2Int targetPosition, BoardManager board)
    {
        Piece piece = board.GetPiece(currentPosition);
        if (piece == null)
            return false;

        int direction = (piece.pieceColor == PieceColor.White) ? 1 : -1;
        if (Mathf.Abs(targetPosition.x - currentPosition.x) == 1 && targetPosition.y == currentPosition.y + direction)
            return true;

        var last = board.lastMove;
        if (last.piece != null && last.piece.pieceType == PieceType.Pawn && last.wasDoublePawnMove && last.piece.pieceColor != piece.pieceColor)
        {
            // The pawn that moved two squares is now at last.to; en passant capture lands on the square behind it (currentPosition.y + direction)
            if (last.to.y == currentPosition.y && last.to.x == targetPosition.x && targetPosition.y == currentPosition.y + direction)
            {
                return true;
            }
        }
        return false;
    }
}
