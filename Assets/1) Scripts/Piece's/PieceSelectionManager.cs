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
            selectedPiece.board.OnPieceDropped(selectedPiece, targetGridPosition);
            bool promotionPending = false;
            if (selectedPiece.pieceType == PieceType.Pawn)
            {
                int y = targetGridPosition.y;
                if (y == 0 || y == 7)
                {
                    promotionPending = true;
                }
            }

            selectedPiece = null;
            ClearIndicators();
            if (!promotionPending)
            {
                TurnManager.SwitchTurn();
            }
        }
        else
        {
            Debug.Log("Geçersiz hareket denemesi.");
            selectedPiece = null;
            ClearIndicators();
        }
    }

    private void ShowValidMoves(Piece piece)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector2Int targetPos = new Vector2Int(x, y);

                if (piece.IsMoveValid(targetPos))
                {
                    GameObject indicator = Instantiate(moveIndicatorPrefab, indicatorsParent);
                    indicator.transform.position = piece.board.GetWorldPosition(targetPos);

                    MoveIndicator indicatorScript = indicator.GetComponent<MoveIndicator>();
                    if (indicatorScript != null)
                    {
                        indicatorScript.gridPosition = targetPos;
                    }

                    activeIndicators.Add(indicator);
                }
            }
        }
    }

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
