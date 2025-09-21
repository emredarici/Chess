using UnityEngine;
using UnityEngine.UI;

public class PromoteController : MonoBehaviour
{
    public GameObject promotePanel;

    public PieceType SelectType;
    private Piece _pendingPawn;

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
        _pendingPawn = pawn;
        promotePanel.SetActive(true);
    }

    public void PromotePawn()
    {
        if (_pendingPawn == null) return;
        Vector2Int pos = _pendingPawn.currentPosition;
        PieceColor color = _pendingPawn.pieceColor;

        Destroy(_pendingPawn.gameObject);
        BoardManager.Instance.PlacePiece(SelectType, color, pos);

        // lastMove'u güncelle ve PieceMoved eventini tetikle
        BoardManager.Instance.lastMove = new LastMoveInfo
        {
            from = pos, // Terfi anında from ve to aynı karede olur
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
