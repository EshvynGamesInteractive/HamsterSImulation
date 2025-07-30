using UnityEngine;

public class BlanketScript : Pickable
{
    [SerializeField] Interactable bathTub;
    public override void Interact(PlayerScript player)
    {
        bathTub.EnableForInteraction(true);
        PickItem(player);
    }
}
