using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CountryEventListener : MonoBehaviour
{
    public Image avatarImage;
    public Image countryImage;
    public GameObject[] avatarSprite;
    public Text playerAge;
    public Text playerName;

    public void OnEnable()
    {
        playerAge.text = PlayerPrefsSaver.nameData;
        playerName.text = PlayerPrefsSaver.Age;
        avatarImage.sprite = PlayerPrefsSaver.currentAvatarSprite;
    }

    public void Next()
    {
        CanvasScriptSplash.instance.LoadScene(2);
        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.ShowInterstitial("CountryEventListener");
            Nicko_ADSManager._Instance.HideRecBanner();
        }

        PlayerPrefsManager.Shown = true;
    }

    public void OnAvatarImageClicked(Sprite i, GameObject avatarPanelSelected)
    {
        foreach (var avatar in avatarSprite)
        {
            avatar.SetActive(false);
        }

        avatarPanelSelected.SetActive(true);
        countryImage.sprite = i;
        PlayerPrefsSaver.currentFlagSprite = i;
    }
}