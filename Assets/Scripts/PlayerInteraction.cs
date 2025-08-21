using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")] public Transform playerHead; // Assign this in the Inspector
    public float interactionRange = 3f;
    public LayerMask interactableLayer;

    [Header("UI")] [SerializeField] Text txtInteraction;
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

        if (Input.GetKeyDown(KeyCode.E))
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
                // If it's a new interactable
                if (currentInteractable != interactable)
                {
                    // Hide old outline
                    currentInteractable?.HideOutline();

                    // Assign new one
                    currentInteractable = interactable;

                    // Update UI
                    txtInteraction.text = interactable.interactionText;
                    interactButton?.SetActive(true);

                    // Show new outline
                    currentInteractable.ShowOutline();
                }

                return; // stop here, since we have a valid target
            }
        }

        // If we got here → no interactable hit
        if (currentInteractable != null)
        {
            currentInteractable.HideOutline();
            currentInteractable = null;
        }
        interactButton?.SetActive(false);
    }

    // void CheckForInteractable()
    // {
    //     if (playerHead == null)
    //     {
    //         Debug.LogWarning("Player head not assigned in PlayerInteraction script.");
    //         return;
    //     }
    //
    //     Ray ray = new Ray(playerHead.position, playerHead.forward);
    //     RaycastHit hit;
    //
    //     if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
    //     {
    //         Interactable interactable = hit.collider.GetComponent<Interactable>();
    //         // If we are looking at a new interactable, hide outline on the old one
    //         if (currentInteractable != null && currentInteractable != interactable)
    //         {
    //             currentInteractable.HideOutline();
    //         }
    //
    //         if (interactable != null && interactable.isInteractable)
    //         {
    //             currentInteractable = interactable;
    //             if (txtInteraction.text != interactable.interactionText)
    //                 txtInteraction.text = interactable.interactionText;
    //             interactButton?.SetActive(true);
    //
    //             currentInteractable.ShowOutline();
    //             return;
    //         }
    //     }
    //
    //     currentInteractable?.HideOutline();
    //     currentInteractable = null;
    //     interactButton?.SetActive(false);
    // }

    public void TryInteract()
    {
        if (currentInteractable != null)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.interact);
            currentInteractable.Interact(player);
        }
    }
}