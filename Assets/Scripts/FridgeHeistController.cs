using DG.Tweening;
using System.Collections;
     using UnityEngine;
using UnityEngine.UI;

public class FridgeHeistController : MonoBehaviour
{
    //[SerializeField] private Transform fridge;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private GameObject stealthUI;
    [SerializeField] private float movementFreezeRadius = 0.1f;
    [SerializeField] Transform sitPosition;
    [SerializeField]  Transform foorCamera;
    [SerializeField] GameObject foodTOPick;
    [SerializeField] private Image stealthMeterImage;

    private PlayerScript player;
    private FP_Controller fP_Controller;
    private bool heistActive = false;
    private bool holdingItem = false;
    private bool isFrozen = false;

    private GrandpaAI grandpaAI;
    private GrandpaPeekController grandpaPeekController;


    private void Start()
    {
        player = MainScript.instance.player;
        grandpaAI = MainScript.instance.grandPa;
        grandpaPeekController = grandpaAI.GetComponent<GrandpaPeekController>();
    }

    private void OnEnable()
    {
        stealthMeterImage.gameObject.SetActive(false);
        
        MiniGameManager.Instance.OnMiniGameStart += OnMiniGameStarted;
    }


   

    private void OnDisable()
    {
        MiniGameManager.Instance.OnMiniGameStart -= OnMiniGameStarted;
    }

    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != MiniGameType.FridgeHeist) return;
        
        StartCoroutine(StartHeist());
    }

    private IEnumerator StartHeist()
    {
        player.DisablePlayer();
        //bubbleCam.transform.position = playerr.playerCamera.position;
        //bubbleCam.transform.rotation = playerr.playerCamera.rotation;
        foorCamera.gameObject.SetActive(true);

        float camMoveDuration = 2f;

        foodTOPick.SetActive(true);
        foorCamera.transform.DOMove(player.playerCamera.position, camMoveDuration).SetEase(Ease.Linear);
        foorCamera.transform.DORotate(player.playerCamera.eulerAngles, camMoveDuration / 4).SetDelay(camMoveDuration - (camMoveDuration / 4));

        DOVirtual.DelayedCall(camMoveDuration, () => {
            foorCamera.gameObject.SetActive(false);
            player.EnablePlayer();
        });






        grandpaAI.MakeGrandpaSit(sitPosition);

        grandpaPeekController.enabled = true;

        grandpaPeekController.GameStarted();
        player = MainScript.instance.player;
        fP_Controller = player.GetComponent<FP_Controller>();
        if (player == null) yield break;

        heistActive = true;
        stealthUI?.SetActive(true);
        SlowDownPlayer(true);

        Debug.Log("Fridge Heist started!");
        MainScript.instance.taskPanel.UpdateTask("Sneak some fruit from the fridge. Careful not to alert Grandpa!");
        MainScript.instance.pnlInfo.ShowInfo("Steal items from the fridge without being caught by Grandpa");
        yield return null;
    }

    float maxSpeed = 0.5f;
    private void Update()
    {
        if (!heistActive || player == null) return;
        if (Vector3.Distance(player.transform.position, exitPoint.position) < 1f 
            && player.pickedObject!=null && player.pickedObject.TryGetComponent<FoodItemScript>(out _ )  )
        {
            Debug.Log("Success! You escaped with food!");
            EndHeist();
        }
        float speed = fP_Controller.controller.velocity.magnitude;
        stealthMeterImage.fillAmount = 1f - (speed / maxSpeed); // 0 = full stealth

    }

    private void HandleGrandpaPeek(bool isLooking)
    {
        if (!heistActive || player == null) return;

        if (isLooking)
        {
            if (PlayerIsMoving())
            {
                Debug.Log("Caught! Grandpa saw you moving!");
                FailHeist();
            }
            else
            {
                isFrozen = true;
            }
        }
        else
        {
            isFrozen = false;
        }
    }

    private bool PlayerIsMoving()
    {
        return player.GetComponent<Rigidbody>()?.linearVelocity.magnitude > movementFreezeRadius;
    }

    public void FridgeItemStolen()
    {
        if (!heistActive || holdingItem) return;

        holdingItem = true;
        Debug.Log("Food stolen! Now escape!");
    }

    private void FailHeist()
    {
        grandpaPeekController.GameEnd();
        heistActive = false;
        stealthUI?.SetActive(false);
        Debug.Log("Heist failed!");
        // Optional: play scolding animation or reload

        grandpaPeekController.enabled = false;
    }

    private void EndHeist()
    {
        grandpaAI.StartPatrolOnGroundFloor();
        grandpaPeekController.GameEnd();
        var movement = player.GetComponent<FP_Controller>(); 
        if (movement != null)
            movement.ExitHeistMode();
        heistActive = false;
        stealthUI?.SetActive(false);
        MainScript.instance.pnlInfo.ShowInfo("Heist completed");
        Debug.Log("Heist completed successfully!");
        MiniGameManager.Instance.EndMiniGame();

        grandpaPeekController.enabled = false;
    }

    private void SlowDownPlayer(bool slow)
    {
        var movement = player.GetComponent<FP_Controller>(); 
        if (movement != null)
            movement.EnterHeistMode();
    }
}
