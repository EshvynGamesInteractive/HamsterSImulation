using UnityEngine;

public static class PlayerPrefManager
{
    private const string SelectedLevelKey = "SelectedLevel";
    private const string LevelUnlockedPrefix = "UnlockedLevel_";

    // Get or set the currently selected level (default is level 1)
    public static int SelectedLevel
    {
        get => PlayerPrefs.GetInt(SelectedLevelKey, 1);
        set
        {
            PlayerPrefs.SetInt(SelectedLevelKey, value);
            PlayerPrefs.Save();
        }
    }

    // Unlock a specific level
    public static void UnlockLevel(int levelNumber)
    {
        PlayerPrefs.SetInt(LevelUnlockedPrefix + levelNumber, 1);
        PlayerPrefs.Save();
    }

    // Check if a specific level is unlocked
    public static bool IsLevelUnlocked(int levelNumber)
    {
        return PlayerPrefs.GetInt(LevelUnlockedPrefix + levelNumber, levelNumber == 1 ? 1 : 0) == 1;
    }
}
