using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Sprite soundOn, soundOff;
    [SerializeField] Image btnSound;
    [SerializeField] GameObject levelSelection;
    public void OnBtnSound()
    {
        if(GlobalValues.Effects==1)
        {
            btnSound.sprite = soundOff;
            GlobalValues.Effects = 0;
            GlobalValues.Music = 0;
        }
        else
        {
            btnSound.sprite = soundOff;
            GlobalValues.Effects = 1;
            GlobalValues.Music = 1;
        }
        SoundManager.instance.ToggleSound();
    }
    public void OnBtnStart()
    {
        levelSelection.SetActive(true);
    }

    public void OpenPopup()
    {

    }
    public void ClosePopup()
    {

    }

    public void OnBtnQuit()
    {
        Application.Quit();
    }
}
