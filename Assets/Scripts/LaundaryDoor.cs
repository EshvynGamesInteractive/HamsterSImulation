using UnityEngine;

public class LaundaryDoor : DoorScript
{
    public override void Interact(PlayerScript player)
    {
        MainScript.instance.activeLevel.TaskCompleted(4);
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
