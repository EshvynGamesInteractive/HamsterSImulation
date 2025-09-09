using UnityEngine;


public abstract class Interactable : MonoBehaviour
{
    
    public Transform indicationPoint;
    [SerializeField] private bool enableAtStart, showIndicationAtStart;
    [SerializeField] GameObject glowingParticle;
    public string interactionText = "Interact";

    public bool isInteractable = true;

    //public GameObject indication;
    [SerializeField] private Outline outline;
    public abstract void Interact(PlayerScript player);

    private void Awake()
    {
        if (outline == null && TryGetComponent<Outline>(out var outl))
        {
            outl.enabled = false;
            outline = outl;
        }


        if (enableAtStart)
        {
            EnableForInteraction(showIndicationAtStart);
        }
        //else
        //{
        //    DisableForInteraction(true);
        //}
    }

    public virtual void EnableForInteraction(bool showIndication)
    {
        Debug.Log(GetComponent<Collider>());
        GetComponent<Collider>().enabled = true;
        //if (glowingParticle != null)
        //{
        //    glowingParticle.SetActive(true);
        //}

        //if (indication != null && showIndication)
        //    indication.SetActive(true);

        if (showIndication)
        {
            ShowOutline();
            MainScript.instance.SetIndicationPosition(transform);
        }
        // else
        // {
        //     MainScript.instance.HideIndication();
        // }

        isInteractable = true;
    }

    public virtual void EnableForInteraction(Sprite indicationIcon)
    {
        Debug.Log(GetComponent<Collider>());
        GetComponent<Collider>().enabled = true;


        ShowOutline();
        MainScript.instance.SetIndicationPosition(transform, indicationIcon);


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