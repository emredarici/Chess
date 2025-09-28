using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public struct LastMoveInfo
{
    public Vector2Int from;
    public Vector2Int to;
    public Piece piece;
    public bool wasDoublePawnMove;
}

//White square color code #EBECD0
//Black square color code #739552

public class BoardManager : Singeleton<BoardManager>
{
    public static Action<LastMoveInfo> PieceMoved;
    public static Action<Piece> PromoteRequested;


    public Piece[,] pieces = new Piece[8, 8];


    public GameObject[] piecePrefabs;

    public LastMoveInfo lastMove;

    public static bool VerboseLogging = false;

    private CheckController whiteKingCheck;
    private CheckController blackKingCheck;

    private RepetitionTracker repetitionTracker = new RepetitionTracker();

    void Start()
    {
        SetupBoard();

        var whiteKing = GetKingOfColor(PieceColor.White);
        if (whiteKing != null)
            whiteKingCheck = whiteKing.GetComponent<CheckController>();
        var blackKing = GetKingOfColor(PieceColor.Black);
        if (blackKing != null)
            blackKingCheck = blackKing.GetComponent<CheckController>();
    }

    public List<Piece> GetAllPieces()
    {
        var PieceList = new List<Piece>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece p = pieces[x, y];
                if (p != null)
                    PieceList.Add(p);
            }
        }
        return PieceList;
    }

    void SetupBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            PlacePiece(PieceType.Pawn, PieceColor.White, new Vector2Int(i, 1));
            PlacePiece(PieceType.Pawn, PieceColor.Black, new Vector2Int(i, 6));
        }

        // White Rooks
        PlacePiece(PieceType.Rook, PieceColor.White, new Vector2Int(0, 0));
        PlacePiece(PieceType.Rook, PieceColor.White, new Vector2Int(7, 0));
        // Black Rooks
        PlacePiece(PieceType.Rook, PieceColor.Black, new Vector2Int(0, 7));
        PlacePiece(PieceType.Rook, PieceColor.Black, new Vector2Int(7, 7));

        // White Bishops
        PlacePiece(PieceType.Bishop, PieceColor.White, new Vector2Int(2, 0));
        PlacePiece(PieceType.Bishop, PieceColor.White, new Vector2Int(5, 0));
        // Black Bishops
        PlacePiece(PieceType.Bishop, PieceColor.Black, new Vector2Int(2, 7));
        PlacePiece(PieceType.Bishop, PieceColor.Black, new Vector2Int(5, 7));

        // White Knights
        PlacePiece(PieceType.Knight, PieceColor.White, new Vector2Int(1, 0));
        PlacePiece(PieceType.Knight, PieceColor.White, new Vector2Int(6, 0));
        // Black Knights
        PlacePiece(PieceType.Knight, PieceColor.Black, new Vector2Int(1, 7));
        PlacePiece(PieceType.Knight, PieceColor.Black, new Vector2Int(6, 7));

        // White Queens
        PlacePiece(PieceType.Queen, PieceColor.White, new Vector2Int(3, 0));
        // Black Queens
        PlacePiece(PieceType.Queen, PieceColor.Black, new Vector2Int(3, 7));

        // White Kings
        PlacePiece(PieceType.King, PieceColor.White, new Vector2Int(4, 0));
        // Black Kings
        PlacePiece(PieceType.King, PieceColor.Black, new Vector2Int(4, 7));
    }

    public void PlacePiece(PieceType type, PieceColor color, Vector2Int position)
    {
        GameObject piecePrefab = FindPiecePrefab(type, color);
        GameObject newPieceObject = Instantiate(piecePrefab, GetWorldPosition(position), Quaternion.identity);

        Piece newPiece = newPieceObject.GetComponent<Piece>();
        newPiece.pieceColor = color;
        newPiece.pieceType = type;
        newPiece.currentPosition = position;
        newPiece.board = this;


        switch (type)
        {
            case PieceType.Pawn:
                newPiece.SetMover(new PawnMover());
                break;
            case PieceType.Rook:
                newPiece.SetMover(new RookMover());
                break;
            case PieceType.Bishop:
                newPiece.SetMover(new BishopMover());
                break;
            case PieceType.Knight:
                newPiece.SetMover(new KnightMover());
                break;
            case PieceType.Queen:
                newPiece.SetMover(new QueenMover());
                break;
            case PieceType.King:
                newPiece.SetMover(new KingMover());
                break;
        }

        pieces[position.x, position.y] = newPiece;
    }

    public void CheckForCheck()
    {
        if (whiteKingCheck != null && whiteKingCheck.IsKingInCheck(this))
        {
            Debug.Log("Beyaz şah tehdit altında!");
            if (whiteKingCheck.IsCheckmate(this))
            {
                Debug.Log("Beyaz mat oldu!");
                GameManager.Instance.TriggerCheckmate(PieceColor.Black);
            }
        }

        if (blackKingCheck != null && blackKingCheck.IsKingInCheck(this))
        {
            Debug.Log("Siyah şah tehdit altında!");
            if (blackKingCheck.IsCheckmate(this))
            {
                Debug.Log("Siyah mat oldu!");
                GameManager.Instance.TriggerCheckmate(PieceColor.White);
            }
        }
    }

    public Piece GetPiece(Vector2Int position)
    {
        if (position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8)
        {
            return pieces[position.x, position.y];
        }
        return null;
    }

    public Vector3 GetWorldPosition(Vector2Int position)
    {
        return new Vector3(position.x, position.y, 0);
    }

    private GameObject FindPiecePrefab(PieceType type, PieceColor color)
    {
        if (piecePrefabs == null || piecePrefabs.Length == 0)
        {
            Debug.LogError("Piece Prefabs listesi BoardManager'da boş! Lütfen prefab'leri ekleyin.");
            return null;
        }

        string desiredPrefabName = type.ToString() + "_" + color.ToString();

        foreach (GameObject prefab in piecePrefabs)
        {
            if (prefab != null && prefab.name == desiredPrefabName)
            {
                return prefab;
            }
        }

        Debug.LogError("Prefab bulunamadı: " + desiredPrefabName + ". Lütfen BoardManager'daki Piece Prefabs listesini kontrol edin.");
        return null;
    }

    public void OnPieceDropped(Piece piece, Vector2Int newPosition)
    {
        Vector2Int fromPosition = piece.currentPosition;


        bool enPassantCapture = false;
        if (piece.pieceType == PieceType.Pawn && Mathf.Abs(newPosition.x - fromPosition.x) == 1 && newPosition.y != fromPosition.y)
        {
            Piece targetPiece = GetPiece(newPosition);
            if (targetPiece == null)
            {
                var last = lastMove;
                if (last.piece != null &&
                    last.piece.pieceType == PieceType.Pawn &&
                    last.wasDoublePawnMove &&
                    last.to.y == fromPosition.y &&
                    last.to.x == newPosition.x &&
                    last.piece.pieceColor != piece.pieceColor)
                {
                    Piece capturedPawn = GetPiece(new Vector2Int(newPosition.x, fromPosition.y));
                    if (capturedPawn != null && capturedPawn.pieceType == PieceType.Pawn && capturedPawn.pieceColor != piece.pieceColor)
                    {
                        Destroy(capturedPawn.gameObject);
                        pieces[newPosition.x, fromPosition.y] = null;
                        enPassantCapture = true;
                    }
                }
            }
        }

        if (!enPassantCapture)
        {
            Piece targetPiece = GetPiece(newPosition);
            if (targetPiece != null && targetPiece.pieceColor != piece.pieceColor)
            {
                Destroy(targetPiece.gameObject);
                pieces[newPosition.x, newPosition.y] = null;
            }
        }

        if (pieces[fromPosition.x, fromPosition.y] == piece)
        {
            pieces[fromPosition.x, fromPosition.y] = null;
        }

        pieces[newPosition.x, newPosition.y] = piece;

        piece.currentPosition = newPosition;
        piece.transform.position = GetWorldPosition(newPosition);

        bool wasDouble = (piece.pieceType == PieceType.Pawn) && Mathf.Abs(newPosition.y - fromPosition.y) == 2;
        lastMove = new LastMoveInfo
        {
            from = fromPosition,
            to = newPosition,
            piece = piece,
            wasDoublePawnMove = wasDouble
        };

        piece.hasMoved = true;

        if (piece.pieceType == PieceType.King && Mathf.Abs(newPosition.x - fromPosition.x) == 2)
        {
            int direction = newPosition.x - fromPosition.x; // +2 = kısa rok, -2 = uzun rok
            int rookFromX = direction > 0 ? 7 : 0;
            int rookToX = fromPosition.x + (direction > 0 ? 1 : -1);
            Vector2Int rookFrom = new Vector2Int(rookFromX, fromPosition.y);
            Vector2Int rookTo = new Vector2Int(rookToX, fromPosition.y);
            Piece rook = GetPiece(rookFrom);
            if (rook != null && rook.pieceType == PieceType.Rook && !rook.hasMoved)
            {
                pieces[rookFrom.x, rookFrom.y] = null;
                pieces[rookTo.x, rookTo.y] = rook;
                rook.currentPosition = rookTo;
                rook.transform.position = GetWorldPosition(rookTo);
                rook.hasMoved = true;

                Debug.Log(rook.pieceColor + " Rook moved for castling: " + rookFrom + " -> " + rookTo);
            }
        }

        if (piece.pieceType == PieceType.Pawn && (newPosition.y == 0 || newPosition.y == 7))
        {
            PromoteRequested?.Invoke(piece);
            return;
        }

        Debug.Log(piece.pieceColor + " " + piece.pieceType + " taşını " + fromPosition + " -> " + newPosition + " konumuna hareket ettirdi.");

        PieceMoved?.Invoke(lastMove);
        CheckForCheck();

        if (IsOnlyKingsRemaining())
        {
            Debug.Log("Draw: Only kings remaining (insufficient mating material).");
            GameManager.Instance.TriggerOnly2Kings();
        }

        string positionKey = GetPositionKey();
        repetitionTracker.RecordPosition(positionKey);
    }


    public bool SimulateMoveAndCheckSelfCheck(Piece piece, Vector2Int targetPosition)
    {
        Vector2Int originalPosition = piece.currentPosition;
        Piece capturedPiece = GetPiece(targetPosition);

        var lastMoveBackup = lastMove;

        if (VerboseLogging)
            Debug.Log($"[SimulateMoveAndCheckSelfCheck] Simülasyon başlıyor: {piece.pieceColor} {piece.pieceType} {originalPosition} -> {targetPosition}");

        pieces[originalPosition.x, originalPosition.y] = null;
        pieces[targetPosition.x, targetPosition.y] = piece;
        piece.currentPosition = targetPosition;


        Piece king = GetKingOfColor(piece.pieceColor);
        bool isInCheck = false;
        if (king != null)
        {
            var checkController = king.GetComponent<CheckController>();
            if (checkController != null)
                isInCheck = checkController.IsKingInCheck(this);
        }

        piece.currentPosition = originalPosition;
        pieces[originalPosition.x, originalPosition.y] = piece;
        pieces[targetPosition.x, targetPosition.y] = capturedPiece;

        lastMove = lastMoveBackup;

        if (VerboseLogging)
            Debug.Log($"[SimulateMoveAndCheckSelfCheck] Sonuç: {piece.pieceColor} {piece.pieceType} {originalPosition} -> {targetPosition} | isInCheck={isInCheck}");

        return isInCheck;
    }

    public Piece GetKingOfColor(PieceColor color)
    {
        foreach (var piece in pieces)
        {
            if (piece != null && piece.pieceType == PieceType.King && piece.pieceColor == color)
            {
                return piece;
            }
        }
        return null;
    }

    public bool IsOnlyKingsRemaining()
    {
        var all = GetAllPieces();
        if (all.Count != 2)
            return false;

        return all[0].pieceType == PieceType.King && all[1].pieceType == PieceType.King;
    }

    private string GetPositionKey()
    {
        StringBuilder sb = new StringBuilder();
        // Pieces
        for (int y = 7; y >= 0; y--)
        {
            int emptyCount = 0;
            for (int x = 0; x < 8; x++)
            {
                var p = pieces[x, y];
                if (p == null)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        sb.Append(emptyCount);
                        emptyCount = 0;
                    }
                    char c = '?';
                    switch (p.pieceType)
                    {
                        case PieceType.King: c = 'k'; break;
                        case PieceType.Queen: c = 'q'; break;
                        case PieceType.Rook: c = 'r'; break;
                        case PieceType.Bishop: c = 'b'; break;
                        case PieceType.Knight: c = 'n'; break;
                        case PieceType.Pawn: c = 'p'; break;
                    }
                    if (p.pieceColor == PieceColor.White)
                        c = char.ToUpper(c);
                    sb.Append(c);
                }
            }
            if (emptyCount > 0)
                sb.Append(emptyCount);
            if (y > 0) sb.Append('/');
        }
        // Turn
        sb.Append(' ');
        sb.Append(TurnManager.currentTurn == PieceColor.White ? 'w' : 'b');

        sb.Append(' ');
        string castling = "";
        // White
        Piece wKing = GetKingOfColor(PieceColor.White);
        if (wKing != null && !wKing.hasMoved)
        {
            var a1 = GetPiece(new Vector2Int(0, 0));
            if (a1 != null && a1.pieceType == PieceType.Rook && !a1.hasMoved && a1.pieceColor == PieceColor.White)
                castling += 'Q';
            var h1 = GetPiece(new Vector2Int(7, 0));
            if (h1 != null && h1.pieceType == PieceType.Rook && !h1.hasMoved && h1.pieceColor == PieceColor.White)
                castling += 'K';
        }
        Piece bKing = GetKingOfColor(PieceColor.Black);
        if (bKing != null && !bKing.hasMoved)
        {
            var a8 = GetPiece(new Vector2Int(0, 7));
            if (a8 != null && a8.pieceType == PieceType.Rook && !a8.hasMoved && a8.pieceColor == PieceColor.Black)
                castling += 'q';
            var h8 = GetPiece(new Vector2Int(7, 7));
            if (h8 != null && h8.pieceType == PieceType.Rook && !h8.hasMoved && h8.pieceColor == PieceColor.Black)
                castling += 'k';
        }
        if (castling == "") castling = "-";
        sb.Append(castling);

        // En passant
        sb.Append(' ');
        string enp = "-";
        if (lastMove.piece != null && lastMove.piece.pieceType == PieceType.Pawn && lastMove.wasDoublePawnMove)
        {
            int ex = lastMove.to.x;
            int ey = (lastMove.from.y + lastMove.to.y) / 2;
            enp = ((char)('a' + ex)).ToString() + (ey + 1).ToString();
        }
        sb.Append(enp);

        return sb.ToString();
    }

    public bool IsSquareUnderAttack(Vector2Int square, PieceColor byColor)
    {
        foreach (var piece in GetAllPieces())
        {
            if (piece.pieceColor == byColor && piece.CanAttack(square, this))
            {
                return true;
            }
        }
        return false;
    }

}