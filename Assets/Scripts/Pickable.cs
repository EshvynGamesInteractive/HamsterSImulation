using UnityEngine;

public class Pickable : Interactable
{

    protected void PickItem(PlayerScript player)
    {
        if (player != null && !player.IsObjectPicked)
        {
            player.PickObject(this);
        }
    }
    public override void Interact(PlayerScript player)
    {
        PickItem(player);
    }
}
