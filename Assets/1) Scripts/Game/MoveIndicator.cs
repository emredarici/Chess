using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveIndicator : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int gridPosition;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameManager.Instance.IsModeSet)
            return;
        PieceSelectionManager.Instance.TryMoveSelectedPiece(gridPosition);
    }
}
