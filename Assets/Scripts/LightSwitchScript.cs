using DG.Tweening;
using UnityEngine;

public class LightSwitchScript : Interactable
{
    [SerializeField] Transform switchBtn;
    [SerializeField] Interactable nextSwitch;
    [SerializeField] bool isFinalSwitch;
    public override void Interact(PlayerScript player)
    {
        DisableForInteraction(false);
        switchBtn.DOLocalRotate(new Vector3(-10, 0, 0), 0.1f);

        if(!isFinalSwitch)
        {
            nextSwitch.EnableForInteraction(true);
        }
        else
        {
            MainScript.instance.activeLevel.TaskCompleted(3);
        }
    }
}
