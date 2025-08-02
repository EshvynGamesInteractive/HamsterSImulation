using UnityEngine;

public class ToyTOThrow : Pickable
{
    [SerializeField] Transform posTOThrowAt;
    public override void Interact(PlayerScript player)
    {
        MainScript.instance.SetIndicationPosition(posTOThrowAt);
        PickItem(player);
    }
}
