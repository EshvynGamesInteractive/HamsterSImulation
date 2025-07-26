using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField]private LevelBehaviour currentLevelBehaviour;
    [SerializeField]private GameObject levelInstance;
    void Start()
    {
        levelInstance = LevelFactory.CreateLevel();
        if (levelInstance == null)
        {
            Debug.LogError("Level could not be instantiated.");
            return;
        }
        currentLevelBehaviour = levelInstance.GetComponent<LevelBehaviour>();
        if (currentLevelBehaviour == null)
        {
            Debug.LogError("Level prefab missing LevelBehaviour script.");
            return;
        }
        currentLevelBehaviour.onLevelComplete = OnLevelComplete;
    }
    public void CompleteTask()
    {
        currentLevelBehaviour?.CompleteTask();
    }
    private void OnLevelComplete()
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        int nextLevel = currentLevel + 1;
        PlayerPrefs.SetInt("LevelUnlocked_" + nextLevel, 1);
        Debug.Log($"Level {currentLevel} complete! Next level unlocked.");
    }
}
