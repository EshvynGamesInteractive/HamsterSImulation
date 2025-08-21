using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public static MainScript instance;
    [SerializeField] private bool isTesting = false;
    [SerializeField] int activeLevelIndex;
    [SerializeField] Sprite soundOn, soundOff;
    [SerializeField] Image btnSound;
    [SerializeField] private Text txtLevel;
    [SerializeField] private Image levelFillBar;
    public InfoPanelScript pnlInfo;
    public TaskPanelScript taskPanel;

    [SerializeField] GameObject btnRetry, btnNext, pnlPause, pnlWin, pnlLose, pnlAd;

    //[SerializeField] Text txtScore;
    [SerializeField] LevelScript[] levels;
    [SerializeField] PanelSpawner timerAd;
    public PlayerScript player;
    public GrandpaAI grandPa;
    private int scoreCount;
    [HideInInspector] public LevelScript activeLevel;
    [HideInInspector]public bool gameover;
    public GameObject indication;
    public NavmeshPathDraw pathDraw;
    public static int currentTaskNumber;
    public static int decrementedNumber;
    public bool canShowRewardedPopup = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (isTesting)
            GlobalValues.currentLevel = activeLevelIndex + 1; //forTesting
        txtLevel.text ="Level "+ GlobalValues.currentLevel;
        
        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.HideRecBanner();
            Nicko_ADSManager._Instance.ShowBanner("GameStart");
        }

        Time.timeScale = 1;
        if (GlobalValues.currentLevel > levels.Length)
            GlobalValues.currentLevel = levels.Length;
        if (GlobalValues.retryAfterLevelCompleted && GlobalValues.currentLevel > 1)
        {
            GlobalValues.currentLevel--;
        }

        GlobalValues.retryAfterLevelCompleted = false;

        activeLevelIndex = GlobalValues.currentLevel - 1;

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].gameObject.SetActive(false);
        }

        if (activeLevelIndex >= levels.Length)
            activeLevelIndex = levels.Length - 1;


        levels[activeLevelIndex].gameObject.SetActive(true);
        activeLevel = levels[activeLevelIndex];
        CheckSound();
    }

    private void CheckSound()
    {
        if (GlobalValues.Effects == 1)
        {
            btnSound.sprite = soundOn;
            SoundManager.instance.SoundOn();
        }
        else
        {
            btnSound.sprite = soundOff;
            SoundManager.instance.SoundOff();
        }
    }

    public void OnBtnSound()
    {
        if (GlobalValues.Effects == 1)
        {
            btnSound.sprite = soundOff;
            GlobalValues.Effects = 0;
            GlobalValues.Music = 0;
            SoundManager.instance.SoundOff();
        }
        else
        {
            btnSound.sprite = soundOn;
            GlobalValues.Effects = 1;
            GlobalValues.Music = 1;
            SoundManager.instance.SoundOn();
        }
    }

    public void RestartRewardedTimer()
    {
        timerAd.StartPanelTimer();
    }

    public void OnBtnClick()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonClick);
    }

    //public void PointScored(int points)
    //{
    //    scoreCount += points;
    //    txtScore.text = scoreCount.ToString();
    //}


    public void SetIndicationPosition(Transform pos)
    {
        ShowIndication();
        indication.transform.position = pos.position;
        if (pos.TryGetComponent<Interactable>(out Interactable interactable))
        {
            if (interactable.indicationPoint != null)
            {
                pathDraw.SetDestination(interactable.indicationPoint);
                return;
            }
        }

        pathDraw.SetDestination(pos);
    }

    public void HideIndication()
    {
        pathDraw.Stop();
        if (indication != null)
            indication.SetActive(false);
    }

    public void ShowIndication()
    {
        if (indication != null)
            indication.SetActive(true);
    }

    public void PlayerCaught(bool caughtWhileMinigame)
    {
        if (gameover)
            return;
        if (currentTaskNumber > 0 && !caughtWhileMinigame &&
            currentTaskNumber != decrementedNumber) // so it does not decrement when caught on same task
        {
            currentTaskNumber--;
            decrementedNumber = currentTaskNumber;
        }

        MainScript.instance.pnlInfo.ShowInfo("You have been caught");
        gameover = true;
        Invoke(nameof(LevelFailed), 2);
    }

    public void PlayerRevived()
    {
        gameover = false;
        ClosePopup(pnlLose);
    }

    public void TaskCompleted(int completedTasks, int totalTasks)
    {
        float taskPercentage = (float)completedTasks / totalTasks;
        levelFillBar.DOFillAmount(taskPercentage, 0.2f).SetUpdate(true);
    }
    public void AllTasksCompleted()
    {
        if (gameover)
            return;
        levelFillBar.DOFillAmount(1, 0.2f).SetUpdate(true);
        player.DisablePlayer();
        player.gameObject.SetActive(true);
        gameover = true;
        Invoke(nameof(LevelCompleted), 1);
    }


    private void LevelCompleted()
    {
        canShowRewardedPopup = false;
        if (GlobalValues.currentLevel == GlobalValues.UnlockedLevels)
            GlobalValues.UnlockedLevels++;
        GlobalValues.currentLevel++;
        SoundManager.instance.PlaySound(SoundManager.instance.levelComplete);
        OpenPopup(pnlWin);
        currentTaskNumber = 0;
        decrementedNumber = 0;
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    private void LevelFailed()
    {
        canShowRewardedPopup = false;
        SoundManager.instance.PlaySound(SoundManager.instance.levelFail);
        OpenPopup(pnlLose);
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    public void OpenPausePanel()
    {
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowInterstitial("LevelPauseAD");
        OpenPopup(pnlPause);
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    public void OpenPopup(GameObject pnl)
    {
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.HideBanner();
        canShowRewardedPopup = false;

        Time.timeScale = 0.001f;
        pnl.SetActive(true);
        float animTime = 1;
        pnl.GetComponent<Image>().DOFade(1, animTime / 2).SetUpdate(true);

        RectTransform midPanel = pnl.transform.GetChild(0).GetComponent<RectTransform>();

        midPanel.DOAnchorPosY(0, animTime).From(new Vector2(0, -200)).SetUpdate(true);
        midPanel.GetComponent<CanvasGroup>().DOFade(1, animTime).SetUpdate(true);
    }

    public void CloseAdPopup()
    {
        ClosePopup(pnlAd);
    }


    public void ClosePopup(GameObject pnl)
    {
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowBanner("Popup close");
        canShowRewardedPopup = true;
        Time.timeScale = 1;
        float animTime = 1;

        RectTransform midPanel = pnl.transform.GetChild(0).GetComponent<RectTransform>();
        midPanel.DOAnchorPosY(-200, animTime / 2).SetUpdate(true);
        midPanel.GetComponent<CanvasGroup>().DOFade(0, animTime / 2).SetUpdate(true);
        pnl.GetComponent<Image>().DOFade(0, animTime).SetUpdate(true).OnComplete(() => { pnl.SetActive(false); });
    }

    public void CloseFailPopup()
    {
        ClosePopup((pnlLose));
    }

    public void OnBtnRetry()
    {
        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.ShowInterstitial("LevelRetryONPauseAD");
            Nicko_ADSManager._Instance.RecShowBanner("LoadingStart");
        }

        Time.timeScale = 1;
        //GlobalValues.sceneTOLoad = "Gameplay";
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
    }

    public void OnBtnRetryAfterLevelCompleted()
    {
        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.ShowInterstitial("LevelRetryAfterWinAD");
            Nicko_ADSManager._Instance.RecShowBanner("LoadingStart");
        }

        GlobalValues.retryAfterLevelCompleted = true;
        Time.timeScale = 1;
        //GlobalValues.sceneTOLoad = "Gameplay";
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
    }

    public void OnBtnHome()
    {
        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.ShowInterstitial("HomeButtonAD");
            Nicko_ADSManager._Instance.RecShowBanner("LoadingStart");
        }

        currentTaskNumber = 0;
        decrementedNumber = 0;
        Time.timeScale = 1;
        //GlobalValues.sceneTOLoad = "MainMenu";
        CanvasScriptSplash.instance.LoadScene("MainMenu");
        //SceneManager.LoadScene("Loading");
    }


    public void OnBtnNext()
    {
        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.ShowInterstitial("NextButtonAD");
            Nicko_ADSManager._Instance.RecShowBanner("LoadingStart");
        }

        Time.timeScale = 1;

        //GlobalValues.sceneTOLoad = "Gameplay";
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
    }
}