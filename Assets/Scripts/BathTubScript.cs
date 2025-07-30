using DG.Tweening;
using UnityEngine;

public class BathTubScript : Interactable
{
    [SerializeField] Transform blanketPos;
    public override void Interact(PlayerScript player)
    {
        if (player.pickedObject != null && player.pickedObject.TryGetComponent<BlanketScript>(out BlanketScript blanket))
        {
            blanket.DisableForInteraction(false);
            player.PlaceObject(blanketPos.position);
            DisableForInteraction(true);
        }
    }
}
