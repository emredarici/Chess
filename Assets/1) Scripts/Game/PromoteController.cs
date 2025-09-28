using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromoteController : MonoBehaviour
{
    public GameObject promotePanel;

    public PieceType SelectType;
    private Piece _pendingPawn;

    public List<SpriteRenderer> promoteOptionRenderers;
    public List<Sprite> whitePieceSprites;
    public List<Sprite> blackPieceSprites;

    public void SetTypeToQueen() { SelectType = PieceType.Queen; }
    public void SetTypeToRook() { SelectType = PieceType.Rook; }
    public void SetTypeToBishop() { SelectType = PieceType.Bishop; }
    public void SetTypeToKnight() { SelectType = PieceType.Knight; }

    void OnEnable()
    {
        BoardManager.PromoteRequested += ShowPromotePanel;
    }
    void OnDisable()
    {
        BoardManager.PromoteRequested -= ShowPromotePanel;
    }

    private void Start()
    {
        promotePanel.SetActive(false);
        _pendingPawn = null;
    }

    public void ShowPromotePanel(Piece pawn)
    {
        if (pawn == null) return;
        if (GameManager.Instance != null
            && GameManager.Instance.currentGameMode == GameMode.PlayervsAI
            && pawn.pieceColor == GameManager.Instance.aiColor)
        {
            _pendingPawn = null;
            if (promotePanel != null) promotePanel.SetActive(false);
            return;
        }

        _pendingPawn = pawn;
        promotePanel.SetActive(true);
        promotePanel.transform.position = new Vector3(promotePanel.transform.position.x, pawn.currentPosition.y, promotePanel.transform.position.z);
        if (pawn.pieceColor == PieceColor.White)
        {
            for (int i = 0; i < promoteOptionRenderers.Count; i++)
            {
                promoteOptionRenderers[i].sprite = whitePieceSprites[i];
            }
        }
        else
        {
            for (int i = 0; i < promoteOptionRenderers.Count; i++)
            {
                promoteOptionRenderers[i].sprite = blackPieceSprites[i];
            }
        }
    }

    public void PromotePawn()
    {
        if (_pendingPawn == null) return;
        Vector2Int pos = _pendingPawn.currentPosition;
        PieceColor color = _pendingPawn.pieceColor;

        Destroy(_pendingPawn.gameObject);
        BoardManager.Instance.PlacePiece(SelectType, color, pos);

        BoardManager.Instance.lastMove = new LastMoveInfo
        {
            from = pos,
            to = pos,
            piece = BoardManager.Instance.GetPiece(pos),
            wasDoublePawnMove = false
        };
        BoardManager.PieceMoved?.Invoke(BoardManager.Instance.lastMove);

        TurnManager.SwitchTurn();

        _pendingPawn = null;
        promotePanel.SetActive(false);
    }
}
