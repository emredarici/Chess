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

// Bu sınıf, oyun tahtasının yönetiminden sorumludur.
// Taşların hareket kurallarını bilmesine gerek yoktur, sadece tahtadaki
// taşların konumunu ve durumunu yönetir.
public class BoardManager : Singeleton<BoardManager>
{
    // Event fired after a piece move is applied. Listeners can show indicators, sounds, etc.
    public static Action<LastMoveInfo> PieceMoved;
    public static Action<Piece> PromoteRequested;

    // Tahtanın 8x8'lik yapısını temsil eden 2D dizi.
    // Her bir hücre, o karede bulunan Piece nesnesini (taşı) tutar.
    public Piece[,] pieces = new Piece[8, 8];

    // Bu, tahta üzerinde kullanılacak tüm taş prefab'lerinin listesidir.
    // Unity Inspector penceresinden bu listeyi doldurmalısın.
    public GameObject[] piecePrefabs;

    public LastMoveInfo lastMove;

    // Toggle verbose debug logs for diagnosis. Set to true to enable detailed simulation/mover logs.
    public static bool VerboseLogging = false;

    // Performans için şahların CheckController referansları
    private CheckController whiteKingCheck;
    private CheckController blackKingCheck;
    // SOLID: Sadece pozisyon stringi üretir, tekrarları ayrı class takip eder
    private RepetitionTracker repetitionTracker = new RepetitionTracker();

    void Start()
    {
        // Oyun başladığında tahtayı ilk konfigürasyonuna ayarla.
        SetupBoard();

        // Şahların CheckController referanslarını al
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

    // Bu metot, bir taşı verilen konuma yerleştirmekten sorumludur.
    // Taşı oluşturur, verilerini atar ve tahtanın 2D dizisine ekler.
    public void PlacePiece(PieceType type, PieceColor color, Vector2Int position)
    {
        // piecePrefabs dizisinden doğru prefab'i bulur.
        GameObject piecePrefab = FindPiecePrefab(type, color);
        // Prefab'den bir kopya oluşturur ve onu doğru dünya koordinatına yerleştirir.
        GameObject newPieceObject = Instantiate(piecePrefab, GetWorldPosition(position), Quaternion.identity);

        // Yeni oluşturulan nesnedeki Piece bileşenini alır.
        Piece newPiece = newPieceObject.GetComponent<Piece>();
        // Piece bileşeninin verilerini ayarlar.
        newPiece.pieceColor = color;
        newPiece.pieceType = type;
        newPiece.currentPosition = position;
        // Piece nesnesine BoardManager referansını atar, böylece Piece tahta hakkında bilgi alabilir.
        newPiece.board = this;

        // SOLID: Burada, taşa kendi hareket mantığını atıyoruz.
        // Yeni taş türü eklemek için sadece buraya yeni bir else if ekle.

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
        // Diğer taş türleri için de buraya else if'ler eklenecek.

        // Taşı, tahtanın 2D dizisinde doğru konuma yerleştirir.
        pieces[position.x, position.y] = newPiece;
    }

    // Bir taş hamle ettikten sonra ilgili şah için şah kontrolü yapar.
    public void CheckForCheck()
    {
        // Beyaz şah kontrolü
        if (whiteKingCheck != null && whiteKingCheck.IsKingInCheck(this))
        {
            Debug.Log("Beyaz şah tehdit altında!");
            if (whiteKingCheck.IsCheckmate(this))
            {
                Debug.Log("Beyaz mat oldu!");
                // Oyun sonu işlemleri burada yapılabilir
            }
        }

        // Siyah şah kontrolü
        if (blackKingCheck != null && blackKingCheck.IsKingInCheck(this))
        {
            Debug.Log("Siyah şah tehdit altında!");
            if (blackKingCheck.IsCheckmate(this))
            {
                Debug.Log("Siyah mat oldu!");
                // Oyun sonu işlemleri burada yapılabilir
            }
        }
    }


    // Verilen konumdaki taşı döndürür. Eğer kare boşsa null döndürür.
    public Piece GetPiece(Vector2Int position)
    {
        if (position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8)
        {
            return pieces[position.x, position.y];
        }
        return null;
    }

    // Tahta koordinatlarını (örn: 0,0), Unity'deki sahne koordinatlarına dönüştürür.
    public Vector3 GetWorldPosition(Vector2Int position)
    {
        // Bu kısım, tahta objesinin ve kameranın konumuna göre ayarlanmalıdır.
        // Örnek: Tahtanın sol alt köşesi (0,0) olsun ve kareler 1 birim büyüklüğünde olsun.
        return new Vector3(position.x, position.y, 0);
    }

    // piecePrefabs dizisinde ilgili taşın prefab'ini bulur.
    private GameObject FindPiecePrefab(PieceType type, PieceColor color)
    {
        // Öncelikle piecePrefabs listesinin boş olup olmadığını kontrol edelim.
        if (piecePrefabs == null || piecePrefabs.Length == 0)
        {
            Debug.LogError("Piece Prefabs listesi BoardManager'da boş! Lütfen prefab'leri ekleyin.");
            return null;
        }

        // Beklenen prefab ismini oluşturalım. Örneğin: "Pawn_White", "Pawn_Black".
        string desiredPrefabName = type.ToString() + "_" + color.ToString();

        // piecePrefabs listesindeki tüm prefab'leri kontrol edelim.
        foreach (GameObject prefab in piecePrefabs)
        {
            // Eğer bir prefab'in adı, aradığımız isimle eşleşiyorsa, o prefab'i döndür.
            if (prefab != null && prefab.name == desiredPrefabName)
            {
                return prefab;
            }
        }

        // Eğer istenen prefab listede bulunamazsa hata mesajı logla.
        Debug.LogError("Prefab bulunamadı: " + desiredPrefabName + ". Lütfen BoardManager'daki Piece Prefabs listesini kontrol edin.");
        return null;
    }

    // YENİ EKLENEN METOT: Bir taşın sahnedeki yeni konumuna yerleştirilmesini yönetir.
    public void OnPieceDropped(Piece piece, Vector2Int newPosition)
    {
        // Capture the original position before any mutation
        Vector2Int fromPosition = piece.currentPosition;


        // EN PASSANT: Eğer piyon çapraz boş kareye gidiyorsa ve lastMove'da iki kare ilerlemiş bir piyon hemen yanında ise, o piyonu sil.
        bool enPassantCapture = false;
        if (piece.pieceType == PieceType.Pawn && Mathf.Abs(newPosition.x - fromPosition.x) == 1 && newPosition.y != fromPosition.y)
        {
            // Hedef karede taş yoksa, ama lastMove'da iki kare ilerlemiş bir piyon hemen yanımızda ise
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
                    // Rakip piyonun bulunduğu kareyi sil
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

        // Normal taş alma (veya en passant değilse)
        if (!enPassantCapture)
        {
            Piece targetPiece = GetPiece(newPosition);
            if (targetPiece != null && targetPiece.pieceColor != piece.pieceColor)
            {
                Destroy(targetPiece.gameObject); // Unity sahnesinden obje olarak kaldır.
                pieces[newPosition.x, newPosition.y] = null; // clear board reference
            }
        }

        // Remove piece from its current square on the board array (safety check)
        if (pieces[fromPosition.x, fromPosition.y] == piece)
        {
            pieces[fromPosition.x, fromPosition.y] = null;
        }

        // Place the piece into the new square (board array)
        pieces[newPosition.x, newPosition.y] = piece;

        // Update piece's internal position and visual transform
        piece.currentPosition = newPosition;
        piece.transform.position = GetWorldPosition(newPosition);

        // Compute whether this move was a double pawn move (based on fromPosition)
        bool wasDouble = (piece.pieceType == PieceType.Pawn) && Mathf.Abs(newPosition.y - fromPosition.y) == 2;
        lastMove = new LastMoveInfo
        {
            from = fromPosition,
            to = newPosition,
            piece = piece,
            wasDoublePawnMove = wasDouble
        };

        // Mark that this piece has moved (useful for pawn double-step and castling logic)
        piece.hasMoved = true;

        if (piece.pieceType == PieceType.Pawn && (newPosition.y == 0 || newPosition.y == 7))
        {
            PromoteRequested?.Invoke(piece);
            return;
        }

        Debug.Log(piece.pieceColor + " " + piece.pieceType + " taşını " + fromPosition + " -> " + newPosition + " konumuna hareket ettirdi.");

        PieceMoved?.Invoke(lastMove);
        // Her hamle sonrası şah kontrolü
        CheckForCheck();

        // Eğer sadece iki şah kaldıysa (yetersiz materyal) oyunu berabere ilan et
        if (IsOnlyKingsRemaining())
        {
            Debug.Log("Draw: Only kings remaining (insufficient mating material).");
            // Burada oyun bitirme / UI gösterme çağrısı yapılabilir.
        }

        string positionKey = GetPositionKey();
        repetitionTracker.RecordPosition(positionKey);
    }


    public bool SimulateMoveAndCheckSelfCheck(Piece piece, Vector2Int targetPosition)
    {
        Vector2Int originalPosition = piece.currentPosition;
        Piece capturedPiece = GetPiece(targetPosition);

        // lastMove'u geçici olarak sakla
        var lastMoveBackup = lastMove;

        if (VerboseLogging)
            Debug.Log($"[SimulateMoveAndCheckSelfCheck] Simülasyon başlıyor: {piece.pieceColor} {piece.pieceType} {originalPosition} -> {targetPosition}");

        pieces[originalPosition.x, originalPosition.y] = null;
        pieces[targetPosition.x, targetPosition.y] = piece;
        piece.currentPosition = targetPosition;

        // Simülasyon sırasında lastMove'u güncelleme! (en passant gibi kontrollerde yanlışlık olmasın)

        Piece king = GetKingOfColor(piece.pieceColor);
        bool isInCheck = false;
        if (king != null)
        {
            var checkController = king.GetComponent<CheckController>();
            if (checkController != null)
                isInCheck = checkController.IsKingInCheck(this);
        }

        // Taşları ve pozisyonu geri al
        piece.currentPosition = originalPosition;
        pieces[originalPosition.x, originalPosition.y] = piece;
        pieces[targetPosition.x, targetPosition.y] = capturedPiece;

        // lastMove'u geri yükle
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

    // Returns true when the only pieces left on the board are the two kings.
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

        // Rooking rights
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
        // Black
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

}