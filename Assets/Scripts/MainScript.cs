using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public static MainScript instance;
    public InfoPanelScript pnlInfo;
    public TaskPanelScript taskPanel;
    [SerializeField] GameObject btnRetry, btnNext, pnlPause, pnlWin, pnlLose;
    [SerializeField] Text txtScore;
    [SerializeField] int activeLevelIndex;
    [SerializeField] LevelScript[] levels;
    public PlayerScript player;
    public GrandpaAI grandPa;
    private int scoreCount;
    public LevelScript activeLevel;
    public GameObject indication;
    public bool gameover;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1;
        activeLevelIndex = GlobalValues.currentLevel - 1;

        Debug.Log(activeLevelIndex);
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].gameObject.SetActive(false);
        }
        if (activeLevelIndex >= levels.Length)
            activeLevelIndex = levels.Length - 1;
        levels[activeLevelIndex].gameObject.SetActive(true);
        activeLevel = levels[activeLevelIndex];
    }




    public void PointScored(int points)
    {
        scoreCount += points;
        txtScore.text = scoreCount.ToString();
    }


    public void SetIndicationPosition(Transform pos)
    {
        ShowIndication();
        indication.transform.position = pos.position;
    }

    public void HideIndication()
    {
        indication.SetActive(false);
    }

    public void ShowIndication()
    {
        indication.SetActive(true);
    }
    public void PlayerCaught()
    {
        MainScript.instance.pnlInfo.ShowInfo("You have been caught");
        gameover = true;
        Invoke(nameof(LevelFailed), 2);
    }

    public void AllTasksCompleted()
    {
        player.DisablePlayer();
        player.gameObject.SetActive(true);
        Invoke(nameof(LevelCompleted), 1);
    }


    private void LevelCompleted()
    {
        OpenPopup(pnlWin);
    }
    private void LevelFailed()
    {
        OpenPopup(pnlLose);
    }
    public void OpenPausePanel()
    {
        Time.timeScale = 0.001f;
        OpenPopup(pnlPause);
    }
    public void OpenPopup(GameObject pnl)
    {
        pnl.SetActive(true);
    }
    public void ClosePopup(GameObject pnl)
    {
        Time.timeScale = 1;
        pnl.SetActive(false);
    }
    public void OnBtnRetry()
    {
        Time.timeScale = 1;
        GlobalValues.sceneTOLoad = "Gameplay";
        SceneManager.LoadScene("Loading");
    }
    public void OnBtnHome()
    {
        Time.timeScale = 1;
        GlobalValues.sceneTOLoad = "MainMenu";
        SceneManager.LoadScene("Loading");
    }

    public void OnBtnResume()
    {

    }

    public void OnBtnNext()
    {
        Time.timeScale = 1;
        if (GlobalValues.currentLevel == GlobalValues.UnlockedLevels)
            GlobalValues.UnlockedLevels++;
        GlobalValues.currentLevel++;
        GlobalValues.sceneTOLoad = "Gameplay";
        SceneManager.LoadScene("Loading");
    }
}
