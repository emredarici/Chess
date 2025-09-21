using UnityEngine;
using System;

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

    void Start()
    {
        // Oyun başladığında tahtayı ilk konfigürasyonuna ayarla.
        SetupBoard();
    }

    void SetupBoard()
    {
        // Başlangıç pozisyonundaki tüm piyonları tahtaya yerleştirir.
        // PlacePiece metodu, karmaşık instantiating (oluşturma) işlemini
        // bu metottan ayırır.
        for (int i = 0; i < 8; i++)
        {
            PlacePiece(PieceType.Pawn, PieceColor.White, new Vector2Int(i, 1));
            PlacePiece(PieceType.Pawn, PieceColor.Black, new Vector2Int(i, 6));
        }

        // Kale, At, Fil, Vezir ve Şahlar için de buraya benzer kodlar eklenecek.
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
        // Bu, yeni bir taş türü eklemek istediğinde sadece buraya yeni bir satır ekleyeceğin anlamına gelir.
        if (type == PieceType.Pawn)
        {
            newPiece.SetMover(new PawnMover()); // Piyon için PawnMover nesnesi oluşturulup atanır.
        }
        // Diğer taş türleri için de buraya else if'ler eklenecek.

        // Taşı, tahtanın 2D dizisinde doğru konuma yerleştirir.
        pieces[position.x, position.y] = newPiece;
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
    }
}