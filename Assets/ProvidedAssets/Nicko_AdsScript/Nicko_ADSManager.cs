using System;
using DG.Tweening;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

public class Nicko_ADSManager : MonoBehaviour
{
    public static Nicko_ADSManager _Instance;

    public GameObject noAdAvailablePanel;
     public Button btnOkNoAd;

    public enum AdPriority
    {
        Admob,
        Max
    }

    public AdPriority adPriority;

    public static bool isAdsRemove
    {
        get => PlayerPrefs.GetInt("RemoveAds", 0) == 1;
        set => PlayerPrefs.SetInt("RemoveAds", value ? 1 : 0);
    }

    [Header("Admob State Flags")] public bool _isBannerShowing, _isBannerReady;
    public bool _isRecShowing, _isRecBannerReady;

    [Header("References")] public Nicko_Admob admobInstance;
    public Nicko_AppLovinMax appLovinMax;
    // public TimeBasedAd timeBasedAd;

    
    public static bool isInterstitialShown;
    
    
    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;

        DontDestroyOnLoad(gameObject);
        btnOkNoAd.onClick.AddListener(HideNoAdPanel);
    }

    #region Init

    public void Init()
    {
        Debug.Log("Application.version " + Application.version);
        PostInit();
    }

    void PostInit()
    {
        adPriority = GlobalConstant.adPriority;
        admobInstance.recBannerIDMed = admobInstance.bannerIDMed = GlobalConstant.Nicko_Admob_Banner_MID;
        admobInstance.interMedID = GlobalConstant.Nicko_Admob_Inter_IdMid;
        admobInstance.rewardedIDMed = GlobalConstant.Nicko_Admob_Rewarded_Id_Mid;
        admobInstance.appOpenMed = GlobalConstant.Nicko_Admob_AppOpen_Id_Mid;

        admobInstance.recBannerIDHigh = admobInstance.bannerIDHigh = GlobalConstant.Nicko_Admob_Banner_HIGH;
        admobInstance.interHighID = GlobalConstant.Nicko_Admob_Inter_IdHigh;
        admobInstance.rewardedIDHigh = GlobalConstant.Nicko_Admob_Rewarded_Id_High;
        admobInstance.appOpenHigh = GlobalConstant.Nicko_Admob_AppOpen_IdHigh;

        admobInstance.recBannerIDLow = admobInstance.bannerIDLow = GlobalConstant.Nicko_Admob_Banner_Simple;
        admobInstance.interstitialID = GlobalConstant.Nicko_Admob_Inter_IdLow;
        admobInstance.rewardedIDLow = GlobalConstant.Nicko_Admob_Rewarded_Id_Simple;
        admobInstance.appOpenLow = GlobalConstant.Nicko_Admob_AppOpen_Id_Low;
        appLovinMax.InterstitialAdUnitId = GlobalConstant.InterstitialAdUnitId;
        appLovinMax.RewardedAdUnitId = GlobalConstant.RewardedAdUnitId;
        appLovinMax.BannerAdUnitId = GlobalConstant.BannerMax;
        appLovinMax.MrecAdUnitId = GlobalConstant.Mrecmax;
        appLovinMax.AppOpenAdUnitId = GlobalConstant.Appopenmax;
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        admobInstance.Init();
        appLovinMax.Init();
        DOVirtual.DelayedCall(5, ShowAppOpenAd);
    }

    #endregion

    #region Remove Ads

    public void OnRemoveAds()
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        Debug.Log("[ADS] Remove Ads purchased → disabling all ads.");

        // Hide AdMob banners

        admobInstance?.HideBanner();
        admobInstance?.HideRecBanner();

        // Hide Max banners
        appLovinMax?.HideBanner();
        appLovinMax?.HideMRec();

        // Reset flags
        _isBannerShowing = false;
        _isRecShowing = false;
        _isBannerReady = false;
        _isRecBannerReady = false;
    }

    #endregion

    #region UI Panel

    public void ShowNoAdPanel()
    {
        noAdAvailablePanel.SetActive(true);
    }

    public void HideNoAdPanel()
    {
        noAdAvailablePanel.SetActive(false);
    }

    #endregion

    #region Interstitial

    public void ShowInterstitial(string placement)
    {
        if (isAdsRemove || !GlobalConstant.AdsON)
            return;

        Nicko_AnalyticalManager.instance.InterstitialEvent(placement);

        if (adPriority == AdPriority.Max && appLovinMax != null)
        {
            if (appLovinMax.InterstitialAdUnitId != string.Empty)
            {
                isInterstitialShown = true;
                appLovinMax.ShowInterstitial();
                // timeBasedAd.ResetAdCycle();
            }
        }
        else
        {
            isInterstitialShown = true;
            admobInstance.ShowInterstitial();
            // timeBasedAd.ResetAdCycle();
        }
    }

    #endregion

    #region Rewarded

    public Action action;

    public void ShowRewardedAd(Action ac, string placement)
    {
        if (isAdsRemove || !GlobalConstant.AdsON) return;

        action = ac;

        if (adPriority == AdPriority.Max && appLovinMax != null)
        {
            if (appLovinMax.RewardedAdUnitId != string.Empty)
            {
                isInterstitialShown = true;
                appLovinMax.ShowRewardedAd(ac);
                // timeBasedAd.ResetAdCycle();
            }
        }
        else
        {
            isInterstitialShown = true;
            admobInstance.ShowRewardedAd(ac);
            // timeBasedAd.ResetAdCycle();
        }

        Nicko_AnalyticalManager.instance?.VideoEvent(placement);
    }

    #endregion

    #region Banner

    public void ShowBanner(string placement)
    {
        if (isAdsRemove || !GlobalConstant.AdsON) return;

        if (_isBannerShowing) HideBanner();
        _isBannerShowing = true;

        if (adPriority == AdPriority.Max && appLovinMax != null)
        {
            appLovinMax.ShowBanner();
        }
        else
        {
            admobInstance.ShowBanner();
        }

        Nicko_AnalyticalManager.instance?.CustomScreenEvent(placement);
    }

    public void HideBanner()
    {
        admobInstance?.HideRecBanner();
        appLovinMax?.HideBanner();

        _isBannerShowing = false;
        _isBannerReady = false;
    }

    #endregion

    #region RecBanner

    public void RecShowBanner(string placement)
    {
        if (isAdsRemove || !GlobalConstant.AdsON) return;

        _isRecShowing = true;

        if (adPriority == AdPriority.Max && appLovinMax != null)
        {
            if (appLovinMax.BannerAdUnitId != string.Empty)
                appLovinMax.ShowMRec();
        }
        else
        {
            admobInstance.ShowRecBanner();
        }

        Nicko_AnalyticalManager.instance?.CustomScreenEvent(placement);
    }

    public void HideRecBanner()
    {
        admobInstance?.HideRecBanner();
        appLovinMax?.HideMRec();

        _isRecShowing = false;
        _isRecBannerReady = false;
    }

    #endregion

    #region AppOpen

    public void ShowAppOpenAd()
    {
        if (isAdsRemove || !GlobalConstant.AdsON) return;

        if (isInterstitialShown)
        {
            isInterstitialShown = false;
            return;
        }
        
        HideBanner();
        HideRecBanner();

        if (adPriority == AdPriority.Max && appLovinMax != null && appLovinMax.AppOpenAdUnitId != string.Empty)
        {
            appLovinMax.ShowAppOpenAd();
        }
        else
        {
            admobInstance.ShowAppOpenAd();
        }
    }

    #endregion


    private void OnApplicationPause(bool pauseStatus)
    {
        if(!pauseStatus)
            ShowAppOpenAd();
    }
}