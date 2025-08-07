using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Transform playerHead; // Assign this in the Inspector
    public float interactionRange = 3f;
    public LayerMask interactableLayer;

    [Header("UI")]
    public GameObject interactButton;

    private Interactable currentInteractable;
    private PlayerScript player;

    private void Start()
    {
        player = GetComponent<PlayerScript>();
    }

    void Update()
    {
        CheckForInteractable();

        if(Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.ThrowObject();
        }
    }

    void CheckForInteractable()
    {
        if (playerHead == null)
        {
            Debug.LogWarning("Player head not assigned in PlayerInteraction script.");
            return;
        }

        Ray ray = new Ray(playerHead.position, playerHead.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null && interactable.isInteractable)
            {
                currentInteractable = interactable;
                interactButton?.SetActive(true);
                //currentInteractable.ShowOutline();
                return;
            }
        }
        //currentInteractable?.HideOutline();
        currentInteractable = null;
        interactButton?.SetActive(false);
    }

    public void TryInteract()
    {
        if (currentInteractable != null)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.interact);
            currentInteractable.Interact(player);
        }
    }
}
