using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveIndicator : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int gridPosition;

    public void OnPointerClick(PointerEventData eventData)
    {
        PieceSelectionManager.Instance.TryMoveSelectedPiece(gridPosition);
    }
}
