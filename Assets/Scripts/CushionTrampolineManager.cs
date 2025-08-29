using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class CushionTrampolineManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Transform exitPoint;

    [SerializeField] Transform[] cushionPositions;
    [SerializeField] Transform[] cushions;
    [SerializeField] GameObject cookieJar;
    [SerializeField] Transform cookieJarPosition;
    [SerializeField] GameObject jarCamera;
    [SerializeField] private Transform stackPoint;
    [SerializeField] private GameObject gameStartTrigger;
    [SerializeField] private GameObject jumpPowerUI;
    [SerializeField] private Text txtTimer;
    [SerializeField] float cushionHeight;
    [SerializeField] private Sprite cookieJarIcon;
    public bool jarPicked = false;

    private PlayerScript playerScript;
    private FP_Controller player;


    [Header("Settings")] [SerializeField] private float gameDuration = 30f;
    [SerializeField] private int maxCushions = 5;
    private float timer;
    private bool isGameActive;
    private List<Cushion> stackedCushions = new();
    private Vector3 jarCamPos;
    private Quaternion jarCamRot;

    //public void StackCushion(Cushion cushion)
    //{
    //    Vector3 stackPos = stackPoint.position + Vector3.up * stackedCushions.Count * cushionHeight;
    //    cushion.transform.position = stackPos;
    //    cushion.transform.rotation = Quaternion.identity;
    //    //cushion.GetComponent<Rigidbody>().isKinematic = true; // Freeze in place

    //    stackedCushions.Add(cushion);
    //}
    public Vector3 GetStackPos(Cushion cushion)
    {
        Vector3 stackPos = stackPoint.position + Vector3.up * stackedCushions.Count * cushionHeight;
        Debug.Log(stackPos);
        stackedCushions.Add(cushion);
        return stackPos;
    }

    private void OnEnable()
    {
        if (MiniGameManager.Instance != null)
            MiniGameManager.Instance.OnMiniGameStart += OnMiniGameStarted;
    }

    private void OnDisable()
    {
        if (MiniGameManager.Instance != null)
            MiniGameManager.Instance.OnMiniGameStart -= OnMiniGameStarted;
    }

    private void Start()
    {
        stackPoint.gameObject.SetActive(false);
        jarCamPos = jarCamera.transform.position;
        jarCamRot = jarCamera.transform.rotation;
        playerScript = MainScript.instance.player;

        player = playerScript.GetComponent<FP_Controller>();
        cookieJar.SetActive(false);
        txtTimer.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isGameActive) return;


        if (player == null) return;
        if (Vector3.Distance(player.transform.position, exitPoint.position) < 1f
            && playerScript.pickedObject != null && playerScript.pickedObject.TryGetComponent<CookieJar>(out _))
        {
            Debug.Log("Success! You escaped with food!");
            EndMiniGame();
        }

        // if (!jarPicked)
        // {
        timer -= Time.deltaTime;
        txtTimer.text = timer.ToString("f0");

        if (timer <= 0f)
            EndMiniGame();
        // }
    }

    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != MiniGameType.CushionTrampoline) return;
        jarPicked = false;
        MainScript.instance.taskPanel.UpdateTask("Stack cushions on the marked spot and jump to reach the cookie jar!",
            cookieJarIcon);


        stackPoint.gameObject.SetActive(true);
        PlayerScript playerr = player.GetComponent<PlayerScript>();
        playerr.DisablePlayer();
        //bubbleCam.transform.position = playerr.playerCamera.position;
        //bubbleCam.transform.rotation = playerr.playerCamera.rotation;
        jarCamera.SetActive(true);

        float camMoveDuration = 2f;

        jarCamera.transform.DOMove(playerr.playerCamera.position, camMoveDuration).SetEase(Ease.Linear);
        jarCamera.transform.DORotate(playerr.playerCamera.eulerAngles, camMoveDuration / 4)
            .SetDelay(camMoveDuration - (camMoveDuration / 4));

        DOVirtual.DelayedCall(camMoveDuration, () =>
        {
            jarCamera.SetActive(false);
            jarCamera.transform.position = jarCamPos;
            jarCamera.transform.rotation = jarCamRot;
            playerr.EnablePlayer();
        });


        //for (int i = 0; i < cushionPositions.Length; i++)
        //{
        //    cushions[i].position = cushionPositions[i].position;
        //    cushions[i].rotation = cushionPositions[i].rotation;
        //    cushions[i].GetComponent<Interactable>().EnableForInteraction(false);
        //    cushions[i].gameObject.SetActive(true);
        //}
        PlaceCushionsOnOriginalPosition();

        cookieJar.transform.position = cookieJarPosition.position;
        cookieJar.transform.rotation = cookieJarPosition.rotation;
        cookieJar.SetActive(true);
        cookieJar.GetComponent<Interactable>().EnableForInteraction(false);


        isGameActive = true;
        timer = gameDuration;
        //stackedCushions.Clear();

        txtTimer.transform.parent.gameObject.SetActive(true);

        gameStartTrigger.SetActive(false);

        MainScript.instance.pnlInfo.ShowInfo("Stack cushions and jump to get the cookie jar!");
    }

    public bool CanAddMoreCushions() => stackedCushions.Count < maxCushions;

    public void AddCushion(Cushion cushion)
    {
        if (!CanAddMoreCushions()) return;

        cushion.transform.position = stackPoint.position + Vector3.up * stackedCushions.Count * cushion.Height;
        stackedCushions.Add(cushion);
    }


    public void CookieJarPicked()
    {
        // txtTimer.transform.parent.gameObject.SetActive(false);
        jarPicked = true;
    }

    public void StartJumpSequence()
    {
        player.DoTrampolineJump(CalculateBounceForce());
        //player.DoTrampolineJump(CalculateBounceForce(), trampolineCamera.transform);
    }

    private float CalculateBounceForce()
    {
        float totalForce = 0f;
        foreach (var c in stackedCushions)
            totalForce += c.BounceForce;

        return totalForce;
    }

    private void EndMiniGame()
    {
        cookieJar.SetActive(false);


        DOVirtual.DelayedCall(2, () =>
        {
            if (playerScript.pickedObject != null)
                playerScript.ThrowObject();
        });
       
        stackPoint.gameObject.SetActive(false);
        isGameActive = false;
        txtTimer.transform.parent.gameObject.SetActive(false);

       

        MiniGameManager.Instance.EndMiniGame();
        MainScript.instance.pnlInfo.ShowInfo("Mini-game over! Try again anytime.");

        //for (int i = 0; i < cushions.Length; i++)
        //{
        //    cushions[i].gameObject.SetActive(false);
        //}

        PlaceCushionsOnOriginalPosition();
        stackedCushions.Clear();
    }

    private void PlaceCushionsOnOriginalPosition()
    {
        if (playerScript.pickedObject != null && playerScript.pickedObject.TryGetComponent<Cushion>(out _))
        {
            //playerScript.PlaceObject(cushions[0].position);
            playerScript.ThrowObject();
        }

        for (int i = 0; i < cushionPositions.Length; i++)
        {
            cushions[i].position = cushionPositions[i].position;
            cushions[i].rotation = cushionPositions[i].rotation;
            cushions[i].GetComponent<Interactable>().EnableForInteraction(false);
            cushions[i].gameObject.SetActive(true);
        }
    }
}