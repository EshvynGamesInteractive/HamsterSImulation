using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerScript : MonoBehaviour
{
    public Transform playerCamera;
    public GameObject playerCanvas;
    [Header("Pickup Settings")]
    [SerializeField] private Transform pickedItemHolder, pickedCushionHolder;
    [SerializeField] private float moveDuration = 0.3f;

    [SerializeField] private float throwForce = 5f;
    [SerializeField] GameObject btnThrow;
    [SerializeField] private Transform playerHead, playerModel, caughtCamera;
    [SerializeField] private GrandpaAI grandpa;


    public bool IsObjectPicked { get; private set; }
    public Pickable pickedObject;


    [Header("Death Effect")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float deathTiltAngle = 75f;
    [SerializeField] private float deathDuration = 1f;

    public void DisablePlayer()
    {
        playerCanvas.SetActive(false);
        gameObject.SetActive(false);
        GetComponent<FP_Controller>().StopPlayerMovement();
    }
    public void EnablePlayer()
    {
        playerCanvas.SetActive(true);
        gameObject.SetActive(true);
        GetComponent<FP_Controller>().canControl = true;
    }
    public void PickObject(Pickable itemToPick)
    {
        if (IsObjectPicked || itemToPick == null) return;
        btnThrow.SetActive(true);
        if (itemToPick.TryGetComponent<Rigidbody>(out Rigidbody existingRb))
            Destroy(existingRb);
        itemToPick.DisableForInteraction();
        pickedObject = itemToPick;
        IsObjectPicked = true;

        if (itemToPick.TryGetComponent<Cushion>(out Cushion existingCushion))
            pickedObject.transform.SetParent(pickedCushionHolder);
        else
            pickedObject.transform.SetParent(pickedItemHolder);
        pickedObject.transform.DOLocalMove(Vector3.zero, moveDuration);
        pickedObject.transform.DOLocalRotate(Vector3.zero, moveDuration);

        ChangeObjectLayer(pickedObject.transform, "PickedLayer");
    }

    public void ThrowObject()
    {
        if (!IsObjectPicked || pickedObject == null) return;
        btnThrow.SetActive(false);
        pickedObject.EnableForInteraction(false);
        pickedObject.transform.SetParent(null);
        Rigidbody rb = pickedObject.gameObject.AddComponent<Rigidbody>();
        rb.AddForce(playerHead.forward * throwForce, ForceMode.Impulse);

        ChangeObjectLayer(pickedObject.transform, "Default");
        pickedObject = null;
        IsObjectPicked = false;
        if (grandpa != null)
            grandpa.NotifyPlayerHasThrown();
    }
    public void PlaceObject(Vector3 placementPos)
    {
        if (!IsObjectPicked || pickedObject == null) return;
        btnThrow.SetActive(false);
        pickedObject.EnableForInteraction(false);
        //Rigidbody rb = pickedObject.gameObject.AddComponent<Rigidbody>();
        pickedObject.transform.DOMove(placementPos, 0.3f);
        ChangeObjectLayer(pickedObject.transform, "Default");
        pickedObject.transform.SetParent(null);
        pickedObject = null;
        IsObjectPicked = false;
    }
    public bool HasPickedObject()
    {
        return IsObjectPicked && pickedObject != null;
    }

    public void PlayerCaught()
    {
        MainScript.instance.pnlInfo.ShowInfo("You have been caught");

        GetComponent<FP_Controller>().StopPlayerMovement();

        playerHead.gameObject.SetActive((false));
        playerModel.gameObject.SetActive((true));
        caughtCamera.gameObject.SetActive((true));

        float x = 0, y = 5, z = -4;

        // caughtCamera.LookAt(transform);
        caughtCamera.DOLocalMove(new Vector3(x, y, z), 2);

        // if (TryGetComponent<Animator>(out Animator animator))
        //     animator.SetTrigger("die"); // Optional: trigger death animation

        Debug.Log("Player has died.");

        // DOVirtual.DelayedCall(0.5f, DeathFallEffect)

        //DeathFallEffect();  



        // Optional: Disable player components, show game over, etc.
        // e.g., GetComponent<CharacterController>().enabled = false;
    }
    private void DeathFallEffect()
    {
        GetComponent<FP_CameraLook>().enabled = false;
        Quaternion endRot = Quaternion.Euler(0, 0, deathTiltAngle);
        Vector3 endPos = cameraTransform.localPosition;
        endPos.y = -0.15f;

        cameraTransform.DOLocalRotateQuaternion(endRot, deathDuration).SetEase(Ease.InOutQuad);
        cameraTransform.DOLocalMove(endPos, deathDuration).SetEase(Ease.InOutQuad);
    }


    public void ChangeObjectLayer(Transform item, string newLayerName)
    {
        int newLayer = LayerMask.NameToLayer(newLayerName);
        if (newLayer == -1)
        {
            Debug.LogWarning("Invalid layer name: " + newLayerName);
            return;
        }

        item.gameObject.layer = newLayer;
        foreach (Transform child in item.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = newLayer;
        }
    }

}
