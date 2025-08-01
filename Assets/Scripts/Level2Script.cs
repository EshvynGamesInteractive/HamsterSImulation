using UnityEngine;

public class Level2Script : LevelScript
{
    [SerializeField] GameObject levelCompleteCutscene;

    private void Awake()
    {
        grandpa.gameObject.SetActive(false);
    }
    public override void TaskCompleted(int taskNumber)
    {
        if (taskNumber >= tasks.Length)
        {
            if (levelCompleteCutscene != null)
                levelCompleteCutscene.SetActive(true);
            //grandpa.StopTheChase();
            MainScript.instance.AllTasksCompleted();
        }
        else
        {
            Debug.Log(items[taskNumber].name);

            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);


            //if (taskNumber == 3 || taskNumber == 5)
            //    grandpa.StartTheChase();

           
        }

    }

    public override void MiniGameEnded()
    {
        throw new System.NotImplementedException();
    }
}
