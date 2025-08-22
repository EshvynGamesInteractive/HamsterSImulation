using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class LevelScript : MonoBehaviour
{
    [SerializeField] protected Interactable[] items;
    [SerializeField] protected string[] tasks;
    [SerializeField] protected int[] eachLevelTasksCount;
    // [SerializeField] protected int currentLevelCompletedTasks;
    protected bool miniGameisActive;
    protected GrandpaAI grandpa;
    protected PlayerScript player;

    // private void Awake()
    // {
    //     // int unlockedLevels = GetCurrentStageUnlockedLevels();
    //     // SetCurrentStageTaskNumber(eachLevelTasksCount[unlockedLevels-1]);
    //   //  currentLevelCompletedTasks = 0; //so that the level starts from beginning
    // }

    // protected static int currentLevelCompletedTasks
    // {
    //     get { return PlayerPrefs.GetInt("currentLevelCompletedTasks", 0); }
    //     set { PlayerPrefs.SetInt("currentLevelCompletedTasks", value); }
    // }
    protected void OnEnable()
    {
        grandpa = MainScript.instance.grandPa;
        player = MainScript.instance.player;
    }
    public abstract void StartNextLevel();

    public abstract void TaskCompleted(int taskNumber);
    public abstract void UpdateTask(int taskNumber);
    public abstract void MiniGameEnded();

    public void MiniGameStarted()
    {
        miniGameisActive = true;
    }

    public int GetCurrentStageUnlockedLevels() //for storing unlocked levels for each stage
    {
        return PlayerPrefs.GetInt("UnlockedLevels"+gameObject.name + transform.GetSiblingIndex(), 1);
    }

    public void SetCurrentStageUnlockedLevels(int level)
    {
        PlayerPrefs.SetInt("UnlockedLevels"+gameObject.name + transform.GetSiblingIndex(), level);
    }
    
    public int GetCurrentStageCompletedTaskNumber() //for storing unlocked tasks  for each stage
    {
        Debug.Log(PlayerPrefs.GetInt("StageCompletedTasks"+gameObject.name + transform.GetSiblingIndex()));
        return PlayerPrefs.GetInt("StageCompletedTasks"+gameObject.name + transform.GetSiblingIndex(), 0);
    }

    public void SetCurrentStageTaskNumber(int task)
    {
        PlayerPrefs.SetInt("StageCompletedTasks"+gameObject.name + transform.GetSiblingIndex(), task);
    }
    
    
    public int GetCurrentLevelCompletedTaskNumber() //for storing unlocked tasks of each level for each stage
    {
        Debug.Log("LevelCompletedTasks"+gameObject.name + GetCurrentStageUnlockedLevels());
        return PlayerPrefs.GetInt("LevelCompletedTasks"+gameObject.name + GetCurrentStageUnlockedLevels(), 0);
    }

    public void SetCurrentLevelCompletedTaskNumber(int task)
    {
        PlayerPrefs.SetInt("LevelCompletedTasks"+gameObject.name + GetCurrentStageUnlockedLevels(), task);
    }
    
    
}