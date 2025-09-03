using UnityEngine;

public class HensCage : Interactable
{
    [SerializeField] HenScript[] hens;
    public override void Interact(PlayerScript player)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.hens);
        SoundManager.instance.PlaySoundDelayed(SoundManager.instance.hens, 4);
        SoundManager.instance.PlaySoundDelayed(SoundManager.instance.hens, 8);
        player.AnimatePawToInteract();
        MainScript.instance.activeLevel.TaskCompleted(4);
        DisableForInteraction(true);
        for (int i = 0; i < hens.Length; i++)
        {
            hens[i].CauseChaos();
        }
    }
}
