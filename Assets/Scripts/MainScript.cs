using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SickscoreGames.HUDNavigationSystem;

public class MainScript : MonoBehaviour
{
    public static MainScript instance;
    public TutorialScript tutorial;
    [SerializeField] private GameObject shopBtn;
    [SerializeField] private Text txtBonesReward;
    [SerializeField] private HUDNavigationElement navElement;
    [SerializeField] private Sprite indicationDefaultIcon;
    [SerializeField] private bool isTesting = false;
    [SerializeField] int activeLevelIndex;
    [SerializeField] Sprite soundOn, soundOff;
    [SerializeField] Image btnSound;
    [SerializeField] private Text txtLevel, txtBones;
    [SerializeField] private Image levelFillBar;
    [SerializeField] private GameObject btnNextStage;
    public InfoPanelScript pnlInfo;
    public TaskPanelScript taskPanel;

    [SerializeField] GameObject btnRetry, btnNext, pnlPause, pnlStageWin, pnlLevelWin, pnlLose, pnlAd;

    //[SerializeField] Text txtScore;
    [SerializeField] LevelScript[] levels;
    [SerializeField] PanelSpawner timerAd;
    public PlayerScript player;
    public GrandpaAI grandPa;
    private int scoreCount;
    [HideInInspector] public LevelScript activeLevel;
    [HideInInspector] public bool gameover;
    public GameObject indication;

    public NavmeshPathDraw pathDraw;

    // public static int currentTaskNumber;
    private static int decrementedNumber;
    public bool canShowRewardedPopup = true;


    // public int currentTaskNumber
    // {
    //
    //     get { return activeLevel.GetCurrentStageCompletedTaskNumber(); }
    //     set { activeLevel.SetCurrentStageTaskNumber(value); }
    // }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (isTesting)
            GlobalValues.currentStage = activeLevelIndex + 1; //forTesting

        if (Nicko_ADSManager.instance)
        {
            Nicko_ADSManager.instance.HideRecBanner();
            Nicko_ADSManager.instance.ShowBanner("GameStart");

            if (!GlobalValues.canShowInterstitialAtSceneStart)
            {
                GlobalValues.canShowInterstitialAtSceneStart = true;
            }
            Nicko_ADSManager.instance.ShowInterstitial("GameplayStart");
        }

        CheckSound();
        Time.timeScale = 1;
        if (GlobalValues.TutorialPlayed == 0 && !isTesting)
        {
            canShowRewardedPopup = false;
            tutorial.gameObject.SetActive(true);
            shopBtn.SetActive(false);
        }
        else
        {
            shopBtn.SetActive(true);
            canShowRewardedPopup = true;
            if (GlobalValues.currentStage > levels.Length)
                GlobalValues.currentStage = levels.Length;
            if (GlobalValues.retryAfterLevelCompleted && GlobalValues.currentStage > 1)
            {
                GlobalValues.currentStage--;
            }

            GlobalValues.retryAfterLevelCompleted = false;

            activeLevelIndex = GlobalValues.currentStage - 1;

            for (int i = 0; i < levels.Length; i++)
            {
                levels[i].gameObject.SetActive(false);
            }

            if (activeLevelIndex >= levels.Length)
                activeLevelIndex = levels.Length - 1;

// Debug.Log(activeLevelIndex);
// Debug.Log(levels.Length);
            levels[activeLevelIndex].gameObject.SetActive(true);
            activeLevel = levels[activeLevelIndex];
        }

        UpdateBonesText();
    }

    public void UpdateLevelText(int levelNumber)
    {
        txtLevel.text = "Chapter " + levelNumber;
    }

    public void UpdateBonesText()
    {
        txtBones.text = "" + GlobalValues.TotalBones;
    }

    public void OpenDogSelection()
    {
        if (Nicko_ADSManager.instance)
            Nicko_ADSManager.instance.ShowInterstitial("DogSelectionOpen");
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
        if (Nicko_ADSManager.instance)
        {
            Nicko_ADSManager.instance.HideRecBanner();
        }

        SoundManager.instance.PlaySound(SoundManager.instance.buttonClick);
    }

    //public void PointScored(int points)
    //{
    //    scoreCount += points;
    //    txtScore.text = scoreCount.ToString();
    //}


    public void SetIndicationPosition(Transform pos)
    {
        Debug.Log("first");
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
        ChangeIndicationIcons(indicationDefaultIcon);
    }

    public void SetIndicationPosition(Transform pos, Sprite indicationIcon)
    {
        
        ShowIndication();
        indication.transform.position = pos.position;
        ChangeIndicationIcons(indicationIcon);
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

    private HNSIndicatorPrefab indicator;

    private void ChangeIndicationIcons(Sprite newIcon)
    {
        DOVirtual.DelayedCall(0.01f, () =>
        {
            if (indicator == null)
                indicator = FindFirstObjectByType<HNSIndicatorPrefab>();
           
           
            if (indicator != null)
            {
                indicator.ChangeIcon(newIcon);
            }
        });
    }

    public void HideIndication()
    {
        Debug.Log("HideIndication"  );
        if (pathDraw != null)
        {
            pathDraw.Stop();
        }

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

        Debug.Log(activeLevel.GetCurrentLevelCompletedTaskNumber());
        if (activeLevel.GetCurrentStageCompletedTaskNumber() > 0 && !caughtWhileMinigame &&
            activeLevel.GetCurrentStageCompletedTaskNumber() != decrementedNumber
            && activeLevel.GetCurrentLevelCompletedTaskNumber() !=
            0) // so it does not decrement when caught on same task
        {
            activeLevel.SetCurrentLevelCompletedTaskNumber(activeLevel.GetCurrentLevelCompletedTaskNumber() - 1);
            activeLevel.SetCurrentStageTaskNumber(activeLevel.GetCurrentStageCompletedTaskNumber() - 1);
            decrementedNumber = activeLevel.GetCurrentStageCompletedTaskNumber();
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
        // Debug.Log(taskPercentage);
        levelFillBar.DOFillAmount(taskPercentage, 0.2f).SetUpdate(true);
    }

    public void OnBtn2xWatchAd()
    {
        if (Nicko_ADSManager.instance)
            Nicko_ADSManager.instance.ShowRewardedAd(() => Add2xCoins(), "RewardedReviveAd");
        else
            Add2xCoins();
    }

    private void Add2xCoins()
    {
        OnBtnNextLevel();
        GlobalValues.TotalBones += activeLevel.levelCompleteReward;

        UpdateBonesText();
    }

    public void CurrentLevelTasksCompleted()
    {
        if (gameover)
            return;

        GlobalValues.TotalBones += activeLevel.levelCompleteReward;
        txtBonesReward.text = "" + activeLevel.levelCompleteReward;
        UpdateBonesText();


        activeLevel.SetCurrentLevelCompletedTaskNumber(0);
        Debug.Log("currenttasks");
        grandPa.StopTheChase();
        levelFillBar.DOFillAmount(1, 0.2f).SetUpdate(true);

        gameover = true;
        Invoke(nameof(LevelCompleted), 2);
    }

    private void LevelCompleted()
    {
        canShowRewardedPopup = false;
        player.DisablePlayer();
        player.gameObject.SetActive(true);
        grandPa.StopTheChase();
        SoundManager.instance.PlaySound(SoundManager.instance.levelComplete);
        OpenPopup(pnlLevelWin);
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    public void AllTasksCompleted()
    {
        Debug.Log("allTasksCompleted " + gameover);
        if (gameover)
            return;


        activeLevel.SetCurrentLevelCompletedTaskNumber(0);
        activeLevel.SetCurrentStageUnlockedLevels(1);
        activeLevel.SetCurrentStageTaskNumber(0);
        decrementedNumber = 0;
        Debug.Log("ALltasks");
        grandPa.StopTheChase();
        levelFillBar.DOFillAmount(1, 0.2f).SetUpdate(true);
        player.DisablePlayer();
        player.gameObject.SetActive(true);
        gameover = true;
        Invoke(nameof(StageCompleted), 1);
    }


    private void StageCompleted()
    {
        canShowRewardedPopup = false;

        if (GlobalValues.UnlockedLevels >= levels.Length)
            btnNextStage.SetActive(false);
        else
        {
            btnNextStage.SetActive(true);
        }

        if (GlobalValues.currentStage == GlobalValues.UnlockedStages)
            GlobalValues.UnlockedStages++;
        GlobalValues.currentStage++;
        SoundManager.instance.PlaySound(SoundManager.instance.levelComplete);


        OpenPopup(pnlStageWin);

        // decrementedNumber = 0;
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
        if (Nicko_ADSManager.instance)
            Nicko_ADSManager.instance.ShowInterstitial("LevelPauseAD");
        OpenPopup(pnlPause);
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    public void OpenPopup(GameObject pnl)
    {
        if (Nicko_ADSManager.instance)
        {
            Nicko_ADSManager.instance.HideRecBanner();
            Nicko_ADSManager.instance.HideBanner();
        }

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
        player.GetComponent<FP_Controller>().OnEnable();

        if (Nicko_ADSManager.instance)
            Nicko_ADSManager.instance.ShowBanner("Popup close");
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

    public void OnBtnResume()
    {
        if (Nicko_ADSManager.instance)
            Nicko_ADSManager.instance.ShowInterstitial("ResumeButtonAd");
    }
    public void OnBtnRetry()
    {
        timerAd.StartPanelTimer();
        if (Nicko_ADSManager.instance)
        {
            Nicko_ADSManager.instance.ShowInterstitial("LevelRetryONPauseAD");
            Nicko_ADSManager.instance.RecShowBanner("OnBtnRetry");
        }
        grandPa.gameObject.SetActive(false);
        Time.timeScale = 1;
        gameover = true;
        
        //GlobalValues.sceneTOLoad = "Gameplay";
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
    }

    public void OnBtnRetryAfterLevelCompleted()
    {
        timerAd.StartPanelTimer();
        if (Nicko_ADSManager.instance)
        {
             Nicko_ADSManager.instance.ShowInterstitial("LevelRetryAfterWinAD");
            Nicko_ADSManager.instance.RecShowBanner("OnBtnRetry2");
        }

        GlobalValues.retryAfterLevelCompleted = true;
        
        grandPa.gameObject.SetActive(false);
        Time.timeScale = 1;
        gameover = true;
        
        //GlobalValues.sceneTOLoad = "Gameplay";
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
    }

    public void OnBtnHome()
    {
        timerAd.StartPanelTimer();
        if (Nicko_ADSManager.instance)
        {
             Nicko_ADSManager.instance.ShowInterstitial("HomeButtonAD");
            Nicko_ADSManager.instance.RecShowBanner("OnBtnHome");
        }

        // decrementedNumber = 0;
        grandPa.gameObject.SetActive(false);
        Time.timeScale = 1;
        gameover = true;
        
        //GlobalValues.sceneTOLoad = "MainMenu";
        CanvasScriptSplash.instance.LoadScene("MainMenu");
        //SceneManager.LoadScene("Loading");
    }


    public void OnBtnNext()
    {
        timerAd.StartPanelTimer();
        if (Nicko_ADSManager.instance)
        {
             Nicko_ADSManager.instance.ShowInterstitial("NextStageButtonAD");
            Nicko_ADSManager.instance.RecShowBanner("OnBtnNext");
        }
        grandPa.gameObject.SetActive(false);
        Time.timeScale = 1;
        gameover = true;

        //GlobalValues.sceneTOLoad = "Gameplay";
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
    }


    public void OnBtnNextLevel()
    {
        timerAd.StartPanelTimer();
        if (Nicko_ADSManager.instance)
        {
            Nicko_ADSManager.instance.ShowInterstitial("NextLevelButtonAD");
        }

        activeLevel.StartNextLevel();
        gameover = false;
        ClosePopup(pnlLevelWin);
        player.EnablePlayer();
        levelFillBar.DOFillAmount(0, 0f).SetUpdate(true);
    }
}