using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour
{
    public int totalTasks = 3;
    private int completedTasks = 0;
    public System.Action onLevelComplete;
    public void CompleteTask()
    {
        completedTasks++;
        Debug.Log($"[Level] Task {completedTasks}/{totalTasks}");
        if (completedTasks >= totalTasks)
        {
            Debug.Log("[Level] All tasks complete!");
            onLevelComplete?.Invoke();
        }
    }
}
