using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flagholder : MonoBehaviour
{
    public Image avatarPic;
    public bool needsToCallDuringOnEnable;
    public GameObject selectedImage;

    private void OnEnable()
    {
        if (needsToCallDuringOnEnable)
        {
            OnClickFlagPic();
        }
    }

    public void Next()
    {
        if (Singolton.Instance)
            Singolton.Instance.canvasManager.CurrentStateChanger(UserInfoStates.SceneLoading);

    }
    public void OnClickFlagPic()
    {
        if (Singolton.Instance)
            Singolton.Instance.countryEventListener.OnAvatarImageClicked(avatarPic.sprite, selectedImage.gameObject);
    }
}