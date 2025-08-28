using DG.Tweening;
using UnityEngine;

public class RatCageScript : Interactable
{
    [SerializeField] private Transform[] ratsPositions;
    private int ratReturnedCount = 0;
    public override void Interact(PlayerScript player)
    {
        if (player.HasPickedObject() && player.pickedObject.TryGetComponent<RatAI>(out RatAI rat))
        {
            // player.PlaceObject(ratsPositions[ratReturnedCount].position);
            PlaceRatInCage(player, rat, ratReturnedCount);
            RatReturned();
        }
        else
            MainScript.instance.pnlInfo.ShowInfo("Fetch the rat and bring it back");
    }
    public void PlaceRatInCage(PlayerScript player, RatAI rat, int placeIndex)
    {
        rat.StopMovement();
        rat.DisableForInteraction(true);
        rat.transform.SetParent(ratsPositions[placeIndex]);
        rat.transform.DOLocalMove(Vector3.zero, 0.2f);
        rat.transform.DOLocalRotate(Vector3.zero, 0.2f);

        if (player.pickedObject!=null && player.pickedObject.TryGetComponent<RatAI>(out RatAI ratAI))
        {
            player.ChangeObjectLayer(rat.transform, "Default");
            player.pickedObject = null;
            player.IsObjectPicked = false;
        }
    }

    public void RatReturned()
    {
        ratReturnedCount++;
        MiniGameManager.Instance.ratGameController.PlaceRatInCage(ratReturnedCount);
    }

    public void GameStarted()
    {
        EnableForInteraction(false);
        ratReturnedCount = 0;
    }

    public void ReturnAllRats(RatAI[] rats)
    {
        ratReturnedCount = 0;
        PlayerScript player = MainScript.instance.player;
        for (int i = ratReturnedCount; i < rats.Length; i++)
        {
            PlaceRatInCage(player, rats[i], i);
        }
    }
}
