using UnityEngine;

public class CutleryScript : Interactable
{
    public override void Interact(PlayerScript player)
    {
        player.ShowAndHideDog(4);
        DisableForInteraction(false);
        MainScript.instance.activeLevel.TaskCompleted(3);
       
        player.PlayDogEatingAnim();
    }
}
