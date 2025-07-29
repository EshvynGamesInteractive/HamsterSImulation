using UnityEngine;

public class WallArtGameController : MonoBehaviour
{
    [SerializeField] private GameObject pawPrintPrefab;
    [SerializeField] private GameObject gameStartTrigger;
    [SerializeField] private Transform wallSurface;
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] GameObject grandpa;
    [SerializeField] GameObject btnDraw;
    public float drawDistance = 3f;
    private Camera playerCamera;

    private float timer;
    private bool isGameActive;

    private void Start()
    {
        grandpa.SetActive(false);
        playerCamera = MainScript.instance.player.playerCamera.GetComponent<Camera>();
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

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndMiniGame();
        }
    }

    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != MiniGameType.WallArtWhirl) return;
        grandpa.SetActive(true);
        btnDraw.SetActive(true);
        isGameActive = true;
        timer = gameDuration;
    }

    private void EndMiniGame()
    {
        grandpa.SetActive(false);
        btnDraw.SetActive(false);
        isGameActive = false;
        gameStartTrigger.SetActive(true);
        MainScript.instance.pnlInfo.ShowInfo("Art session over!");
        MiniGameManager.Instance.EndMiniGame();
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
                Vector3 spawnPoint = hit.point + hit.normal * 0.01f;
                Instantiate(pawPrintPrefab, spawnPoint, Quaternion.LookRotation(hit.normal));
            }
        }
    }

}
