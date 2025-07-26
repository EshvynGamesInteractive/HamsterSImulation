using UnityEngine;

public class FoodItemScript : Pickable
{
    public override void Interact(PlayerScript player)
    {
        base.PickItem(player);

        MiniGameManager.Instance.fridgeLevel.FridgeItemStolen();
    }
}
