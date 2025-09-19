using UnityEngine;

public static class TurnManager
{
    public static PieceColor currentTurn = PieceColor.White;

    public static void SwitchTurn()
    {
        currentTurn = (currentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
    }
}
