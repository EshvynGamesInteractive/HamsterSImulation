using UnityEngine;

public class RatCageScript : Interactable
{
    [SerializeField] private Transform[] ratsPositions;
    private int ratReturnedCount = 0;
    public override void Interact(PlayerScript player)
    {
        if (player.HasPickedObject() && player.pickedObject.TryGetComponent<RatAI>(out RatAI rat))
        {
            player.PlaceObject(ratsPositions[ratReturnedCount].position);
            RatReturned();
        }
        else
            MainScript.instance.pnlInfo.ShowInfo("Fetch the rat and bring it back");
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
}
