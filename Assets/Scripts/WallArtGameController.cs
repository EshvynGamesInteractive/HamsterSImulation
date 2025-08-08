using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WallArtGameController : MonoBehaviour
{
    [SerializeField] private GameObject pawPrintPrefab;
    [SerializeField] private GameObject gameStartTrigger;
    [SerializeField] private Transform wallSurface;
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] BoxCollider wallCanvas;
    [SerializeField] GameObject btnDraw;
    [SerializeField] Transform playerStartPos;
    [SerializeField] Transform grandpaPosWithWall;
    [SerializeField] GameObject stareWallCutscene;
    [SerializeField] float cutsceneDuration;
    public float drawDistance = 3f;
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

        CheckWallDistance();

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
        timerText.transform.parent.gameObject.SetActive(true);
        MainScript.instance.taskPanel.UpdateTask("Smudge the clean wall with your muddy paws. But beware, Grandpa checks in often");

        MainScript.instance.pnlInfo.ShowInfo("Make this wall your canvas");
        wallCanvas.enabled = true;
        MainScript.instance.grandPa.StartPatrolOnFirstFloor();

        isGameActive = true;
        timer = gameDuration;

        player.transform.SetPositionAndRotation(playerStartPos.position, playerStartPos.rotation);

        player.ShowAndHideDog(2);
    }

    private void EndMiniGame()
    {
        timerText.transform.parent.gameObject.SetActive(false);
        wallCanvas.enabled = false;
        isGameActive = false;
        if (!MainScript.instance.gameover)
        {
            MainScript.instance.grandPa.StartPatrolOnGroundFloor();
            GrandpaAI grandpaAI = MainScript.instance.grandPa;
            //grandpaAI.gameObject.SetActive(false);
            stareWallCutscene.SetActive(true);
            grandpaAI.transform.SetPositionAndRotation(grandpaPosWithWall.position, grandpaPosWithWall.rotation);


            DOVirtual.DelayedCall(cutsceneDuration, () =>
            {
                stareWallCutscene.SetActive(false);


                DOVirtual.DelayedCall(4, () => { grandpaAI.ChasePlayerForDuration(30); });
            });

            //gameStartTrigger.SetActive(true);
            //MainScript.instance.pnlInfo.ShowInfo("Art session over!");
            MiniGameManager.Instance.EndMiniGame();
        }
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
                player.AnimatePawToCenter();
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
