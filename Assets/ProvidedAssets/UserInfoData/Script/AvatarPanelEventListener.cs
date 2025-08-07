using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPanelEventListener : MonoBehaviour
{
    public Text nameText;
    public Text ageText;

    private void OnEnable()
    {
        nameText.text = "Name: " + PlayerPrefsSaver.nameData;
        ageText.text = "Age: " + PlayerPrefsSaver.Age;
      /*  if (AdMob.Instance)
        {
            AdMob.Instance.ShowLeftBanner();
            AdMob.Instance.ShowRightBanner();
            AdMob.Instance.HideRecBanner();
        }*/
    }

    public Image avatarImage;
    public GameObject[] avatarSprite;

    public void Next()
    {
        Singolton.Instance.canvasManager.CurrentStateChanger(UserInfoStates.CountrySelection);
    }

    public void OnAvatarImageClicked(Sprite i, GameObject avatarPanelSelected)
    {
        foreach (var avatar in avatarSprite)
        {
            avatar.SetActive(false);
        }

        avatarPanelSelected.SetActive(true);

        avatarImage.sprite = i;
        PlayerPrefsSaver.currentAvatarSprite = i;
    }
}