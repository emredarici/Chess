using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverMainText;
    public TextMeshProUGUI gameOverSubText;

    public GameObject modeSelectionPanel;

    private void OnEnable()
    {
        GameManager.onCheckmate += CheckmateText;
        GameManager.onNoLegalMoves += NoLegalMovesText;
        GameManager.onOnly2Kings += OnlyTwoKingsText;
        GameManager.onThirdRepetition += ThirdRepetitionText;
    }

    private void OnDisable()
    {
        GameManager.onCheckmate -= CheckmateText;
        GameManager.onNoLegalMoves -= NoLegalMovesText;
        GameManager.onOnly2Kings -= OnlyTwoKingsText;
        GameManager.onThirdRepetition -= ThirdRepetitionText;
    }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (modeSelectionPanel != null)
            modeSelectionPanel.SetActive(true);
    }

    public void CheckmateText(PieceColor winner)
    {
        gameOverPanel.SetActive(true);
        gameOverMainText.text = "Checkmate!";
        gameOverSubText.text = $"{winner} has won the game.";
    }

    public void NoLegalMovesText()
    {
        gameOverPanel.SetActive(true);
        gameOverMainText.text = "Stelemate!";
        gameOverSubText.text = "No legal moves available.";
    }

    public void OnlyTwoKingsText()
    {
        gameOverPanel.SetActive(true);
        gameOverMainText.text = "Draw!";
        gameOverSubText.text = "Only two kings remain.";
    }

    public void ThirdRepetitionText()
    {
        gameOverPanel.SetActive(true);
        gameOverMainText.text = "Draw!";
        gameOverSubText.text = "The same position has occurred three times.";
    }

    public void ModeSelectionPanel()
    {
        modeSelectionPanel.SetActive(false);
    }
}
