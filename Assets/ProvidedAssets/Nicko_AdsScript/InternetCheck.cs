using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InternetCheck : MonoBehaviour
{
    public Button tryAgain;

    public int PrivacyPolicy
    {
        get => PlayerPrefs.GetInt("PrivacyPolicy", 0);
        set => PlayerPrefs.SetInt("PrivacyPolicy", value);
    }

    public bool ISNETWORKREACHABLE()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    private void Awake()
    {
      
    }

    public void Initlize()
    {
        Application.targetFrameRate = 90;
        remoteConfig.InternetAvailable();
        if (PrivacyPolicy == 0)
        {
            CanvasScriptSplash.instance.EmptyLoading(() =>
            {
                DOVirtual.DelayedCall(1,
                    () => { CanvasScriptSplash.instance.ChangeCanvas(CanvasStats.PrivacyPolicy); });
            });
        }
        else if (PrivacyPolicy == 1)
        {
            Init();
            CanvasScriptSplash.instance.LoadScene(1);
        }
        isInitialize = true;
    }

    void Start()
    {
        tryAgain?.onClick.AddListener(HidePanel);
        if (ISNETWORKREACHABLE())
        {
            Initlize();
        }
        else
        {
            CheckInternet();
        }
    }

    public void Init()
    {
        StartCoroutine(CheckInternetRoutine());
    }

    IEnumerator CheckInternetRoutine()
    {
        while (true)
        {
            CheckInternet();
            yield return new WaitForSeconds(5f); // Internet check har 5 second baad
        }
    }

    public void CheckInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowPanel();
         //   return;
        }

        // if (!isInitialize)
        // {
        //     isInitialize = true;
        //     remoteConfig.InternetAvailable();
        // }
        /* else if (Application.internetReachability != NetworkReachability.NotReachable)
         {
             HidePanel();
         }*/
    }

    public bool isInitialize;
    CanvasStats cStats;

    public void ShowPanel()
    {
        Time.timeScale = 0;
        cStats = CanvasScriptSplash.instance.currentStats;
        CanvasScriptSplash.instance.ChangeCanvas(CanvasStats.InternetNotAvailable);
    }

    public FirebaseRemoteConfig remoteConfig;

    public void HidePanel()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            CanvasScriptSplash.instance.ChangeCanvas(cStats);
            Time.timeScale = 1;
            
            if (!isInitialize)
            {
                Initlize();
            } 
        }
    }
}