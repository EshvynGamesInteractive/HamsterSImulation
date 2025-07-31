using UnityEngine;

public class Pickable : Interactable
{
    [SerializeField] GameObject dropPos;


    protected void PickItem(PlayerScript player)
    {
        if (player != null && !player.IsObjectPicked)
        {
            if (dropPos != null)
            {
                dropPos.SetActive(true);
                MainScript.instance.SetIndicationPosition(dropPos.transform);
            }
            player.PickObject(this);
        }
    }
    public override void Interact(PlayerScript player)
    {
        PickItem(player);
    }
}
