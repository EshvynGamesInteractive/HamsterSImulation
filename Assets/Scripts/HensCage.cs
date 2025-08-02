using UnityEngine;

public class HensCage : Interactable
{
    [SerializeField] HenScript[] hens;
    public override void Interact(PlayerScript player)
    {
        player.AnimatePawToCenter();
        MainScript.instance.activeLevel.TaskCompleted(4);
        DisableForInteraction(true);
        for (int i = 0; i < hens.Length; i++)
        {
            hens[i].CauseChaos();
        }
    }
}
