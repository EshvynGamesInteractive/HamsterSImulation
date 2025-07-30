using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public static MainScript instance;
    public InfoPanelScript pnlInfo;
    [SerializeField] Text txtScore;
    [SerializeField] int activeLevelIndex;
    [SerializeField] LevelScript[] levels;
    public PlayerScript player;
    private int scoreCount;
    public LevelScript activeLevel;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //levels[activeLevelIndex].gameObject.SetActive(true);
        //activeLevel = levels[activeLevelIndex];
    }


    public void PointScored(int points)
    {
        scoreCount+=points;
        txtScore.text=scoreCount.ToString();
    }
}
