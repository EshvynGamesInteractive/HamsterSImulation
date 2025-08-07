using UnityEngine;

public class Pickable : Interactable
{
    [SerializeField] GameObject dropPos;
    [SerializeField] Transform dropPointIndicationPos;


    protected void PickItem(PlayerScript player)
    {
        if (player != null && !player.IsObjectPicked)
        {
            if (dropPos != null)
            {
                dropPos.SetActive(true);
                dropPos.GetComponent<Collider>().enabled = true;
                if (dropPointIndicationPos != null)
                    MainScript.instance.SetIndicationPosition(dropPointIndicationPos);
                else
                    MainScript.instance.SetIndicationPosition(dropPos.transform);
            }
            player.PickObject(this);
        }
        else
        {
            MainScript.instance.pnlInfo.ShowInfo("Can only pick one item at a time");
        }
    }
    public override void Interact(PlayerScript player)
    {
        MainScript.instance.HideIndication();
        PickItem(player);
    }
}
