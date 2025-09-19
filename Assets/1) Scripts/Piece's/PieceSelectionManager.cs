using System.Collections.Generic;
using UnityEngine;

public class PieceSelectionManager : Singeleton<PieceSelectionManager>
{
    [Header("Visiual Settings")]
    public GameObject moveIndicatorPrefab;
    public Transform indicatorsParent;

    public Piece selectedPiece;
    private List<GameObject> activeIndicators = new List<GameObject>();

    void OnEnable()
    {
        BoardClickHandler.onBoardClicked += ClearSelection;
    }

    void OnDisable()
    {
        BoardClickHandler.onBoardClicked -= ClearSelection;
    }

    public void SelectedPiece(Piece piece)
    {
        if (piece.pieceColor != TurnManager.currentTurn)
            return;

        ClearIndicators();
        selectedPiece = piece;
        ShowValidMoves(selectedPiece);
    }

    public void TryMoveSelectedPiece(Vector2Int targetGridPosition)
    {
        if (selectedPiece == null)
        {
            Debug.LogWarning("No piece selected!");
            return;
        }

        if (selectedPiece.IsMoveValid(targetGridPosition))
        {
            // Hareket geçerliyse, BoardManager'a taşı hareket ettirmesini söyle.
            selectedPiece.board.OnPieceDropped(selectedPiece, targetGridPosition);
            selectedPiece = null; // Taşı hareket ettirdikten sonra seçimi kaldır.
            ClearIndicators(); // Belirteçleri temizle.
            TurnManager.SwitchTurn(); // Sırayı değiştir
        }
        else
        {
            // Hareket geçersizse, seçimi iptal et ve belirteçleri temizle.
            Debug.Log("Geçersiz hareket denemesi.");
            selectedPiece = null;
            ClearIndicators();
        }
    }

    private void ShowValidMoves(Piece piece)
    {
        // Tahtadaki tüm kareleri kontrol et.
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector2Int targetPos = new Vector2Int(x, y);

                // Eğer taş bu kareye hareket edebiliyorsa...
                if (piece.IsMoveValid(targetPos))
                {
                    // Belirteç objesini oluştur.
                    GameObject indicator = Instantiate(moveIndicatorPrefab, indicatorsParent);
                    // Belirteci doğru dünya koordinatına yerleştir.
                    indicator.transform.position = piece.board.GetWorldPosition(targetPos);

                    // Belirtecin de hangi grid pozisyonunda olduğunu bilmesi için
                    // ona bir script ekleyebiliriz. (Aşağıda açıklanacak)
                    MoveIndicator indicatorScript = indicator.GetComponent<MoveIndicator>();
                    if (indicatorScript != null)
                    {
                        indicatorScript.gridPosition = targetPos;
                    }

                    activeIndicators.Add(indicator); // Listeye ekle ki sonra temizleyebilelim.
                }
            }
        }
    }

    // Oluşturulan tüm hareket belirteçlerini yok eder.
    public void ClearIndicators()
    {
        foreach (GameObject indicator in activeIndicators)
        {
            Destroy(indicator);
        }
        activeIndicators.Clear();
    }

    public void ClearSelection()
    {
        selectedPiece = null;
        ClearIndicators();
    }
}
