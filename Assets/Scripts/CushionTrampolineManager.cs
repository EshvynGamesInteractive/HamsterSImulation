using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class CushionTrampolineManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject cookieJar;
    [SerializeField] GameObject jarCamera;
    [SerializeField] private Transform stackPoint;
    [SerializeField] private GameObject gameStartTrigger;
    [SerializeField] private GameObject jumpPowerUI;
    [SerializeField] private Text txtTimer;
    [SerializeField] private FP_Controller player; // control jump
    [SerializeField] float cushionHeight;






    [Header("Settings")]
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] private int maxCushions = 5;
    private float timer;
    private bool isGameActive;
    private List<Cushion> stackedCushions = new();


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
        cookieJar.SetActive(false);
        txtTimer.transform.parent.gameObject.SetActive(false);
        
    }

    //private void Update()
    //{
    //    if (!isGameActive) return;

    //    timer -= Time.deltaTime;
    //    txtTimer.text = timer.ToString("f0");

    //    if (timer <= 0f)
    //        EndMiniGame();
    //}

    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != MiniGameType.CushionTrampoline) return;
        PlayerScript playerr = player.GetComponent<PlayerScript>();
        player.canControl = false;
        //bubbleCam.transform.position = playerr.playerCamera.position;
        //bubbleCam.transform.rotation = playerr.playerCamera.rotation;
        jarCamera.SetActive(true);

        float camMoveDuration = 2f;

        jarCamera.transform.DOMove(playerr.playerCamera.position, camMoveDuration).SetEase(Ease.Linear);
        jarCamera.transform.DORotateQuaternion(playerr.playerCamera.rotation, camMoveDuration);

        DOVirtual.DelayedCall(camMoveDuration, () => {
            jarCamera.SetActive(false);
            player.canControl = true;
        });


        cookieJar.SetActive(true);
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
        isGameActive = false;
        txtTimer.transform.parent.gameObject.SetActive(false);
       
        gameStartTrigger.SetActive(true);

        MiniGameManager.Instance.EndMiniGame();
        MainScript.instance.pnlInfo.ShowInfo("Mini-game over! Try again anytime.");
    }
}
