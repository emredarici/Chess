using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static Action onBoardClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameManager.Instance.IsModeSet)
            return;
        onBoardClicked?.Invoke();
    }
}
