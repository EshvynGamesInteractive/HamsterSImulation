using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public static MainScript instance;
    public InfoPanelScript pnlInfo;
    public TaskPanelScript taskPanel;
    [SerializeField] GameObject btnRetry, btnNext;
    [SerializeField] Text txtScore;
    [SerializeField] int activeLevelIndex;
    [SerializeField] LevelScript[] levels;
    public PlayerScript player;
    public GrandpaAI grandPa;
    private int scoreCount;
    public LevelScript activeLevel;
    public GameObject indication;
    public bool gameover;

    public static int currentLevel=1;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //activeLevelIndex = currentLevel - 1;//for deployement


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
        scoreCount+=points;
        txtScore.text=scoreCount.ToString();
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
        btnNext.SetActive(true);
    }
    private void LevelFailed()
    {
        btnRetry.SetActive(true);
    }

    public void OnBtnRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnBtnNext()
    {
        currentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
