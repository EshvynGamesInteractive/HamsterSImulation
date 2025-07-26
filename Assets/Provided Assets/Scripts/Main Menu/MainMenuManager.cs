using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public CanvasStats currentStats;
    public CanvasStats prevStats;
    public GameObject[] CanvasState;
    public SmoothLoading loading;

    // Start is called before the first frame update
    void ChangeCanvas(CanvasStats newStats)
    {
        prevStats = currentStats;
        currentStats = newStats;
        CanvasState[(int)prevStats].SetActive(false);
        CanvasState[(int)currentStats].SetActive(true);
    }

    public void LoadScene(int i)
    {
        loading.LoadSceneSmooth(i);
    }

}

public enum CanvasStats
{
    HomePanel,
    LevelSelectionPanel,
    ExitPanel
}