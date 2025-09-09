using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WallArtGameController : MonoBehaviour
{
    [SerializeField] GameObject pnlWallColor;
    [SerializeField] private GameObject pawPrintPrefab;
    [SerializeField] private Transform wallSurface;
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] BoxCollider wallCanvas;
    [SerializeField] GameObject btnDraw;
    [SerializeField] Transform playerStartPos;
    [SerializeField] Transform grandpaPosWithWall;
    [SerializeField] GameObject stareWallCutscene;
    [SerializeField] Collider wallToColorCollider;
    [SerializeField] float cutsceneDuration;
    [SerializeField] Transform wallCamera;
    [SerializeField] private Sprite wallArtIcon;
    public float drawDistance = 3f;
    private Camera playerCamera;
    private PlayerScript player;
    public Text timerText;
    private float timer;
    private bool isGameActive;

    private void Start()
    {

        wallToColorCollider.enabled = false;
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

        //#if UNITY_EDITOR || UNITY_STANDALONE
        //        if (Input.GetMouseButtonDown(0))
        //            TryDrawAt(Input.mousePosition);
        //#elif UNITY_ANDROID || UNITY_IOS
        //        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        //            TryDrawAt(Input.GetTouch(0).position);
        //#endif

        //CheckWallDistance();

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
    private void CheckWallDistance()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, drawDistance))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                btnDraw.SetActive(true);
            }
            else
            {
                btnDraw.SetActive(false);
            }
        }
        else
        {
            btnDraw.SetActive(false);
        }
    }

    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != MiniGameType.WallArtWhirl) return;

        pnlWallColor.SetActive(true);
        if (!wallToColorCollider.gameObject.activeSelf)
            wallToColorCollider.gameObject.SetActive(true);
        wallToColorCollider.enabled = true;
        timerText.transform.parent.gameObject.SetActive(true);
        MainScript.instance.taskPanel.UpdateTask("Smudge the clean wall with your muddy paws. But beware, Grandpa checks in often", wallArtIcon);

        MainScript.instance.pnlInfo.ShowInfo("Make this wall your canvas");
        //wallCanvas.enabled = true;
        //if (!MainScript.instance.grandPa.isSitting)
        //    MainScript.instance.grandPa.StartPatrolOnFirstFloor();

        isGameActive = true;
        timer = gameDuration;


        wallCamera.gameObject.SetActive(true);
        wallCamera.position = playerCamera.transform.position;
        wallCamera.rotation = playerCamera.transform.rotation;

        wallCamera.DOLocalMove(Vector3.zero, 0.5f);
        wallCamera.DOLocalRotate(Vector3.zero, 0.5f);

        player.transform.SetPositionAndRotation(playerStartPos.position, playerStartPos.rotation);
        player.DisablePlayer();

        //player.ShowAndHideDog(2);
    }

    private void EndMiniGame()
    {
        pnlWallColor.SetActive(false);
        wallToColorCollider.enabled = false;
        timerText.transform.parent.gameObject.SetActive(false);
        wallCanvas.enabled = false;
        isGameActive = false;
        if (!MainScript.instance.gameover)
        {
            MainScript.instance.grandPa.StartPatrolOnGroundFloor();
            GrandpaAI grandpaAI = MainScript.instance.grandPa;
            //grandpaAI.gameObject.SetActive(false);
            stareWallCutscene.SetActive(true);
            if (!grandpaAI.isSitting)
                grandpaAI.transform.SetPositionAndRotation(grandpaPosWithWall.position, grandpaPosWithWall.rotation);

            Typewriter.instance.StartTyping("What in the world, is that a paw-painting?! Dog! This wall was clean yesterday!", 2);
            DOVirtual.DelayedCall(cutsceneDuration, () =>
            {
                MiniGameManager.Instance.EndMiniGame();
                stareWallCutscene.SetActive(false);
                player.EnablePlayer();
                if (!grandpaAI.isSitting)
                {
                    DOVirtual.DelayedCall(4, () =>
                {
                    grandpaAI.ChasePlayerForDuration(2);
                });
                }
            });

            //gameStartTrigger.SetActive(true);
            //MainScript.instance.pnlInfo.ShowInfo("Art session over!");
            //MiniGameManager.Instance.EndMiniGame();
        }

        wallCamera.DOMove(playerCamera.transform.position, 0.5f);
        wallCamera.DORotate(playerCamera.transform.eulerAngles, 0.5f).OnComplete(() =>
        {
            wallCamera.gameObject.SetActive(false);
            //player.EnablePlayer();
        });

        btnDraw.SetActive(false);
    }

    private void TryDrawAt(Vector2 screenPosition)
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, drawDistance))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                //Vector3 spawnPoint = hit.point;
                //Instantiate(pawPrintPrefab, spawnPoint, Quaternion.LookRotation(hit.normal));

                Vector3 spawnPoint = hit.point + hit.normal * 0.01f;
                Instantiate(pawPrintPrefab, spawnPoint, Quaternion.LookRotation(hit.normal));

            }
        }

    }

    public void TryDrawAtCenter()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, drawDistance))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                player.AnimatePawToInteract();
                DOVirtual.DelayedCall(0.3f, () =>
                {
                    SoundManager.instance.PlaySound(SoundManager.instance.dogPawPrint);
                    Vector3 spawnPoint = hit.point + hit.normal * 0.01f;
                    GameObject paw = Instantiate(pawPrintPrefab, spawnPoint, Quaternion.LookRotation(hit.normal));
                    paw.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    paw.transform.DOScale(Vector3.one, 0.2f);
                });

            }
        }
    }

}
