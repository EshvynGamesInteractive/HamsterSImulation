using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class MiniGameTriggerZone : MonoBehaviour
{
    [SerializeField] private MiniGameType miniGameToStart;
    [SerializeField] Button gameStartBtn;

    private void Start()
    {
        gameStartBtn.onClick.AddListener(StartMiniGame); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        gameStartBtn.gameObject.SetActive(true);
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        gameStartBtn.gameObject.SetActive(false);
    }

    public void StartMiniGame()
    {
        gameStartBtn.gameObject.SetActive(false);
        MiniGameManager.Instance?.StartMiniGame(miniGameToStart);

        gameObject.SetActive(false);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}