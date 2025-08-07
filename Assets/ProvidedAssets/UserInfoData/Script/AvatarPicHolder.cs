using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPicHolder : MonoBehaviour
{
    public Image avatarPic;
    public bool needsToCallDuringOnEnable;
    public GameObject selectedImage;
    private void OnEnable()
    {
        if (needsToCallDuringOnEnable)
        {
            OnClickAvatarPic();
        }
    }

    public void OnClickAvatarPic()
    {
        Singolton.Instance.avatarPanelEventListener.OnAvatarImageClicked(avatarPic.sprite,selectedImage.gameObject);
    }
}