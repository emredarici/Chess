using UnityEngine;

public class LastMoveTracker : MonoBehaviour
{
    public GameObject lastMoveIndicator;
    public GameObject captureIndicator;

    void OnEnable()
    {
        BoardManager.PieceMoved += OnPieceMoved;
    }

    private void OnDestroy()
    {
        BoardManager.PieceMoved -= OnPieceMoved;
    }

    private void Start()
    {
        if (lastMoveIndicator != null && captureIndicator != null)
        {
            lastMoveIndicator.SetActive(false);
            captureIndicator.SetActive(false);
        }
    }

    public void ShowLastMove(Vector2Int from, Vector2Int to)
    {
        if (lastMoveIndicator != null)
        {
            lastMoveIndicator.SetActive(true);
            captureIndicator.SetActive(true);
            lastMoveIndicator.transform.position = new Vector3(from.x, from.y, 0.01f);
            captureIndicator.transform.position = new Vector3(to.x, to.y, 0.01f);
        }
    }

    private void OnPieceMoved(LastMoveInfo info)
    {
        ShowLastMove(info.from, info.to);
    }

}
