using UnityEngine;

public class FireAlarm : Interactable
{
    [SerializeField] private ParticleSystem bubbleEffect; // Bubble particle effect

    private bool isActivated;

    

    public override void Interact(PlayerScript player)
    {
        if (isActivated) return;
        isActivated = true;
        bubbleEffect.Play();
        SoundManager.instance.PlaySound(SoundManager.instance.fireAlarm);
        DisableForInteraction(false);
        player.GetComponent<FP_Controller>().ForceJump();
        MainScript.instance.activeLevel.TaskCompleted(6);
    }

   
}