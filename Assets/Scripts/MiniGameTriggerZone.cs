using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class MiniGameTriggerZone : MonoBehaviour
{
    [SerializeField] private MiniGameType miniGameToStart;
    private Button miniGameBtn, gameStartBtn;

    private void Start()
    {
        miniGameBtn = MiniGameManager.Instance.miniGameBtn;
        gameStartBtn = MiniGameManager.Instance.gameStartBtn;
    }

    //private void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.E))
    //    //{
    //    //    gameStartBtn.onClick.Invoke();
    //    //}
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        gameStartBtn.onClick.AddListener(StartMiniGame);
        miniGameBtn.gameObject.SetActive(true);
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        gameStartBtn.onClick.RemoveListener(StartMiniGame);
        miniGameBtn.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (gameStartBtn != null)
            gameStartBtn.onClick.RemoveListener(StartMiniGame);
    }
    public void StartMiniGame()
    {
        MainScript.instance.CloseAdPopup();
        miniGameBtn.gameObject.SetActive(false);
        MiniGameManager.Instance?.StartMiniGame(miniGameToStart);
        Debug.Log(gameObject.name);
        gameObject.SetActive(false);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}