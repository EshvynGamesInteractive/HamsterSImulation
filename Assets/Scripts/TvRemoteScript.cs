using UnityEngine;

public class TvRemoteScript : Interactable
{
    [SerializeField] GameObject tvScreen;
    public override void Interact(PlayerScript player)
    {
        tvScreen.SetActive(false);
        DisableForInteraction(false);
    }
}
