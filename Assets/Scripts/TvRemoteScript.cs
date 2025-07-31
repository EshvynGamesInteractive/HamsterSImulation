using DG.Tweening;
using UnityEngine;

public class TvRemoteScript : Interactable
{
    [SerializeField] GameObject tvScreen;
    [SerializeField] Transform switchBtn;
    public override void Interact(PlayerScript player)
    {
        tvScreen.SetActive(false);
        DisableForInteraction(false);
        MainScript.instance.activeLevel.TaskCompleted(3);
        switchBtn.DOLocalRotate(new Vector3(-10, 0, 0), 0.1f);
    }
}
