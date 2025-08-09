using UnityEngine;


public abstract class Interactable : MonoBehaviour
{
    public Transform indicationPoint;
    [SerializeField] private bool enableAtStart, showIndicationAtStart;
    [SerializeField] GameObject glowingParticle;
    public bool isInteractable = true;
    //public GameObject indication;
    [SerializeField] private Outline outline;
    public abstract void Interact(PlayerScript player);

    private void Awake()
    {
        if (outline == null)
            outline = TryGetComponent<Outline>(out var outl) ? outl : null;

      
        if (enableAtStart)
        {
            EnableForInteraction(showIndicationAtStart);
        }
        //else
        //{
        //    DisableForInteraction(true);
        //}
    }
    public void EnableForInteraction(bool showIndication)
    {
        GetComponent<Collider>().enabled = true;
        //if (glowingParticle != null)
        //{
        //    glowingParticle.SetActive(true);
        //}
        ShowOutline();
        //if (indication != null && showIndication)
        //    indication.SetActive(true);

        if (showIndication)
            MainScript.instance.SetIndicationPosition(transform);

        isInteractable = true;
    }

    public void DisableForInteraction(bool enableCollider)
    {
        HideOutline();
        if (glowingParticle != null)
        {
            glowingParticle.SetActive(false);
        }
        GetComponent<Collider>().enabled = enableCollider;
        //if (indication != null)
        //    indication.SetActive(false);
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
