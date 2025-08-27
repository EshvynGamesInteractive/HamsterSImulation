using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RatCatchingGameController : MonoBehaviour
{
    [SerializeField] private MiniGameType miniGameType;
    [SerializeField] private GameObject gameStartTrigger;
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private Sprite ratIcon;
    [SerializeField] private RatCageScript ratCage;

    [SerializeField] private RatAI[] rats;

    private Camera playerCamera;
    private PlayerScript player;
    public Text timerText;
    private float timer;
    private bool isGameActive;
    private int ratsToBring = 3;

    private void Start()
    {
        player = MainScript.instance.player;
        playerCamera = player.playerCamera.GetComponent<Camera>();
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


    private void Update()
    {
        if (!isGameActive) return;


        timer -= Time.deltaTime;
        UpdateTimerUI();
        if (timer <= 0f)
        {
            EndMiniGame();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText) timerText.text = $"Time: {Mathf.CeilToInt(timer)}";
    }

    public void PlaceRatInCage(int ratsBroughtBack)
    {

        if (ratsBroughtBack >= ratsToBring)
        {
            EndMiniGame();
        }
    }


    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != miniGameType) return;
        ratCage.GameStarted();
        foreach (RatAI rat in rats)
        {
            rat.StartMovement();
        }
        MainScript.instance.grandPa.StopTheChase();
        MainScript.instance.grandPa.StartPatrolOnGroundFloor();
        timerText.transform.parent.gameObject.SetActive(true);

        MainScript.instance.taskPanel.UpdateTask("Catch all the rats in bring them back", ratIcon);

        MainScript.instance.pnlInfo.ShowInfo("Bring all the rats back in time.");


        isGameActive = true;
        timer = gameDuration;


    }

    private void EndMiniGame()
    {
ratCage.DisableForInteraction(true);
        timerText.transform.parent.gameObject.SetActive(false);
        isGameActive = false;
        if (!MainScript.instance.gameover)
        {
            GrandpaAI grandpaAI = MainScript.instance.grandPa;
            grandpaAI.StartPatrolOnGroundFloor();

            MiniGameManager.Instance.EndMiniGame();
        }

    }
}