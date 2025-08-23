using UnityEngine;

public class LaundaryDoor : DoorScript
{
    public bool kidsLocked = false;
    public override void Interact(PlayerScript player)
    {
        if (!kidsLocked)
        {
            kidsLocked = true;
           // DisableForInteraction(true);
            MainScript.instance.activeLevel.TaskCompleted(4);
        }

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }
}
