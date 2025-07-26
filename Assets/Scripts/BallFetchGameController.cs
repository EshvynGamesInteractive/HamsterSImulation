using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BallFetchGameController : MonoBehaviour
{
    
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private float gameduration = 60f;
    [SerializeField] Text txtTimer;
    [SerializeField] GameObject gameStartTrigger;
    private float timer;
    private bool isGameActive;

    private void OnEnable()
    {
        if (MiniGameManager.Instance != null)
            MiniGameManager.Instance.OnMiniGameStart += OnMiniGameStarted;

        //isGameActive = false;
    }

    private void OnDisable()
    {
        if (MiniGameManager.Instance != null)
            MiniGameManager.Instance.OnMiniGameStart -= OnMiniGameStarted;
    }
    private void Start()
    {
        txtTimer.transform.parent.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (!isGameActive) return;

        timer -= Time.deltaTime;
        txtTimer.text = timer.ToString("f0");
        if (timer <= 0f)
        {
            EndMiniGame();
        }
    }

    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != MiniGameType.FetchFrenzy) return;
        isGameActive = true;
        txtTimer.transform.parent.gameObject.SetActive(true);
        timer = gameduration;
        //Debug.Log("Ball Fetch Frenzy started!");
        ballLauncher.BeginLaunching();
        //Invoke(nameof(ThrowBall), 1);
     
    }

    //public void ThrowBall()
    //{
        

    //    timer = gameDuration;
    //    isGameActive = true;
    //}


    private void EndMiniGame()
    {
        gameStartTrigger.SetActive(true);
        MainScript.instance.pnlInfo.ShowInfo("Game session ended. You can play it anytime");
        Debug.Log("Ball Fetch Frenzy ended!");
        isGameActive = false;
        txtTimer.transform.parent.gameObject.SetActive(false);
        //launcherCamera?.SetActive(false);
        ballLauncher?.StopLaunching();

        MiniGameManager.Instance.EndMiniGame();
    }
}
