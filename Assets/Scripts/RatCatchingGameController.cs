using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RatCatchingGameController : MonoBehaviour
{
    [SerializeField] private MiniGameType miniGameType;
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
            Debug.Log("Failed! You could not catch them in time!");
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
            Debug.Log("Success! You got all the rats in time!");
            GlobalValues.TotalBones += 3;

            MainScript.instance.UpdateBonesText();
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
            rat.EnableForInteraction(false);
        }

        MainScript.instance.grandPa.StopTheChase();
        MainScript.instance.grandPa.StartPatrolOnGroundFloor();
        timerText.transform.parent.gameObject.SetActive(true);

        MainScript.instance.taskPanel.UpdateTask("Catch all the rats in time and bring them back to their cage", ratIcon);

        MainScript.instance.pnlInfo.ShowInfo("Catch all the rats in given time.");


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

        
        ratCage.ReturnAllRats(rats);
    }
}