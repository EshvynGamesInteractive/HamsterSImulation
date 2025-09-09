using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class Nicko_Admob : MonoBehaviour
{
    public static Nicko_Admob Instance;
    [Header("Admob Ad Units")] public string interstitialID, interHighID, interMedID;
    public string rewardedIDLow, rewardedIDMed, rewardedIDHigh;
    public string appOpenLow, appOpenMed, appOpenHigh;
    public string bannerIDLow, bannerIDMed, bannerIDHigh;
    public string recBannerIDLow, recBannerIDMed, recBannerIDHigh;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private AppOpenAd appOpenAd;
    private BannerView bannerView, recBannerView;
    private Action rewardAction;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Init()
    {
        MobileAds.Initialize(_ => { });
    }

    private AdRequest CreateAdRequest() => new AdRequest();


    #region Interstitial

    public bool ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            Nicko_AnalyticalManager.instance.InterstitialEvent("AdMob_Inter_Shown");
            return true;
        }
        else
        {
            RequestInterstitial();
            return false;
        }
    }

    private void RequestInterstitial()
    {
        string id = GlobalConstant.UseAdBidding ? interHighID : interstitialID;

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        InterstitialAd.Load(id, CreateAdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                if (id == interHighID) RequestInterstitialWithFallback(interMedID);
                else if (id == interMedID) RequestInterstitialWithFallback(interstitialID);
                else Debug.Log("NoInterstitialAvailable");
               // else Nicko_ADSManager.instance.appLovinMax.ShowInterstitial();
                return;
            }

            interstitialAd = ad;
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Nicko_ADSManager.instance.RestartAdTimers();
                RequestInterstitial();
            };
            interstitialAd.OnAdPaid += (value) =>
            {
                Nicko_AnalyticalManager.instance.Revenue_ReportAdmob(value, "Interstitial");
            };
        });
    }

    private void RequestInterstitialWithFallback(string id)
    {
        InterstitialAd.Load(id, CreateAdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                if (id == interMedID) RequestInterstitialWithFallback(interstitialID);
                else Debug.Log("NoInterstitialAvailable");
                // else Nicko_ADSManager.instance.appLovinMax.ShowInterstitial();
                return;
            }

            interstitialAd = ad;
            interstitialAd.OnAdPaid += (value) =>
            {
                Nicko_AnalyticalManager.instance.Revenue_ReportAdmob(value, "Interstitial");
            };
        });
    }

    #endregion

    #region Rewarded

    public bool ShowRewardedAd(Action onReward)
    {
        rewardAction = onReward;
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show(_ =>
            {
                GlobalConstant.RewardedAdsWatched(onReward);
                Nicko_AnalyticalManager.instance.VideoEvent("AdMob_Rewarded_Completed");
            });
            return true;
        }
        else
        {
            RequestRewarded();
            return false;
        }
    }

    private void RequestRewarded()
    {
        string id = GlobalConstant.UseAdBidding ? rewardedIDHigh : rewardedIDLow;

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }


        RewardedAd.Load(id, CreateAdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                if (id == rewardedIDHigh) RequestRewardedWithFallback(rewardedIDMed);
                else if (id == rewardedIDMed) RequestRewardedWithFallback(rewardedIDLow);
                else Nicko_ADSManager.instance.ShowNoAdPanel();
                return;
            }

            rewardedAd = ad;
            rewardedAd.OnAdPaid += (value) =>
            {
                Nicko_AnalyticalManager.instance.Revenue_ReportAdmob(value, "Rewarded");
            };
        });
    }

    private void RequestRewardedWithFallback(string id)
    {
        RewardedAd.Load(id, CreateAdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                if (id == rewardedIDMed) RequestRewardedWithFallback(rewardedIDLow);
                else Nicko_ADSManager.instance.ShowNoAdPanel();
                // else Nicko_ADSManager.instance.appLovinMax.ShowRewardedAd(rewardAction);
                return;
            }

            rewardedAd = ad;
            rewardedAd.OnAdPaid += (value) =>
            {
                Nicko_AnalyticalManager.instance.Revenue_ReportAdmob(value, "Rewarded");
            };
        });
    }

    #endregion

    #region AppOpen

    public void ShowAppOpenAd()
    {
        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            Debug.LogError("AdmobAppOpen");
            appOpenAd.Show();
            Nicko_AnalyticalManager.instance.CustomOtherEvent("AdMob_AppOpen_Shown");
        }
        else
        {
            RequestAppOpen();
        }
    }

    private void RequestAppOpen()
    {
        string id = GlobalConstant.UseAdBidding ? appOpenHigh : appOpenLow;


        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        AppOpenAd.Load(id, CreateAdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                if (id == appOpenHigh) RequestAppOpenWithFallback(appOpenMed);
                else if (id == appOpenMed) RequestAppOpenWithFallback(appOpenLow);
                else Debug.Log("NoAppOpenAvailable");
                // else Nicko_ADSManager.instance.appLovinMax.ShowAppOpenAd();
                return;
            }

            appOpenAd = ad;
            appOpenAd.OnAdPaid += (value) =>
            {
                Nicko_AnalyticalManager.instance.Revenue_ReportAdmob(value, "AppOpen");
            };
        });
    }

    private void RequestAppOpenWithFallback(string id)
    {
        AppOpenAd.Load(id, CreateAdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                if (id == appOpenMed) RequestAppOpenWithFallback(appOpenLow);
                else Debug.Log("NoAppOpenAvailable");
               // else Nicko_ADSManager.instance.appLovinMax.ShowAppOpenAd();
                return;
            }

            appOpenAd = ad;
            appOpenAd.OnAdPaid += (value) =>
            {
                Nicko_AnalyticalManager.instance.Revenue_ReportAdmob(value, "AppOpen");
            };
        });
    }

    #endregion

    #region RecBanner

    public void ShowRecBanner()
    {
        Debug.LogError("recbannerview" + recBannerView);

        if (recBannerView != null)
        {
            recBannerView.Show();
            return;
        }

        string id = GlobalConstant.UseAdBidding ? recBannerIDHigh : recBannerIDLow;
        LoadRecBannerWithFallback(id);
    }

    private void LoadRecBannerWithFallback(string id)
    {
        if (recBannerView != null)
        {
            recBannerView.Destroy();
            recBannerView = null;
        }

        Debug.LogError("mrecFallback");
        recBannerView = new BannerView(id, AdSize.MediumRectangle, AdPosition.TopLeft);
        recBannerView.LoadAd(CreateAdRequest());

        recBannerView.OnBannerAdLoaded += () => { Debug.Log("[AdMob] Rec Banner loaded: " + id); };

        recBannerView.OnBannerAdLoadFailed += (error) =>
        {
            Debug.LogWarning("[AdMob] Rec Banner failed: " + id + " → " + error.GetMessage());

            if (id == recBannerIDHigh) LoadRecBannerWithFallback(recBannerIDMed);
            else if (id == recBannerIDMed) LoadRecBannerWithFallback(recBannerIDLow);
            else Debug.Log("NoMrecAvailable");
            //else Nicko_ADSManager.instance.appLovinMax.ShowMRec();
        };

        recBannerView.OnAdPaid += (value) => { Nicko_AnalyticalManager.instance.Revenue_ReportAdmob(value, "MREC"); };
    }

    public void HideRecBanner()
    {
        recBannerView?.Destroy();
        recBannerView = null;
    }

    #endregion

    #region Banners

    public void ShowBanner()
    {
        Debug.Log("ShowAdmobBanner");
        if (bannerView != null)
        {
            bannerView.Show();
            return;
        }

        // If bidding is ON → start from High floor, else from Low
        string id = GlobalConstant.UseAdBidding ? bannerIDHigh : bannerIDLow;
        LoadBannerWithFallback(id);
    }

    private void LoadBannerWithFallback(string id)
    {
        Debug.LogError("Bannerview =" + bannerView);
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        bannerView = new BannerView(id, AdSize.Banner, AdPosition.Bottom);
        bannerView.LoadAd(CreateAdRequest());

        bannerView.OnBannerAdLoaded += () => { Debug.Log("[AdMob] Banner loaded: " + id); };

        bannerView.OnBannerAdLoadFailed += (error) =>
        {
            Debug.LogWarning("[AdMob] Banner failed: " + id + " → " + error.GetMessage());

            // Fallback logic
            if (id == bannerIDHigh) LoadBannerWithFallback(bannerIDMed);
            else if (id == bannerIDMed) LoadBannerWithFallback(bannerIDLow);
            else Debug.Log("NoBannerAvailable");
            // else Nicko_ADSManager.instance.appLovinMax.ShowBanner();
        };

        bannerView.OnAdPaid += (value) => { Nicko_AnalyticalManager.instance.Revenue_ReportAdmob(value, "Banner"); };
    }

    public void HideBanner()
    {
        Debug.Log("HideAdmobBanner");
        bannerView?.Destroy();
        bannerView = null;
    }

    #endregion
}