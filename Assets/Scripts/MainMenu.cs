using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Sprite soundOn, soundOff;
    [SerializeField] Image btnSound1, btnSound2;
    [SerializeField] GameObject levelSelection;
    // [SerializeField] private string moreGamesURL;
    // [SerializeField] private string privacyURL;
    // [SerializeField] private string rateUsURL;
    private static bool appOpenShown = false;

    private void Start()
    {
        Time.timeScale = 1;
        if (Nicko_ADSManager._Instance)
        {
            Debug.Log("aaaaaaa");
            // if (!appOpenShown)
            // {
            //     Nicko_ADSManager._Instance.ShowAppOpenAd();
            //     appOpenShown = true;
            // }


            Nicko_ADSManager._Instance.HideRecBanner();
            Nicko_ADSManager._Instance.ShowBanner("MenuStart");
        }

        CheckSound();
    }

    private void CheckSound()
    {
        if (GlobalValues.Effects == 1)
        {
            btnSound1.sprite = soundOn;
            btnSound2.sprite = soundOn;
            SoundManager.instance.SoundOn();
        }
        else
        {
            btnSound1.sprite = soundOff;
            btnSound2.sprite = soundOff;
            SoundManager.instance.SoundOff();
        }
    }

    public void OnBtnSound()
    {
        if (GlobalValues.Effects == 1)
        {
            btnSound1.sprite = soundOff;
            btnSound2.sprite = soundOff;
            GlobalValues.Effects = 0;
            GlobalValues.Music = 0;
            SoundManager.instance.SoundOff();
        }
        else
        {
            btnSound1.sprite = soundOn;
            btnSound2.sprite = soundOn;
            GlobalValues.Effects = 1;
            GlobalValues.Music = 1;
            SoundManager.instance.SoundOn();
        }
    }

    public void OnBtnStart()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.dogBark);
        levelSelection.SetActive(true);
    }

  

    public void OnBtnQuit()
    {
        Application.Quit();
    }

    public void OnBtnMoreGames()
    {
        Application.OpenURL(GlobalConstant.MoreGamesLink);
    }

    public void OnBtnPrivacy()
    {
        Application.OpenURL(GlobalConstant.PrivacyPoliciesLInk);
    }

    public void OnBtnRateUs()
    {
        Application.OpenURL(GlobalConstant.RateUsLink);
    }

    public void OnBtnClick()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonClick);
    }
}