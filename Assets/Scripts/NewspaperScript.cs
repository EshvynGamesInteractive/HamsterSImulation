using DG.Tweening;
using UnityEngine;

public class NewspaperScript : Pickable
{
    [SerializeField] private bool startGrandpaChase;

    public override void Interact(PlayerScript player)
    {
        PickItem(player);

        if (startGrandpaChase)
        {
            DOVirtual.DelayedCall(3, () =>
        {
            MainScript.instance.grandPa.ChasePlayerForDuration(30);
        });
        }

    }
}
