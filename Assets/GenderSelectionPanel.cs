using UnityEngine;
using UnityEngine.UI;

public class GenderSelectionPanel : MonoBehaviour
{
    public Image selectedAvatarIcon;
    public Sprite girlSprite;
    public Sprite boySprite;

    public GameObject girlHighlight;
    public GameObject boyHighlight;

    public Button girlButton;
    public Button boyButton;

    public enum GenderSelectionType
    {
        Boy,
        Girl
    }
    private void OnEnable()
    {
        GenderSelectionType savedAvatar = PlayerPrefsManager.GenderSelection;
        if (savedAvatar == GenderSelectionType.Girl)
        {
            SelectAvatar(GenderSelectionType.Girl);
        }
        else if (savedAvatar == GenderSelectionType.Boy)
        {
            SelectAvatar(GenderSelectionType.Boy);
        }
    }

    public void OnAvatarButtonClicked(int avatarIndex)
    {
        // 0 = Girl, 1 = Boy
        GenderSelectionType gender = avatarIndex == 0 ? GenderSelectionType.Boy : GenderSelectionType.Girl;
        SelectAvatar(gender);
    }

    private void SelectAvatar(GenderSelectionType gender)
    {
        PlayerPrefsManager.GenderSelection = gender;

        if (gender == GenderSelectionType.Girl)
        {
            selectedAvatarIcon.sprite = girlSprite;
            girlHighlight.SetActive(true);
            boyHighlight.SetActive(false);
        }
        else
        {
            selectedAvatarIcon.sprite = boySprite;
            girlHighlight.SetActive(false);
            boyHighlight.SetActive(true);
        }
    }

    public void OnNextClicked()
    {
        Singolton.Instance.canvasManager.CurrentStateChanger(UserInfoStates.PlayerProfile); // or next screen
    }
}
public static class PlayerPrefsManager
{
    public static GenderSelectionPanel.GenderSelectionType GenderSelection
    {
        get 
        {
            return (GenderSelectionPanel.GenderSelectionType)PlayerPrefs.GetInt(nameof(GenderSelection));
        }
        set
        {
            PlayerPrefs.SetInt(nameof(GenderSelection),(int)value);
        }
    }
    public static bool Shown
    {
        get 
        {
            return PlayerPrefs.GetInt(nameof(Shown))==1;
        }
        set
        {
            PlayerPrefs.SetInt(nameof(Shown), value ? 1:0);
        }
    }
}