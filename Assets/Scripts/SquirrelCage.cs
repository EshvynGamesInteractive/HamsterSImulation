using UnityEngine;
using DG.Tweening;

public class SquirrelCage : Interactable
{
    [SerializeField] private SquirrelAI squirrel;
    [SerializeField] Transform cageDoor;
    public override void Interact(PlayerScript player)
    {
        player.AnimatePawToCenter();
        DisableForInteraction(true);
        OpenCageDoor();
    }


    private void OpenCageDoor()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.cageOpen);
        cageDoor.DOLocalRotate(new Vector3(0, 60, 0), 0.3f);
        squirrel.RunTowardsSlipper();
    }
}
