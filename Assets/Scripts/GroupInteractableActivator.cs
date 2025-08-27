using System;
using UnityEngine;

public class GroupInteractableActivator : Interactable
{
    [SerializeField] private BoxCollider[] itemsToEnable;

    private void Start()
    {
        foreach (BoxCollider interactable in itemsToEnable)
        {
            interactable.enabled = false;
        }
    }

    public override void Interact(PlayerScript player)
    {
    }

    public override void EnableForInteraction(bool showIndication)
    {
        if (showIndication)
        {
            ShowOutline();
            MainScript.instance.SetIndicationPosition(itemsToEnable[0].transform);
        }
        else
        {
            MainScript.instance.HideIndication();
        }
        foreach (BoxCollider interactable in itemsToEnable)
        {
            interactable.enabled = true;
            if (interactable.TryGetComponent<Interactable>(out Interactable item))
                item.isInteractable = true;
        }
    }
}