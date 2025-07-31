using UnityEngine;

public class KetchupBoxScript : Pickable
{
    [SerializeField] Interactable teaCup;

    public override void Interact(PlayerScript player)
    {
        PickItem(player);
        teaCup.EnableForInteraction(true);
    }
}
