using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using GoogleMobileAds.Api;
using ToastPlugin;
using UnityEngine;

public class AppLovinMax : MonoBehaviour
{
    public static bool isAdsRemove
    {
        get => PlayerPrefs.GetInt("RemoveAds", 0) == 1;
        set => PlayerPrefs.SetInt("RemoveAds", value ? 1 : 0);
    }

    public void OnRemoveAds()
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        Nicko_ADSManager._Instance.HideBanner();
    }


    [Header("Max IDS")] public string MaxSdkKey = "";
    public string InterstitialAdUnitId = "0bf5dd259a7babe3";
    public string RewardedAdUnitId = "5d75002bbc4126b9";


    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;
    public static AppLovinMax Instance;
    public bool isTestAd;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        MaxSdkKey = "D9Ut-HfJQ-2vKKsAQDCqFcckAiSRTAbDHADl_6Q90aqL2rqEkbWc3HxBNi-ZJWCg1hvZxTrbBVvSsMGHW8NLDG";
    }

    public void Init()
    {
        Debug.Log("Application.version " + Application.version);
        DontDestroyOnLoad(this.gameObject);
        if (!GlobalConstant.ISMAXON)
            return;
        MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();
        PostInit();
    }

    void PostInit()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            if (!GlobalConstant.AdsON)
            {
                return;
            }

            InitializeInterstitialAds();
            InitializeRewardedAds();
        };
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
    }

    #region Interstitial Ad Methods

    public void InitializeInterstitialAds()
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent +=
            Nicko_AnalyticalManager.instance.Revenue_ReportMax;

        // Load the first interstitial
        LoadInterstitial();
    }

    public void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
    }

    public void ShowInterstitial(bool checkAdmob=true)
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        if (isAdsRemove)
        {
            return;
        }

        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        {
            Nicko_Admob.isInterstialAdPresent = true;
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
            Nicko_AnalyticalManager.instance.InterstitialEvent("Max_Inter_Shown");
        }
        else if(checkAdmob)
        {
            Nicko_ADSManager._Instance.admobInstance.ShowInterstitial(false);
            Nicko_AnalyticalManager.instance.InterstitialEvent("Max_Inter_Failed");
        }
        
    }


    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
        // interstitialStatusText.text = "Loaded";
        //    Debug.Log("Interstitial loaded");

        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, interstitialRetryAttempt);

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Interstitial failed to display with error code: " + errorInfo);
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Interstitial dismissed");
        LoadInterstitial();
    }

    #endregion

    #region Rewarded Ad Methods

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += Nicko_AnalyticalManager.instance.Revenue_ReportMax;


        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }

    public void ShowRewardedAd(Action ac)
    {
        if (isAdsRemove)
        {
            return;
        }

        action = ac;
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            Nicko_Admob.isInterstialAdPresent = true;
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);
        }
    }

    public bool IsRewardedLoaded;

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad loaded");
        IsRewardedLoaded = true;
        // Reset retry attempt
        rewardedRetryAttempt = 0;
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // rewardedStatusText.text = "Failed load: " + errorCode + "\nRetrying in 3s...";
        Debug.Log("Rewarded ad failed to load with error code: " + errorInfo);

        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays.

        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, rewardedRetryAttempt);
        IsRewardedLoaded = false;
        Invoke(nameof(LoadRewardedAd), (float)retryDelay);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Debug.Log("Rewarded ad failed to display with error code: " + adInfo);
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad displayed");
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
    }

    public Action action;

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad


        Debug.Log("Rewarded ad dismissed");
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        GlobalConstant.RewardedAdsWatched(action);
        // Rewarded ad was displayed and user should receive the reward
        Debug.Log("Rewarded ad received reward");
    }

    #endregion
}

public enum BannerPosition
{
    Bottom,
    Top,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center
};

public enum BannerSize
{
    Banner,
    SmartBanner,
    MediumRectangle,
    IABBanner,
    Leaderboard,
    Adaptive
};