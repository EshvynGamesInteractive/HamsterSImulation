using DG.Tweening;
using UnityEngine;

public class LightSwitchScript : Interactable
{
    [SerializeField] Transform switchBtn;
    [SerializeField] Interactable nextSwitch;
    [SerializeField] bool isFinalSwitch;
    [SerializeField] GameObject lightToTurnOff;
    public override void Interact(PlayerScript player)
    {
        lightToTurnOff.SetActive(false);
        DisableForInteraction(false);
        switchBtn.DOLocalRotate(new Vector3(-10, 0, 0), 0.1f);
        SoundManager.instance.PlaySound(SoundManager.instance.lightSwitch);
        if (!isFinalSwitch)
        {
            nextSwitch.EnableForInteraction(true);
        }
        else
        {
            MainScript.instance.activeLevel.TaskCompleted(4);
        }
    }
}
