using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pieceMoveClip;

    void OnEnable()
    {
        BoardManager.PieceMoved += PlayPieceMoveSound;
    }

    public void PlayPieceMoveSound(LastMoveInfo piece)
    {
        audioSource.PlayOneShot(pieceMoveClip);
    }
}

