using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class Nicko_AppLovinMax : MonoBehaviour
{
    public static Nicko_AppLovinMax Instance;

    [Header("MAX Ad Units")] public string InterstitialAdUnitId;
    public string RewardedAdUnitId;
    public string BannerAdUnitId;
    public string MrecAdUnitId;
    public string AppOpenAdUnitId;

    private bool isMrecLoaded;
    private Action rewardAction;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }


    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        MaxSdk.InitializeSdk();
        PostInit();
    }

    private void PostInit()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            if (InterstitialAdUnitId != string.Empty)
                InitializeInterstitial();
            if (RewardedAdUnitId != string.Empty)
                InitializeRewarded();
            if (BannerAdUnitId != string.Empty)

                InitializeBanner();

            Debug.LogError(MrecAdUnitId);
            if (MrecAdUnitId != string.Empty)

                InitializeMRec();
            if (AppOpenAdUnitId != string.Empty)
                InitializeAppOpen();
        };
    }

    #region Interstitial

    private void InitializeInterstitial()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += (id, info) =>
        {
            Nicko_AnalyticalManager.instance.InterstitialEvent("Max_Inter_Loaded");
        };
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += (id, err, info) =>
        {
            Nicko_AnalyticalManager.instance.InterstitialEvent("Max_Inter_Failed");
            Nicko_ADSManager._Instance.admobInstance.ShowInterstitial();
        };
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += (id, info) => { LoadInterstitial(); };
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
        {
            Nicko_AnalyticalManager.instance.Revenue_ReportMax(adUnit, adInfo);
        };
        LoadInterstitial();
    }

    private void LoadInterstitial() => MaxSdk.LoadInterstitial(InterstitialAdUnitId);

    public void ShowInterstitial()
    {
        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
            Nicko_AnalyticalManager.instance.InterstitialEvent("Max_Inter_Shown");
        }
        else
        {
            Nicko_ADSManager._Instance.admobInstance.ShowInterstitial();
        }
    }

    #endregion

    #region Rewarded

    private void InitializeRewarded()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += (id, info) =>
        {
            Nicko_AnalyticalManager.instance.VideoEvent("Max_Rewarded_Loaded");
        };
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += (id, err, info) =>
        {
            Nicko_ADSManager._Instance.admobInstance.ShowRewardedAd(rewardAction);
        };
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += (id, info) => { LoadRewarded(); };
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += (id, reward, info) =>
        {
            GlobalConstant.RewardedAdsWatched(rewardAction);
            Nicko_AnalyticalManager.instance.VideoEvent("Max_Rewarded_Completed");
        };
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
        {
            Nicko_AnalyticalManager.instance.Revenue_ReportMax(adUnit, adInfo);
        };
        LoadRewarded();
    }

    private void LoadRewarded() => MaxSdk.LoadRewardedAd(RewardedAdUnitId);

    public void ShowRewardedAd(Action onReward)
    {
        rewardAction = onReward;
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);
        }
        else
        {
            Nicko_ADSManager._Instance.admobInstance.ShowRewardedAd(onReward);
        }
    }

    #endregion

    #region Banner

    private void InitializeBanner()
    {
        MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, new Color(1, 1, 1, 0f));
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += (id, err) =>
        {
          //  Nicko_ADSManager._Instance.admobInstance.ShowBanner();//it can trigger failed event anytime even in gameplay
        };
    }

    public void ShowBanner()
    {
        Debug.Log("ShowMaxBanner");
        MaxSdk.ShowBanner(BannerAdUnitId);
    }

    public void HideBanner()
    {
        Debug.Log("HideMaxBanner");
        MaxSdk.HideBanner(BannerAdUnitId);
    }

    #endregion

    #region MREC

    private void InitializeMRec()
    {
        MaxSdk.CreateMRec(MrecAdUnitId, MaxSdkBase.AdViewPosition.TopLeft);
        Debug.LogError("mrecCreated");
        MaxSdkCallbacks.MRec.OnAdLoadedEvent += (id, info) =>
        {
            isMrecLoaded = true;
            MaxSdk.HideMRec(MrecAdUnitId);
        };
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += (id, err) =>
        {
            isMrecLoaded = false;
            //Nicko_ADSManager._Instance.admobInstance.ShowRecBanner(); //it can trigger failed event anytime even in gameplay
        };
        //MaxSdk.HideMRec(MrecAdUnitId);
    }

    public void ShowMRec()
    {
        Debug.LogError("ismrecLoaded " + isMrecLoaded);
        if (isMrecLoaded)
            MaxSdk.ShowMRec(MrecAdUnitId);
        // else
        //     Nicko_ADSManager._Instance.admobInstance.ShowRecBanner();
    }

    public void HideMRec()
    {
        if (isMrecLoaded)
            MaxSdk.HideMRec(MrecAdUnitId);
    }

    #endregion

    #region AppOpen

    private void InitializeAppOpen()
    {
        MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += (id, err) =>
        {
            Nicko_ADSManager._Instance.admobInstance.ShowAppOpenAd();
        };
        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += (id, info) => { LoadAppOpen(); };
        MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += (id, info) =>
        {
            Nicko_AnalyticalManager.instance.Revenue_ReportMax(id, info);
        };
        LoadAppOpen();
    }

    private void LoadAppOpen() => MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);

    public void ShowAppOpenAd()
    {
        if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId))
        {
            MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
        }
        else
        {
            Nicko_ADSManager._Instance.admobInstance.ShowAppOpenAd();
        }
    }

    #endregion
}