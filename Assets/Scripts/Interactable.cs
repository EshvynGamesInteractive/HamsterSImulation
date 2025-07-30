using UnityEngine;

[RequireComponent (typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private bool enableAtStart, showIndicationAtStart;
    public bool isInteractable=true;
    public GameObject indication;
    private Outline outline;
    public abstract void Interact(PlayerScript player);

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (enableAtStart)
        {
            EnableForInteraction(showIndicationAtStart);
        }
        else
        {
            DisableForInteraction(true);
        }
    }
    public void EnableForInteraction(bool showIndication)
    {
        GetComponent<Collider>().enabled = true;
        if (indication != null && showIndication)
            indication.SetActive(true);
        isInteractable = true;
    }

    public void DisableForInteraction(bool enableCollider)
    {
        GetComponent<Collider>().enabled = enableCollider;
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
