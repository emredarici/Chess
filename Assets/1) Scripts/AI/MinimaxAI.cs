using UnityEngine;

public class MinimaxAI
{
    public struct AIMove
    {
        public Vector2Int from;
        public Vector2Int to;
    }

    private PieceColor aiColor;
    private int maxDepth;

    public MinimaxAI(PieceColor aiColor, int maxDepth = 3)
    {
        this.aiColor = aiColor;
        this.maxDepth = maxDepth;
    }

    public AIMove GetBestMove(BoardManager board)
    {
        int best = int.MinValue;
        AIMove bestMove = default;
        var allPieces = board.GetAllPieces();
        foreach (var p in allPieces)
        {
            if (p.pieceColor != aiColor) continue;
            Vector2Int from = p.currentPosition;
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int to = new Vector2Int(x, y);
                    if (!p.IsMoveValid(to)) continue;

                    var captured = board.pieces[to.x, to.y];
                    board.pieces[from.x, from.y] = null;
                    board.pieces[to.x, to.y] = p;
                    var oldPos = p.currentPosition;
                    p.currentPosition = to;

                    int score = -Negamax(board, maxDepth - 1, int.MinValue / 2, int.MaxValue / 2, Opponent(aiColor));

                    p.currentPosition = oldPos;
                    board.pieces[from.x, from.y] = p;
                    board.pieces[to.x, to.y] = captured;

                    if (score > best)
                    {
                        best = score;
                        bestMove = new AIMove { from = from, to = to };
                    }
                }
        }
        return bestMove;
    }

    private int Negamax(BoardManager board, int depth, int alpha, int beta, PieceColor side)
    {
        if (depth == 0)
            return Evaluate(board, aiColor);

        int max = int.MinValue / 2;
        var all = board.GetAllPieces();
        foreach (var p in all)
        {
            if (p.pieceColor != side) continue;
            Vector2Int from = p.currentPosition;
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int to = new Vector2Int(x, y);
                    if (!p.IsMoveValid(to)) continue;

                    var captured = board.pieces[to.x, to.y];
                    board.pieces[from.x, from.y] = null;
                    board.pieces[to.x, to.y] = p;
                    var oldPos = p.currentPosition;
                    p.currentPosition = to;

                    int val = -Negamax(board, depth - 1, -beta, -alpha, Opponent(side));

                    p.currentPosition = oldPos;
                    board.pieces[from.x, from.y] = p;
                    board.pieces[to.x, to.y] = captured;

                    if (val > max) max = val;
                    if (val > alpha) alpha = val;
                    if (alpha >= beta) return alpha;
                }
        }
        return max == int.MinValue / 2 ? Evaluate(board, aiColor) : max;
    }

    private int Evaluate(BoardManager board, PieceColor forColor)
    {
        int score = 0;
        foreach (var p in board.GetAllPieces())
        {
            int val = PieceValue(p.pieceType);
            score += (p.pieceColor == forColor) ? val : -val;
        }
        return score;
    }

    private int PieceValue(PieceType t)
    {
        switch (t)
        {
            case PieceType.King: return 900;
            case PieceType.Queen: return 90;
            case PieceType.Rook: return 50;
            case PieceType.Bishop: return 30;
            case PieceType.Knight: return 30;
            case PieceType.Pawn: return 10;
            default: return 0;
        }
    }

    private PieceColor Opponent(PieceColor c) => (c == PieceColor.White) ? PieceColor.Black : PieceColor.White;

}
