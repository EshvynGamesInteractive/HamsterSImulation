using UnityEngine;

public static class GlobalValues
{
    public static int currentStage=1;
    public static string sceneTOLoad = "MainMenu";
    public static bool retryAfterLevelCompleted;
    
    public static int TutorialPlayed
    {

        get { return PlayerPrefs.GetInt("TutorialPlayed", 0); }
        set { PlayerPrefs.SetInt("TutorialPlayed", value); }
    }
    
    public static int Music
    {

        get { return PlayerPrefs.GetInt("Music", 1); }
        set { PlayerPrefs.SetInt("Music", value); }
    }

    public static int Effects
    {

        get { return PlayerPrefs.GetInt("Effects", 1); }
        set { PlayerPrefs.SetInt("Effects", value); }
    }
    public static int ShowAppOpen
    {

        get { return PlayerPrefs.GetInt("ShowAppOpen", 0); }
        set { PlayerPrefs.SetInt("ShowAppOpen", value); }
    }
    public static int UnlockedStages
    {

        get { return PlayerPrefs.GetInt("UnlockedStages", 1); }
        set { PlayerPrefs.SetInt("UnlockedStages", value); }
    }
    
    public static int CurrentLevel
    {

        get { return PlayerPrefs.GetInt("CurrentLevel", 1); }
        set { PlayerPrefs.SetInt("CurrentLevel", value); }
    }
    public static int UnlockedLevels
    {

        get { return PlayerPrefs.GetInt("UnlockedLevels", 1); }
        set { PlayerPrefs.SetInt("UnlockedLevels", value); }
    }
    
    public static int TotalBones
    {

        get { return PlayerPrefs.GetInt("TotalBones", 0); }
        set { PlayerPrefs.SetInt("TotalBones", value); }
    }
    public static int SelectedDogIndex
    {

        get { return PlayerPrefs.GetInt("SelectedDogIndex", 0); }
        set { PlayerPrefs.SetInt("SelectedDogIndex", value); }
    }
}
