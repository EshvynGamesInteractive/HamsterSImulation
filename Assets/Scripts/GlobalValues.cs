using UnityEngine;

public static class GlobalValues
{
    public static int currentLevel=1;
    public static string sceneTOLoad = "MainMenu";
    public static bool retryAfterLevelCompleted;
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

    public static int UnlockedLevels
    {

        get { return PlayerPrefs.GetInt("UnlockedLevels", 1); }
        set { PlayerPrefs.SetInt("UnlockedLevels", value); }
    }

}
