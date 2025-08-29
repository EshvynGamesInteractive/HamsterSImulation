using System;
using System;
using GoogleMobileAds.Api;
// using LaundaryMan;
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

    public void OnRemoveAds()
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        admobInstance.HideRecBanner();
        admobInstance.HideLeftBanner();
    }

    [Header("Admob IDS")] public bool _isBannerShowing, _isBannerReady;
    [Header("Admob IDS")] public bool _isRecShowing, _isRecBannerReady;
    private bool _isFlooringBannerReady, _isFlooringBannerShowing;
    private bool isMRecShowing;

    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;

    [Header("Max IDS")] public string MaxSdkKey = "";


    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        DontDestroyOnLoad(gameObject);
        btnOkNoAd.onClick.AddListener(HideNoAdPanel);
    }


    public void ShowNoAdPanel()
    {
        noAdAvailablePanel.SetActive(true);
    }

    public void HideNoAdPanel()
    {
        noAdAvailablePanel.SetActive(false);
    }
    //  public AdTimer TIMER;

    public void Init()
    {
        Debug.Log("Application.version " + Application.version);

        PostInit();
        // TIMER.Init();
    }

    public Nicko_Admob admobInstance;
    public AppLovinMax appLovinMax;

    void PostInit()
    {
        adPriority = GlobalConstant.adPriority;
        admobInstance.bannerIDMed = GlobalConstant.Nicko_Admob_Banner_MID;
        admobInstance.InterMediumFloorID = GlobalConstant.Nicko_Admob_Inter_IdMid;
        admobInstance.rewardedIDMed = GlobalConstant.Nicko_Admob_Rewarded_Id_Mid;
        admobInstance.appOpenIDMed = GlobalConstant.Nicko_Admob_AppOpen_Id_Mid;

        admobInstance.bannerIDHigh = GlobalConstant.Nicko_Admob_Banner_HIGH;
        admobInstance.InterHighFloorID = GlobalConstant.Nicko_Admob_Inter_IdHigh;
        admobInstance.rewardedIDHigh = GlobalConstant.Nicko_Admob_Rewarded_Id_High;
        admobInstance.appOpenIDHigh = GlobalConstant.Nicko_Admob_AppOpen_IdHigh;

        admobInstance.LowBannerID = GlobalConstant.Nicko_Admob_Banner_Simple;
        admobInstance.interstitialID = GlobalConstant.Nicko_Admob_Inter_IdLow;
        admobInstance.rewardedIDLow = GlobalConstant.Nicko_Admob_Rewarded_Id_Simple;
        admobInstance.appOpenIDLow = GlobalConstant.Nicko_Admob_AppOpen_Id_Low;


        appLovinMax.MaxSdkKey = MaxSdkKey;
        appLovinMax.InterstitialAdUnitId = GlobalConstant.InterstitialAdUnitId;
        appLovinMax.RewardedAdUnitId = GlobalConstant.RewardedAdUnitId;

        appLovinMax.adBannerAdUnitId = GlobalConstant.BannerMax;
        appLovinMax.recBannerAdUnitId = GlobalConstant.Mrecmax;
        appLovinMax.appOpenAdUnitId = GlobalConstant.Appopenmax;
        appLovinMax.RewardedAdUnitId = GlobalConstant.RewardedAdUnitId;


        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        admobInstance.Initialize();
        appLovinMax.Init();
    }

    public void ShowInterstitial(string placement)
    {
         return;
        Nicko_Admob.isInterstialAdPresent = true;
        if (isAdsRemove)
        {
            return;
        }

        if (!GlobalConstant.AdsON)
        {
            return;
        }

        Nicko_AnalyticalManager.instance.InterstitialEvent(placement);

        if (adPriority == AdPriority.Max)
        {
            if (appLovinMax)
            {
                appLovinMax.ShowInterstitial();
                print("Max Ad Showed");
            }
            else
            {
                admobInstance.ShowInterstitial();
            }
        }
        else
        {
            //  appLovinMax.InitializeInterstitialAds();
            admobInstance.ShowInterstitial();
        }
    }

    #region Rewarded Ad Methods

    public Action action;

    public void ShowRewardedAd(Action ac, string placement)
    {
        if (isAdsRemove)
        {
            return;
        }

        if (!GlobalConstant.AdsON)
        {
            return;
        }

        action = ac;
        if (adPriority == AdPriority.Max)
        {
             appLovinMax.ShowRewardedAd(ac); 
            // appLovinMax.ShowRewardedOrInterstitialAd(ac); //added by Khubaib
        }
        else
            admobInstance.ShowRewardedAdmob(ac);

        if (Nicko_AnalyticalManager.instance)
        {
            Nicko_AnalyticalManager.instance.VideoEvent(placement);
        }
    }

    public void ShowRewardedAd(string placement)
    {
        if (isAdsRemove)
        {
            return;
        }

        if (!GlobalConstant.AdsON)
        {
            return;
        }

        if (GlobalConstant.isLogger)
            print("Nicko_Admob.Instance");
        if (adPriority == AdPriority.Max)
        {
             appLovinMax.ShowRewardedAd(action); 
            // appLovinMax.ShowRewardedOrInterstitialAd(action);//added by Khubaib
        }
        else
            admobInstance.ShowRewardedAdmob(action);

        if (Nicko_AnalyticalManager.instance)
        {
            Nicko_AnalyticalManager.instance.VideoEvent(placement);
        }
    }

    #endregion

    #region Banner Ad Methods

    // public void ShowBanner(string placement)
    // {
    //     if (isAdsRemove)
    //     {
    //         return;
    //     }
    //
    //     if (!GlobalConstant.AdsON)
    //     {
    //         return;
    //     }
    //
    //     _isBannerShowing = true;
    //
    //     admobInstance.ShowLeftBanner();
    //     if (Nicko_AnalyticalManager.instance)
    //     {
    //         Nicko_AnalyticalManager.instance.CustomScreenEvent(placement);
    //     }
    // }


    public void ShowBanner(string placement)
    {
        if (isAdsRemove || !GlobalConstant.AdsON)
        {
            Debug.Log("[ADS] Ads disabled or removed, skipping banner.");
            return;
        }

        if (_isBannerShowing)
            HideBanner(); // Hide existing banner

        _isBannerShowing = true;

        if (appLovinMax != null)
        {
            appLovinMax.ShowBanner();
            Debug.Log("[ADS] Showing Max banner.");
        }
        else if (admobInstance != null)
        {
            admobInstance.ShowLeftBanner();
            Debug.Log("[ADS] Max not ready, showing AdMob banner.");
        }
        else
        {
            _isBannerShowing = false;
            Debug.LogWarning("[ADS] No banner ready (Max or AdMob).");
        }

        Nicko_AnalyticalManager.instance?.CustomScreenEvent(placement);
    }


    // public void RecShowBanner(string placement) //by khubaib
    // {
    //     if (isAdsRemove)
    //     {
    //         return;
    //     }
    //
    //     if (!GlobalConstant.AdsON)
    //     {
    //         return;
    //     }
    //
    //     _isBannerShowing = true;
    //
    //     admobInstance.ShowRecBanner();
    //     if (Nicko_AnalyticalManager.instance)
    //     {
    //         Nicko_AnalyticalManager.instance.CustomScreenEvent(placement);
    //     }
    // }
    // public void RecShowBanner(string placement)
    // {
    //     if (isAdsRemove || !GlobalConstant.AdsON)
    //         return;
    //
    //     if (_isBannerShowing)
    //         HideRecBanner(); // Hide existing banner
    //
    //     _isBannerShowing = true;
    //
    //     if (admobInstance != null)
    //     {
    //         admobInstance.ShowRecBanner();
    //         Debug.Log("[ADS] Showing Admob rectangular banner.");
    //     }
    //     else if (appLovinMax != null && appLovinMax._isRecBannerReady)
    //
    //     {
    //         appLovinMax.ShowRecBanner();
    //         Debug.Log("[ADS] Admob not ready, showing Max rectangular banner.");
    //     }
    //     else
    //     {
    //         _isBannerShowing = false;
    //         Debug.LogWarning("[ADS] No rectangular banner ready.");
    //     }
    //
    //     Nicko_AnalyticalManager.instance?.CustomScreenEvent(placement);
    // }
    public void RecShowBanner(string placement)
    {
        if (isAdsRemove || !GlobalConstant.AdsON)
        {
            Debug.Log("[ADS] Ads disabled or removed, skipping rectangular banner.");
            return;
        }

       
        _isBannerShowing = true;

        if (appLovinMax)
        {
            appLovinMax.ShowRecBanner();
            Debug.Log("[ADS] Showing Max rectangular banner.");
        }
        else if (admobInstance != null)
        {
            admobInstance.ShowRecBanner();
           // appLovinMax.LoadRecBanner();
            Debug.Log("[ADS] Max not ready, showing AdMob rectangular banner.");
        }
        else
        {
            _isBannerShowing = false;
            Debug.LogWarning("[ADS] No rectangular banner ready (Max or AdMob).");
        }

        Nicko_AnalyticalManager.instance?.CustomScreenEvent(placement);
    }

    public void HideRecBanner()
    {
        Debug.Log("HideMREC1");
        if (!_isBannerShowing)
            return;

        Debug.Log("HideMREC2");
        if (admobInstance != null)
        {
            admobInstance.HideRecBanner(); // Ensure AdMob has this method
            Debug.Log("[ADS] Hiding Admob rectangular banner.");
        }

        if (appLovinMax != null)
        {
            appLovinMax.HideRecBanner();
            Debug.Log("[ADS] Hiding Max MREC banner.");
        }

        _isBannerShowing = false;
        _isBannerReady = false;
    }

    public void HideBanner()
    {
        Debug.Log("HideBanner");
        if (admobInstance != null)
        {
            admobInstance.HideLeftBanner();
            admobInstance.HideRightBanner();
            Debug.Log("[ADS] Hiding Admob banner.");
        }

        if (appLovinMax != null)
        {
            appLovinMax.HideBanner();
            Debug.Log("[ADS] Hiding Max banner.");
        }

        _isBannerReady = false;
        _isBannerShowing = false;
        // admobInstance.HideLeftBanner();
        // admobInstance.HideRightBanner();
    }

    public void ShowAppOpenAd()
    {
        Debug.Log("showing appopen");
        if (!GlobalConstant.AdsON)
        {
            Debug.Log("[ADS] Ads disabled or removed, skipping App Open ad.");
            return;
        }


        if (appLovinMax != null && appLovinMax.IsAppOpenReady())
        {
         //   HideRecBanner();
            HideBanner();
            appLovinMax.ShowAppOpen();
            Debug.Log("[ADS] Showing Max App Open ad.");
        }
        else
        {
        //    HideRecBanner();
            HideBanner();
            admobInstance.ShowAppOpenAd();
        }
    }
    // public void HideRecBanner()
    // {
    //     _isBannerReady = false;
    //     _isBannerShowing = false;
    //     admobInstance.HideRecBanner();
    // }

    public void HideBannerAppOpen()
    {
        admobInstance.HideRightBanner();
        admobInstance.HideLeftBanner();
    }

    public void HideRecBannerAppOpen()
    {
        admobInstance.HideRecBanner();
    }

    public bool IsBannerAdAvailable()
    {
        return _isBannerReady;
    }

    public bool IsMaxRecBannerReady()
    {
        print(appLovinMax._isRecBannerReady);
      //  appLovinMax.LoadRecBanner();
        return appLovinMax != null && appLovinMax.IsRecBannerReady();
    }

    #endregion
}