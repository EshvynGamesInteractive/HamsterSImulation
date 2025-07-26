using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private bool enableAtStart, showIndicationAtStart;
    public bool isInteractable=true;
    public GameObject indication;
    public Outline outline;
    public abstract void Interact(PlayerScript player);

    private void Awake()
    {
        if (enableAtStart)
        {
            EnableForInteraction(showIndicationAtStart);
        }
        else
            isInteractable = false;
    }
    public void EnableForInteraction(bool showIndication)
    {
        if (indication != null && showIndication)
            indication.SetActive(true);
        isInteractable = true;
    }

    public void DisableForInteraction()
    {
        if (indication != null)
            indication.SetActive(false);
        isInteractable = false;
    }
    public void ShowOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
