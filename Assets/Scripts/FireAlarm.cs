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

        DisableForInteraction(false);

        MainScript.instance.activeLevel.TaskCompleted(6);
    }

   
}