using DG.Tweening;
using UnityEngine;

public class KitchenFaucet : Interactable
{
    [SerializeField] private ParticleSystem waterFLow;
    [SerializeField] private Transform tap;
    public override void Interact(PlayerScript player)
    {
        tap.DOLocalRotate(new Vector3(0,270, 0), 0.5f);
        waterFLow.Play();
        DisableForInteraction(false);
        MainScript.instance.activeLevel.TaskCompleted(8);
    }
}
