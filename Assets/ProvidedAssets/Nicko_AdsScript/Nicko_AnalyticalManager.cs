using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using GameAnalyticsSDK;
using GoogleMobileAds.Api;
using UnityEngine;

public class Nicko_AnalyticalManager : MonoBehaviour
{
    public static Nicko_AnalyticalManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }

        instance = this;
    }


    #region Events

    public void Revenue_ReportMax(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        double revenue = impressionData.Revenue;
        // AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
        // // set revenue and currency
        // adjustAdRevenue.setRevenue(revenue, "USD");
        //
        //
        // adjustAdRevenue.setAdRevenueNetwork(impressionData.NetworkName);
        // adjustAdRevenue.setAdRevenueUnit(impressionData.AdUnitIdentifier);
        //
        // // track ad revenue
        // Adjust.trackAdRevenue(adjustAdRevenue);
        Debug.Log("EnterMax" + impressionData);

        var impressionParameters = new[]
        {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("paid_ad_impression", impressionParameters);
    }


    public void ProgressionEvent(int Id, int LevelNumber)
    {
        string msg = "";

        if (Id == 1) //levelStart
            msg = "GD_LVL_Start";
        else if (Id == 2) //levelWin
            msg = "GD_LVL_Complete";
        else if (Id == 3) //levelFail
            msg = "GD_LVL_Fail";
        else if (Id == 4) //levelTie
            msg = "GD_LVL_Tie";
        else if (Id == 5) //levelRetry
            msg = "GD_LVL_Retry";

        FB_ProgressionEvent(msg, LevelNumber);
        GA_ProgressionEvent(Id, msg, LevelNumber);
    }

    public void VideoEvent(string placement)
    {
        FB_VideoEvent(placement);
        GA_VideoEvent(placement);
    }

    public void InterstitialEvent(string placement)
    {
        FB_InterstitialEvent(placement);
        GA_InterstitialEvent(placement);
    }

    public void CustomScreenEvent(string placement)
    {
        FB_CustomScreenEvent(placement);
        GA_CustomScreenEvent(placement);
    }

    public void CustomBtnEvent(string placement)
    {
        FB_CustomBtnEvent(placement);
        GA_CustomBtnEvent(placement);
    }

    public void CustomOtherEvent(string placement)
    {
        FB_CustomOtherEvent(placement);
        GA_CustomOtherEvent(placement);
    }

    public void IAPEvent(string sku)
    {
        FB_IAPEvent(sku);
        GA_IAPEvent(sku);
    }

    #endregion


    #region firebaseEvents

    public void FB_ProgressionEvent(string msg, int LevelNumber)
    {
        FirebaseAnalytics.LogEvent(msg + LevelNumber);
        if (GlobalConstant.isLogger)
            print("FB_ProgressionEvent :" + msg + LevelNumber);
    }

    public void FB_VideoEvent(string placement)
    {
        FirebaseAnalytics.LogEvent("ADS_REWARDED_" + placement);
        if (GlobalConstant.isLogger)
            print("FB_ADS_REWARDED_" + placement);
    }

    public void FB_InterstitialEvent(string placement)
    {
        FirebaseAnalytics.LogEvent("ADS_INTER_" + placement);
        if (GlobalConstant.isLogger)
            print("FB_ADS_INTER_" + placement);
    }

    public void FB_CustomScreenEvent(string placement)
    {
        FirebaseAnalytics.LogEvent("GD_SCREEN_" + placement);
        if (GlobalConstant.isLogger)
            print("FB_GD_SCREEN_" + placement);
    }

    public void FB_CustomBtnEvent(string placement)
    {
        FirebaseAnalytics.LogEvent("GD_BTN_" + placement);

        if (GlobalConstant.isLogger)
            print("FB_GD_BTN_" + placement);
    }

    public void FB_CustomOtherEvent(string placement)
    {
        FirebaseAnalytics.LogEvent("GD_Other_" + placement);
        if (GlobalConstant.isLogger)
            print("FB_GD_Other_" + placement);
    }

    public void FB_IAPEvent(string sku)
    {
        FirebaseAnalytics.LogEvent("IAP_" + sku);
        if (GlobalConstant.isLogger)
            print("FB_IAP_" + sku);
    }

    #endregion


    #region GameAnalyticsEvent

    public void GA_ProgressionEvent(int Id, string msg, int LevelNumber)
    {
        GameAnalytics.NewProgressionEvent((GAProgressionStatus)Id, LevelNumber.ToString());
        GameAnalytics.NewDesignEvent(msg + LevelNumber);

        if (GlobalConstant.isLogger)
            print("GA_ProgressionEvent :" + msg + LevelNumber);
    }

    public void GA_CustomScreenEvent(string placement)
    {
        GameAnalytics.NewDesignEvent("GD_SCREEN_" + placement);
        if (GlobalConstant.isLogger)
            print("GA_GD_SCREEN_" + placement);
    }

    public void GA_CustomBtnEvent(string placement)
    {
        GameAnalytics.NewDesignEvent("GD_BTN_" + placement);
        if (GlobalConstant.isLogger)
            print("GA_GD_BTN_" + placement);
    }

    public void GA_VideoEvent(string placement)
    {
        GameAnalytics.NewDesignEvent("ADS_REWARDED_" + placement);
        if (GlobalConstant.isLogger)
            print("GA_ADS_REWARDED_" + placement);
    }

    public void GA_InterstitialEvent(string placement)
    {
        GameAnalytics.NewDesignEvent("ADS_INTER_" + placement);
        if (GlobalConstant.isLogger)
            print("GA_ADS_INTER_" + placement);
    }

    public void GA_CustomOtherEvent(string placement)
    {
        GameAnalytics.NewDesignEvent("GD_Other_" + placement);
        if (GlobalConstant.isLogger)
            print("GA_GD_Other_" + placement);
    }

    public void GA_IAPEvent(string sku)
    {
        GameAnalytics.NewDesignEvent("IAP_" + sku);
        if (GlobalConstant.isLogger)
            print("GA_IAP_" + sku);
    }

    #endregion


    #region Admob Paid Event

    public void Revenue_ReportAdmob(AdValue admobAd, string adType)
    {
        double revenue = (admobAd.Value / 1000000f);

        //Dictionary<string, string> dic = new Dictionary<string, string>();
        //dic.Add("ad_format", "admob_" + adType);
        //AppsFlyerAdRevenue.logAdRevenue("simple_admob", AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob, revenue, "USD", dic);
        if (GlobalConstant.isLogger)
            Debug.Log("EnterAdmob" + revenue);

        var impressionParameters = new[]
        {
            new Firebase.Analytics.Parameter("ad_platform", "Admob"),
            new Firebase.Analytics.Parameter("ad_source", "Simple Admob"),
            new Firebase.Analytics.Parameter("ad_format", "Admob_" + adType),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", admobAd.CurrencyCode),
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("paid_ad_impression", impressionParameters);
    }

    #endregion

    #region Max Paid Event

//    public void Revenue_ReportMax(string adUnitId, MaxSdkBase.AdInfo impressionData)
//    {
//        double revenue = impressionData.Revenue;
//        AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
//        // set revenue and currency
//        adjustAdRevenue.setRevenue(revenue, "USD");


//        adjustAdRevenue.setAdRevenueNetwork(impressionData.NetworkName);
//        adjustAdRevenue.setAdRevenueUnit(impressionData.AdUnitIdentifier);

//        // track ad revenue
//        Adjust.trackAdRevenue(adjustAdRevenue);
//        Debug.Log("EnterMax" + impressionData);
//        if (impressionData.NetworkName == "AdMob" || impressionData.NetworkName == "Google Ad Manager Native")
//        {
//            return;
//        }
//        var impressionParameters = new[] {
//  new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
//  new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
//  new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
//  new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
//  new Firebase.Analytics.Parameter("value", revenue),
//  new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
//};
//        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
//        Firebase.Analytics.FirebaseAnalytics.LogEvent("paid_ad_impression", impressionParameters);


//    }

    #endregion
}