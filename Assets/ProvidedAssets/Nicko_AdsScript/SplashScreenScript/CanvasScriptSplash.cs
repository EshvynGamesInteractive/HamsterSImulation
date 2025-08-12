using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK.Setup;
using UnityEngine;

public class CanvasScriptSplash : MonoBehaviour
{
    public static CanvasScriptSplash instance;
    public CanvasStats currentStats;
    public CanvasStats prevState;
    public GameObject[] canvasState;
    public SmoothLoading loading;
    public void ChangeCanvas(CanvasStats newStats)
    {
        prevState = currentStats;
        currentStats = newStats;
        canvasState[(int)prevState].SetActive(false);
        canvasState[(int)currentStats].SetActive(true);
        
    }
    public void LoadScene(string i)
    {
        loading.StartLoading(i);
    }
    public void LoadScene(int i)
    {
        loading.StartLoading(i);
    }
    public void EmptyLoading(Action callBack)
    {
        loading.EmptyLoading(callBack);
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;

      
        
    }
}

public enum CanvasStats
{
    InternetNotAvailable,
    MainScreen,
    PrivacyPolicy,
    Loading
}