using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public static MainScript instance;
    public InfoPanelScript pnlInfo;
    [SerializeField] Text txtScore;
    public PlayerScript player;
    private int scoreCount;


    private void Awake()
    {
        instance = this;
    }


    public void PointScored(int points)
    {
        scoreCount+=points;
        txtScore.text=scoreCount.ToString();
    }
}
