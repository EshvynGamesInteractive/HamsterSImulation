using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class MiniGameTriggerZone : MonoBehaviour
{
    [SerializeField] private MiniGameType miniGameToStart;
    [SerializeField] private float triggerStayDuration = 2f;
    [SerializeField] private Image imageTOFill;

    private Button miniGameBtn, gameStartBtn;
    private float stayTimer;
    private bool playerInZone;
    private Collider playerCollider;

    private void Start()
    {
        if (MiniGameManager.Instance == null) return;

        miniGameBtn = MiniGameManager.Instance.miniGameBtn;
        gameStartBtn = MiniGameManager.Instance.gameStartBtn;

        if (imageTOFill != null)
            imageTOFill.fillAmount = 0f;
    }

    private void Update()
    {
        if (playerInZone && playerCollider != null&& playerCollider.gameObject.activeInHierarchy)
        {
            stayTimer += Time.deltaTime;

            if (imageTOFill != null)
                imageTOFill.fillAmount = stayTimer / triggerStayDuration;

            if (stayTimer >= triggerStayDuration)
            {
                StartMiniGame();
                playerInZone = false;
            }
        }
        else if (playerInZone && (playerCollider == null || !playerCollider.gameObject.activeInHierarchy))
        {
            playerInZone = false;
            stayTimer = 0f;
            if (imageTOFill != null)
            {
                imageTOFill.fillAmount = 0f;
                imageTOFill.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Debug.Log("playerenter");
        if (!other.CompareTag("Player")) return;
        
        stayTimer = 0f;
        playerInZone = true;
        playerCollider = other;
        //if (miniGameBtn != null)
        //    miniGameBtn.gameObject.SetActive(true);

        if (imageTOFill != null)
        {
            imageTOFill.fillAmount = 0f;
            imageTOFill.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("playerexit");
        if (!other.CompareTag("Player")) return;

        playerInZone = false;
        stayTimer = 0f;
        playerCollider = null;
        if (miniGameBtn != null)
            miniGameBtn.gameObject.SetActive(false);

        if (imageTOFill != null)
        {
            imageTOFill.fillAmount = 0f;
            imageTOFill.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        playerInZone = false;

        if (imageTOFill != null)
        {
            imageTOFill.fillAmount = 0f;
            imageTOFill.gameObject.SetActive(false);
        }
    }

    public void StartMiniGame()
    {
        MainScript.instance?.CloseAdPopup();
       
        if (miniGameBtn != null)
            miniGameBtn.gameObject.SetActive(false);

        if (imageTOFill != null)
        {
            imageTOFill.fillAmount = 0f;
            imageTOFill.gameObject.SetActive(false);
        }

        MiniGameManager.Instance?.StartMiniGame(miniGameToStart);
        //Debug.Log(gameObject.name);
        // gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}






















//using UnityEngine;
//using UnityEngine.UI;

//[RequireComponent(typeof(Collider))]
//public class MiniGameTriggerZone : MonoBehaviour
//{
//    [SerializeField] private MiniGameType miniGameToStart;
//    private Button miniGameBtn, gameStartBtn;

//    private void Start()
//    {
//        miniGameBtn = MiniGameManager.Instance.miniGameBtn;
//        gameStartBtn = MiniGameManager.Instance.gameStartBtn;
//    }

//    //private void Update()
//    //{
//    //    //if (Input.GetKeyDown(KeyCode.E))
//    //    //{
//    //    //    gameStartBtn.onClick.Invoke();
//    //    //}
//    //}

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        gameStartBtn.onClick.AddListener(StartMiniGame);
//        miniGameBtn.gameObject.SetActive(true);

//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        gameStartBtn.onClick.RemoveListener(StartMiniGame);
//        miniGameBtn.gameObject.SetActive(false);
//    }

//    private void OnDisable()
//    {
//        if (gameStartBtn != null)
//            gameStartBtn.onClick.RemoveListener(StartMiniGame);
//    }
//    public void StartMiniGame()
//    {
//        MainScript.instance.CloseAdPopup();
//        miniGameBtn.gameObject.SetActive(false);
//        MiniGameManager.Instance?.StartMiniGame(miniGameToStart);
//        Debug.Log(gameObject.name);
//        gameObject.SetActive(false);
//    }


//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.green;
//        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
//    }
//}