using DG.Tweening;
using UnityEngine;

public class PianoButtonScript : MonoBehaviour
{
    [SerializeField] private float downVal = 0.1f;
    [SerializeField] private AudioClip buttonSound;
    private float originalPosY;
    private MusicGameController musicGameController;
    private void Start()
    {
        originalPosY = transform.localPosition.y;
        musicGameController = MiniGameManager.Instance.pianoGameController;
    }

    private void OnMouseDown()
    {
        SoundManager.instance.PlaySound(buttonSound);
        transform.DOLocalMoveY(originalPosY - downVal, 0.1f);
        transform.DOLocalMoveY(originalPosY, 0.1f).SetDelay(0.1f);
        musicGameController.ButtonPressed(transform);
    }
}