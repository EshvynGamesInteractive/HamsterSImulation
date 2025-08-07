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
    [SerializeField] private Transform playerHead, playerModel, cinematicCamera;
    [SerializeField] private GrandpaAI grandpa;
    [SerializeField] private Transform rightPaw; // Walking right paw, parented to player
    [SerializeField] private Transform leftPaw; // Walking left paw, parented to player
    [SerializeField] private Transform animRightPaw; // Animation right paw, parented to camera
    [SerializeField] private Transform animLeftPaw; // Animation left paw, parented to camera
    [SerializeField] private float pawCenterDistance = 0.5f;

    public bool IsObjectPicked { get; private set; }
    public Pickable pickedObject;

    [Header("Death Effect")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float deathTiltAngle = 75f;
    [SerializeField] private float deathDuration = 1f;

    private Vector3 pawOriginalLocalPosition;
    private Vector3 leftPawOriginalLocalPosition;
    private Vector3 animRightPawOriginalLocalPosition;
    private Vector3 animLeftPawOriginalLocalPosition;
    private Vector3 dogCamOriginalPos;
    private Vector3 dogCamOriginalRot;
    private bool isPicking;

    void Start()
    {
        dogCamOriginalPos = cinematicCamera.transform.localPosition;
        dogCamOriginalRot = cinematicCamera.transform.localEulerAngles;
        if (rightPaw) pawOriginalLocalPosition = rightPaw.localPosition;
        if (leftPaw) leftPawOriginalLocalPosition = leftPaw.localPosition;
        if (animRightPaw) animRightPawOriginalLocalPosition = animRightPaw.localPosition;
        if (animLeftPaw) animLeftPawOriginalLocalPosition = animLeftPaw.localPosition;
        if (!playerCamera || !rightPaw || !leftPaw || !animRightPaw || !animLeftPaw)
        {
            Debug.LogError("Camera or paws missing.");
        }
    }

    public void DisablePlayer()
    {
        Debug.Log("disableeeeeeee");
        playerCanvas.SetActive(false);
        gameObject.SetActive(false);
        GetComponent<FP_Controller>().StopPlayerMovement();
    }

    public void EnablePlayer()
    {
        Debug.Log("aaa");
        playerCanvas.SetActive(true);
        gameObject.SetActive(true);
        GetComponent<FP_Controller>().canControl = true;
    }

    public void AnimatePawToCenter()
    {
        //if (isPicking) return;
        isPicking = true;

        // Disable walking paws, enable animation paws
        rightPaw.gameObject.SetActive(false);
        animRightPaw.gameObject.SetActive(true);

        float xRot = -120;
        Sequence pawRotateSequence = DOTween.Sequence();
        pawRotateSequence.Append(animRightPaw.DOLocalRotate(new Vector3(xRot, 0, 0), moveDuration).SetEase(Ease.InOutQuad));
        pawRotateSequence.Append(animRightPaw.DOLocalRotate(Vector3.zero, moveDuration).SetEase(Ease.InOutQuad));

        pawRotateSequence.OnComplete(() =>
        {
            rightPaw.gameObject.SetActive(true);
            animRightPaw.gameObject.SetActive(false);
            isPicking = false;
        });
    }

    public void AnimatePawsForJump()
    {
        if (isPicking) return;
        isPicking = true;

        // Disable walking paws, enable animation paws
        rightPaw.gameObject.SetActive(false);
        leftPaw.gameObject.SetActive(false);
        animRightPaw.gameObject.SetActive(true);
        animLeftPaw.gameObject.SetActive(true);

        float xRot = -110;

        Sequence pawSequence = DOTween.Sequence();
        // Move both paws to center
        pawSequence.Append(animRightPaw.DOLocalRotate(new Vector3(xRot, 0, 0), moveDuration).SetEase(Ease.InOutQuad));
        pawSequence.Join(animLeftPaw.DOLocalRotate(new Vector3(xRot, 0, 0), moveDuration).SetEase(Ease.InOutQuad));
        // Pause
        pawSequence.AppendInterval(moveDuration);
        // Return both paws
        pawSequence.Append(animRightPaw.DOLocalRotate(new Vector3(-20, 0, 0), moveDuration).SetEase(Ease.InOutQuad));
        pawSequence.Join(animLeftPaw.DOLocalRotate(new Vector3(-20, 0, 0), moveDuration).SetEase(Ease.InOutQuad));
        pawSequence.OnComplete(() =>
        {
            rightPaw.gameObject.SetActive(true);
            leftPaw.gameObject.SetActive(true);
            animRightPaw.gameObject.SetActive(false);
            animLeftPaw.gameObject.SetActive(false);
            isPicking = false;
        });
    }

    public void PickObject(Pickable itemToPick)
    {
        if (IsObjectPicked || itemToPick == null || isPicking) return;
        isPicking = true;
        if (itemToPick.TryGetComponent<BlanketScript>(out BlanketScript blanket) ||
            itemToPick.TryGetComponent<NewspaperScript>(out NewspaperScript news))
            btnThrow.SetActive(false);
        else
            btnThrow.SetActive(true);

        if (itemToPick.TryGetComponent<Rigidbody>(out Rigidbody existingRb))
            Destroy(existingRb);
        itemToPick.DisableForInteraction(false);
        pickedObject = itemToPick;
        IsObjectPicked = true;

        //if (itemToPick.TryGetComponent<Cushion>(out Cushion existingCushion))
        //    pickedObject.transform.SetParent(pickedCushionHolder);
        //else
        DOVirtual.DelayedCall(moveDuration, () =>
        {
            pickedObject.transform.SetParent(pickedItemHolder);
            pickedObject.transform.DOLocalMove(Vector3.zero, moveDuration);
            pickedObject.transform.DOLocalRotate(Vector3.zero, moveDuration);
            ChangeObjectLayer(pickedObject.transform, "PickedLayer");
        });
        AnimatePawToCenter();
    }

    public void ThrowObject()
    {
        if (!IsObjectPicked || pickedObject == null) return;
        SoundManager.instance.PlaySound(SoundManager.instance.throwItem);
        btnThrow.SetActive(false);
        pickedObject.EnableForInteraction(false);
        pickedObject.transform.SetParent(null);
        Rigidbody rb = pickedObject.gameObject.AddComponent<Rigidbody>();
        rb.AddForce(playerHead.forward * throwForce, ForceMode.Impulse);

        Transform objectThrown = pickedObject.transform;
        DOVirtual.DelayedCall(moveDuration + 0.2f, () =>
        {
            ChangeObjectLayer(objectThrown, "Default");  //added delay so object doesnt collide with player

        });

        if (pickedObject.TryGetComponent<WaterBalloon>(out WaterBalloon balloon))
            MainScript.instance.activeLevel.TaskCompleted(5);


        pickedObject = null;
        IsObjectPicked = false;
        if (grandpa != null)
            grandpa.NotifyPlayerHasThrown();


    }

    public void ShowDogCamera(Vector3 pos)
    {

    }

    public void PlaceObject(Vector3 placementPos)
    {
        if (!IsObjectPicked || pickedObject == null) return;
        btnThrow.SetActive(false);
        pickedObject.EnableForInteraction(false);
        pickedObject.transform.DOLocalJump(placementPos, 1, 1, 0.5f);
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
        MainScript.instance.PlayerCaught();
        cinematicCamera.gameObject.SetActive(true);
        GetComponent<FP_Controller>().StopPlayerMovement();
        playerHead.gameObject.SetActive(false);
        playerCanvas.SetActive(false);
        //caughtCamera.gameObject.SetActive(true);
        //ShowDogModel();
    }

    public void PlayereRevived()
    {
        MainScript.instance.PlayerRevived();
        HideDogModel();
    }

    public void ShowDogModel()
    {
        cinematicCamera.gameObject.SetActive(true);
        GetComponent<FP_Controller>().StopPlayerMovement();
        playerHead.gameObject.SetActive(false);
        playerModel.gameObject.SetActive(true);
        playerCanvas.SetActive(false);
        //caughtCamera.DOLocalMove(new Vector3(0, 5, -4), 2);
    }

    public void HideDogModel()
    {
        cinematicCamera.gameObject.SetActive(false);
        GetComponent<FP_Controller>().canControl = true;
        playerHead.gameObject.SetActive(true);
        playerModel.gameObject.SetActive(false);
        playerCanvas.SetActive(true);
    }
    public void PlayDogEatingAnim()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.eat);
        playerModel.GetComponent<Animator>().SetTrigger("Eating");
    }
    public void ShowAndHideDog(float delay)
    {
        ShowDogModel();
        cinematicCamera.gameObject.SetActive(true);
        cinematicCamera.transform.localPosition = dogCamOriginalPos;
        cinematicCamera.transform.localEulerAngles = dogCamOriginalRot;
        cinematicCamera.GetComponent<CameraVisibilityAdjuster>().enabled = false;

        DOVirtual.DelayedCall(delay, () =>
        {
            cinematicCamera.DOMove(playerCamera.position, 0.5f);
            cinematicCamera.DORotateQuaternion(playerCamera.rotation, 0.5f).OnComplete(() =>
            {
                playerHead.gameObject.SetActive(true);
                playerModel.gameObject.SetActive(false);
                cinematicCamera.gameObject.SetActive(false);
                GetComponent<FP_Controller>().canControl = true;
                cinematicCamera.GetComponent<CameraVisibilityAdjuster>().enabled = true;
                playerCanvas.SetActive(true);
            });
        });




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





































//using System.Collections;
//using UnityEngine;
//using DG.Tweening;

//public class PlayerScript : MonoBehaviour
//{
//    public Transform playerCamera;
//    public GameObject playerCanvas;
//    [Header("Pickup Settings")]
//    [SerializeField] private Transform pickedItemHolder, pickedCushionHolder;
//    [SerializeField] private float moveDuration = 0.3f;

//    [SerializeField] private float throwForce = 5f;
//    [SerializeField] GameObject btnThrow;
//    [SerializeField] private Transform playerHead, playerModel, caughtCamera;
//    [SerializeField] private GrandpaAI grandpa;


//    public bool IsObjectPicked { get; private set; }
//    public Pickable pickedObject;


//    [Header("Death Effect")]
//    [SerializeField] private Transform cameraTransform;
//    [SerializeField] private float deathTiltAngle = 75f;
//    [SerializeField] private float deathDuration = 1f;

//    public void DisablePlayer()
//    {
//        playerCanvas.SetActive(false);
//        gameObject.SetActive(false);
//        GetComponent<FP_Controller>().StopPlayerMovement();
//    }
//    public void EnablePlayer()
//    {
//        playerCanvas.SetActive(true);
//        gameObject.SetActive(true);
//        GetComponent<FP_Controller>().canControl = true;
//    }
//    public void PickObject(Pickable itemToPick)
//    {
//        if (IsObjectPicked || itemToPick == null) return;
//        btnThrow.SetActive(true);
//        if (itemToPick.TryGetComponent<Rigidbody>(out Rigidbody existingRb))
//            Destroy(existingRb);
//        itemToPick.DisableForInteraction();
//        pickedObject = itemToPick;
//        IsObjectPicked = true;

//        if (itemToPick.TryGetComponent<Cushion>(out Cushion existingCushion))
//            pickedObject.transform.SetParent(pickedCushionHolder);
//        else
//            pickedObject.transform.SetParent(pickedItemHolder);
//        pickedObject.transform.DOLocalMove(Vector3.zero, moveDuration);
//        pickedObject.transform.DOLocalRotate(Vector3.zero, moveDuration);

//        ChangeObjectLayer(pickedObject.transform, "PickedLayer");
//    }

//    public void ThrowObject()
//    {
//        if (!IsObjectPicked || pickedObject == null) return;
//        btnThrow.SetActive(false);
//        pickedObject.EnableForInteraction(false);
//        pickedObject.transform.SetParent(null);
//        Rigidbody rb = pickedObject.gameObject.AddComponent<Rigidbody>();
//        rb.AddForce(playerHead.forward * throwForce, ForceMode.Impulse);

//        ChangeObjectLayer(pickedObject.transform, "Default");
//        pickedObject = null;
//        IsObjectPicked = false;
//        if (grandpa != null)
//            grandpa.NotifyPlayerHasThrown();
//    }
//    public void PlaceObject(Vector3 placementPos)
//    {
//        if (!IsObjectPicked || pickedObject == null) return;
//        btnThrow.SetActive(false);
//        pickedObject.EnableForInteraction(false);
//        //Rigidbody rb = pickedObject.gameObject.AddComponent<Rigidbody>();
//        pickedObject.transform.DOMove(placementPos, 0.3f);
//        ChangeObjectLayer(pickedObject.transform, "Default");
//        pickedObject.transform.SetParent(null);
//        pickedObject = null;
//        IsObjectPicked = false;
//    }
//    public bool HasPickedObject()
//    {
//        return IsObjectPicked && pickedObject != null;
//    }

//    public void PlayerCaught()
//    {
//        MainScript.instance.pnlInfo.ShowInfo("You have been caught");

//        GetComponent<FP_Controller>().StopPlayerMovement();

//        playerHead.gameObject.SetActive((false));
//        playerModel.gameObject.SetActive((true));
//        caughtCamera.gameObject.SetActive((true));

//        float x = 0, y = 5, z = -4;

//        // caughtCamera.LookAt(transform);
//        caughtCamera.DOLocalMove(new Vector3(x, y, z), 2);

//        // if (TryGetComponent<Animator>(out Animator animator))
//        //     animator.SetTrigger("die"); // Optional: trigger death animation

//        Debug.Log("Player has died.");

//        // DOVirtual.DelayedCall(0.5f, DeathFallEffect)

//        //DeathFallEffect();  



//        // Optional: Disable player components, show game over, etc.
//        // e.g., GetComponent<CharacterController>().enabled = false;
//    }
//    private void DeathFallEffect()
//    {
//        GetComponent<FP_CameraLook>().enabled = false;
//        Quaternion endRot = Quaternion.Euler(0, 0, deathTiltAngle);
//        Vector3 endPos = cameraTransform.localPosition;
//        endPos.y = -0.15f;

//        cameraTransform.DOLocalRotateQuaternion(endRot, deathDuration).SetEase(Ease.InOutQuad);
//        cameraTransform.DOLocalMove(endPos, deathDuration).SetEase(Ease.InOutQuad);
//    }


//    public void ChangeObjectLayer(Transform item, string newLayerName)
//    {
//        int newLayer = LayerMask.NameToLayer(newLayerName);
//        if (newLayer == -1)
//        {
//            Debug.LogWarning("Invalid layer name: " + newLayerName);
//            return;
//        }

//        item.gameObject.layer = newLayer;
//        foreach (Transform child in item.GetComponentsInChildren<Transform>())
//        {
//            child.gameObject.layer = newLayer;
//        }
//    }

//}
