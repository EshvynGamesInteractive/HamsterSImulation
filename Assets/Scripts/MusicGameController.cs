using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MusicGameController : MonoBehaviour
{
    [SerializeField] private MiniGameType miniGameType;
    [SerializeField] private GameObject gameStartTrigger;
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] Transform playerStandPos;
    [SerializeField] Transform pianoCamera;
    [SerializeField] private ParticleSystem musicParticle;

    [SerializeField] private Collider[] itemsTOEnable;

    private Camera playerCamera;
    private PlayerScript player;
    public Text timerText;
    private float timer;
    private bool isGameActive;

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

    public void ButtonPressed(Transform button)
    {
        GameObject particle = Instantiate(musicParticle.gameObject, musicParticle.transform.parent);
        particle.transform.position = button.position;
        particle.GetComponent<ParticleSystem>().Play();
        Destroy(particle.gameObject, 2);
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


    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != miniGameType) return;

        foreach (Collider btn in itemsTOEnable)
        {
            btn.enabled = true;
        }
        
        MainScript.instance.grandPa.StopTheChase();
        MainScript.instance.grandPa.StartPatrolOnGroundFloor();
        timerText.transform.parent.gameObject.SetActive(true);
        if (miniGameType == MiniGameType.Piano)
        {
            MainScript.instance.taskPanel.UpdateTask("Play the piano and create your tune!");

            MainScript.instance.pnlInfo.ShowInfo("Tap the piano keys to make sounds.");
        }
        else
        {
            MainScript.instance.taskPanel.UpdateTask("Play with the drums and create your own beat");

            MainScript.instance.pnlInfo.ShowInfo("Tap on the drums to play sounds");
            
        }

        isGameActive = true;
        timer = gameDuration;


        pianoCamera.gameObject.SetActive(true);
        pianoCamera.position = playerCamera.transform.position;
        pianoCamera.rotation = playerCamera.transform.rotation;

        pianoCamera.DOLocalMove(Vector3.zero, 0.5f);
        pianoCamera.DOLocalRotate(Vector3.zero, 0.5f);

        player.transform.SetPositionAndRotation(playerStandPos.position, playerStandPos.rotation);
        player.DisablePlayer();
    }

    private void EndMiniGame()
    {
        
        foreach (Collider btn in itemsTOEnable)
        {
            btn.enabled = false;
        }
        timerText.transform.parent.gameObject.SetActive(false);
        isGameActive = false;
        if (!MainScript.instance.gameover)
        {
            GrandpaAI grandpaAI = MainScript.instance.grandPa;
            grandpaAI.StartPatrolOnGroundFloor();
           
            MiniGameManager.Instance.EndMiniGame();
            
        }

        pianoCamera.DOMove(playerCamera.transform.position, 0.5f);
        pianoCamera.DORotate(playerCamera.transform.eulerAngles, 0.5f).OnComplete(() =>
        {
            pianoCamera.gameObject.SetActive(false);
            player.EnablePlayer();
        });
    }
}