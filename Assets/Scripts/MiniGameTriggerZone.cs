using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class MiniGameTriggerZone : MonoBehaviour
{
    [SerializeField] private MiniGameType miniGameToStart;
    private Button gameStartBtn;

    private void Start()
    {
        gameStartBtn = MiniGameManager.Instance.gameStartBtn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(gameStartBtn);
            gameStartBtn.onClick.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        gameStartBtn.onClick.AddListener(StartMiniGame);
        gameStartBtn.gameObject.SetActive(true);
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        gameStartBtn.onClick.RemoveListener(StartMiniGame);
        gameStartBtn.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        gameStartBtn.onClick.RemoveListener(StartMiniGame);
    }
    public void StartMiniGame()
    {
        gameStartBtn.gameObject.SetActive(false);
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