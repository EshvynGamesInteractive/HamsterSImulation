using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelFactory 
{
    public static GameObject CreateLevel()
    {
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        string path = $"Levels/Level_{selectedLevel}";
        GameObject levelPrefab = Resources.Load<GameObject>(path);
        if (levelPrefab == null)
        {
            Debug.LogError($"Level prefab not found at path: Resources/{path}");
            return null;
        }
        GameObject levelInstance = GameObject.Instantiate(levelPrefab);
        return levelInstance;
    }
}
