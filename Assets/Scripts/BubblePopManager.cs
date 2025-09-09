using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BubblePopManager : MonoBehaviour
{
    //public static BubblePopManager Instance { get; private set; }

    [SerializeField] private MiniGameType miniGameType;
    [SerializeField] Material stinkyBubble, rubberDuck, shampooBottle, goldenBone;
    [SerializeField] Transform playerEndPos;
    [SerializeField] private Sprite bubbleGameIcon;
    [Header("Mini‑Game Settings")]
    public GameObject bubbleCam;
    public GameObject bubblePrefab;
    public Transform bubbleSpawnArea;
    public float bubbleInterval = 0.5f;
    public float gameDuration = 15f;
    private PlayerScript player;

    [Header("UI")]
    public Text timerText;
    public Text scoreText;
    public Text updateScoreText;

    private float timer;
    private bool gameActive;
    private int score;
    private Material bubbleMat;
    private void Awake()
    {
        //Instance = this;
        player=MainScript.instance.player;
    }


    private void OnEnable()
    {
        MiniGameManager.Instance.OnMiniGameStart += OnMiniGameStarted;
    }




    private void OnDisable()
    {
        MiniGameManager.Instance.OnMiniGameStart -= OnMiniGameStarted;
    }

    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != miniGameType) return;

        MiniGameManager.Instance.bubbleLevel = this;

        MainScript.instance.taskPanel.UpdateTask(
            "Pop as many bubbles as you can. Each gives points! But watch out for the stinky green ones!",
            bubbleGameIcon);
        StartCoroutine(
                StartBubblePopMiniGame());
    }


    public IEnumerator StartBubblePopMiniGame()
    {
           timerText.transform.parent.gameObject.SetActive(true);

        scoreText.transform.parent.gameObject.SetActive(true);
        MainScript.instance.pnlInfo.ShowInfo("Pop the bubbles, Avoid Stinky green ones");

        player.DisablePlayer();
        bubbleCam.SetActive(true);
        bubbleCam.transform.position = player.playerCamera.position;
        bubbleCam.transform.rotation = player.playerCamera.rotation;
        float jumpDuration = 1;
        bubbleCam.transform.DOLocalJump(Vector3.zero, 0.5f ,1, jumpDuration);
        bubbleCam.transform.DOLocalRotate(Vector3.zero, jumpDuration);
        score = 0;
        UpdateScoreUI();
        player.transform.SetPositionAndRotation(playerEndPos.position, playerEndPos.rotation);
        timer = gameDuration;
        UpdateTimerUI();

        gameActive = true;
        yield return new WaitForSeconds(2);
        StartCoroutine(BubbleRoutine());
    }

    private IEnumerator BubbleRoutine()
    {
        while (timer > 0f)
        {
            SpawnBubble();
            yield return new WaitForSeconds(bubbleInterval);
            timer -= bubbleInterval;
            UpdateTimerUI();
        }
        EndMiniGame();
    }

    private void SpawnBubble()
    {
        float areaWidth = 0.1f;
        Vector3 rand = bubbleSpawnArea.position +
            new Vector3(Random.Range(-areaWidth, areaWidth), 0, Random.Range(-areaWidth, areaWidth));

        var b = Instantiate(bubblePrefab, rand, Quaternion.identity)
                    .GetComponent<Bubble>();

        // Randomly choose a bubble type
        BubbleType type = GetRandomBubbleType();
        
        switch(type)
        {
            case BubbleType.StinkBomb:
                bubbleMat = stinkyBubble;
                break;
            case BubbleType.RegularBubble:
                bubbleMat = rubberDuck;
                break;
            case BubbleType.PrizeBubble:
                bubbleMat = shampooBottle;
                break;
            case BubbleType.GlodenBubble:
                bubbleMat = goldenBone  ;
                break;
        }


        b.InitBubble(type, bubbleMat);
    }

    private BubbleType GetRandomBubbleType()
    {
        float r = Random.value;
        if (r < 0.1f) return BubbleType.GlodenBubble;
        if (r < 0.4f) return BubbleType.RegularBubble;
        if (r < 0.7f) return BubbleType.PrizeBubble;
        return BubbleType.StinkBomb;
    }
    public void AddScore(int amount)
    {
        if (!gameActive) return;
        score += amount;
        UpdateScoreUI();
        ShowUpdatedScore("+" + amount);
    }

    public void ApplyPenalty(int amount)
    {
        if (!gameActive) return;
        score = Mathf.Max(0, score - amount);
        UpdateScoreUI();
        ShowUpdatedScore("-" + amount);
    }

    private void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
    }

    private void UpdateTimerUI()
    {
        if (timerText) timerText.text = $"Time: {Mathf.CeilToInt(timer)}";
    }

    private void EndMiniGame()
    {
        scoreText.transform.parent.gameObject.SetActive(false);
        gameActive = false;
        timerText.transform.parent.gameObject.SetActive(false);

        float jumpDuration = 1;
        bubbleCam.transform.DOJump(player.playerCamera.position, 0.5f, 1, jumpDuration);
        bubbleCam.transform.DORotateQuaternion(player.playerCamera.rotation, jumpDuration);
        DOVirtual.DelayedCall(jumpDuration, () => {
            MiniGameManager.Instance.EndMiniGame();
            // gameStartTrigger.SetActive(true);
            player.EnablePlayer();
            bubbleCam.SetActive(false);
        }); 

        // Show final score screen, etc.
    }


    private void ShowUpdatedScore(string x)
    {
        updateScoreText.text = x;
        updateScoreText.GetComponent<RectTransform>().DOAnchorPosY(0, 0f);
        updateScoreText.DOFade(1, 0);
        updateScoreText.GetComponent<RectTransform>().DOAnchorPosY(150,1f);
        updateScoreText.DOFade(0, 1f);
    }
}
