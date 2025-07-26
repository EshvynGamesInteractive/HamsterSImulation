using DG.Tweening;
using UnityEngine;

public class DoorScript : Interactable
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private Transform door;
    [SerializeField] float animationDuration = 0.2f;
    [SerializeField]private bool isOpen;


    private void Start()
    {
        if(door == null)
            door = transform;
    }
    private void OpenDoor()
    {
        isOpen = true;
        door.DOLocalRotate(new Vector3(0, openAngle, 0), animationDuration);
    }

    private void CloseDoor()
    {
        isOpen = false;
        door.DOLocalRotate(new Vector3(0, closeAngle, 0), animationDuration);
    }

    public override void Interact(PlayerScript player)
    {
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
