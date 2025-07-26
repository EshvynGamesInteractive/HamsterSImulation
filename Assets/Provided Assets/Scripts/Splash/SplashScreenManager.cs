using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenManager : MonoBehaviour
{
    public CanvasStates currentStats;
    public CanvasStates prevStats;
    public GameObject[] CanvasState;
    public SmoothLoading loading;

    // Start is called before the first frame update
    void ChangeCanvas(CanvasStates newStats)
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

    // Update is called once per frame

}

public enum CanvasStates
{
    SplashScreen, 
}