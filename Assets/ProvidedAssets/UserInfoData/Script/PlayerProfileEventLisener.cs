using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileEventLisener : MonoBehaviour
{
    public TMP_InputField inputFieldName;
    public TMP_InputField inputFieldAge;

    private void OnEnable()
    {
        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.RecShowBanner("PlayerProfileEventLisener");
        }
    }
    public void Next()
    {
        Singolton.Instance.canvasManager.CurrentStateChanger(UserInfoStates.CountrySelection);
        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.ShowInterstitial("PlayerProfileEventListener");
        }
    }

    public void DataCollect()
    {
        PlayerPrefsSaver.nameData = inputFieldName.text;
        PlayerPrefsSaver.Age = inputFieldAge.text;
        Next();
    }
}

public static class PlayerPrefsSaver
{
    public static string nameData
    {
        get => PlayerPrefs.GetString("nameData");
        set => PlayerPrefs.SetString("nameData", value);
    }

    public static string Age
    {
        get => PlayerPrefs.GetString("Age");
        set => PlayerPrefs.SetString("Age", value);
    }

    public static Sprite currentAvatarSprite;
    public static Sprite currentFlagSprite;
}