using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public static MainScript instance;
    public InfoPanelScript pnlInfo;
    public TaskPanelScript taskPanel;

    [SerializeField] GameObject btnRetry, btnNext, pnlPause, pnlWin, pnlLose, pnlAd;

    //[SerializeField] Text txtScore;
    [SerializeField] int activeLevelIndex;
    [SerializeField] LevelScript[] levels;
    [SerializeField] PanelSpawner timerAd;
    public PlayerScript player;
    public GrandpaAI grandPa;
    private int scoreCount;
    [HideInInspector] public LevelScript activeLevel;
    public GameObject indication;
    public bool gameover;
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
        //GlobalValues.currentLevel = activeLevelIndex + 1; //forTesting


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

    public void PlayerCaught()
    {
        if (gameover)
            return;
        if (currentTaskNumber > 0 &&
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

    public void AllTasksCompleted()
    {
        if (gameover)
            return;
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
        canShowRewardedPopup = true;
        Time.timeScale = 1;
        float animTime = 1;

        RectTransform midPanel = pnl.transform.GetChild(0).GetComponent<RectTransform>();
        midPanel.DOAnchorPosY(-200, animTime / 2).SetUpdate(true);
        midPanel.GetComponent<CanvasGroup>().DOFade(0, animTime / 2).SetUpdate(true);
        pnl.GetComponent<Image>().DOFade(0, animTime).SetUpdate(true).OnComplete(() => { pnl.SetActive(false); });
    }

    public void OnBtnRetry()
    {
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowInterstitial("LevelRetryONPauseAD");
        Time.timeScale = 1;
        //GlobalValues.sceneTOLoad = "Gameplay";
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
    }

    public void OnBtnRetryAfterLevelCompleted()
    {
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowInterstitial("LevelRetryAfterWinAD");
        GlobalValues.retryAfterLevelCompleted = true;
        Time.timeScale = 1;
        //GlobalValues.sceneTOLoad = "Gameplay";
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
    }

    public void OnBtnHome()
    {
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowInterstitial("HomeButtonAD");
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
            Nicko_ADSManager._Instance.ShowInterstitial("NextButtonAD");
        Time.timeScale = 1;

        //GlobalValues.sceneTOLoad = "Gameplay";
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
    }
}