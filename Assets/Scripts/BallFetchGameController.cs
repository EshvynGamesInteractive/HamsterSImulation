using DG.Tweening;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class BallFetchGameController : MonoBehaviour
{
    
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private float gameduration = 60f;
    [SerializeField] Text txtTimer, scoreText;
    [SerializeField] GameObject gameStartTrigger;
    [SerializeField] private Sprite ballGameIcon;
    private int score;
    private float timer;
    private bool gameActive;

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
        if (!gameActive) return;

        timer -= Time.deltaTime;
        txtTimer.text = timer.ToString("f0");
        if (timer <= 0f)
        {
            EndMiniGame();
        }
    }
    public void AddScore(int amount)
    {
        if (!gameActive) return;
        score += amount;
        UpdateScoreUI();
        
        GlobalValues.TotalBones += 1;

        MainScript.instance.UpdateBonesText();
    }
    private void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
    }
    private void OnMiniGameStarted(MiniGameType type)
    {
        if (type != MiniGameType.FetchFrenzy) return;
        scoreText.transform.parent.gameObject.SetActive(true);
        score = 0;
        UpdateScoreUI();
        MainScript.instance.taskPanel.UpdateTask("Fetch the balls launched and bring them back before time runs out", ballGameIcon);
        gameActive = true;
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
      
        MainScript.instance.pnlInfo.ShowInfo("Game session ended. You can play it anytime");
        Debug.Log("Ball Fetch Frenzy ended!");
        gameActive = false;
        txtTimer.transform.parent.gameObject.SetActive(false);
        //launcherCamera?.SetActive(false);
        ballLauncher?.StopLaunching();
        scoreText.transform.parent.gameObject.SetActive(false);
        MiniGameManager.Instance.EndMiniGame();
    }
}
