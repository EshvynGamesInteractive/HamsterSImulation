using UnityEngine;

public class Level3Script : LevelScript
{

    [SerializeField] GameObject levelCompleteCutscene;
    [SerializeField] Transform dinnerPos;

    private void Awake()
    {
        Invoke(nameof(GrandpaSitForDinner), 0.2f);
    }

    private void GrandpaSitForDinner()
    {
        grandpa.MakeGrandpaSit(dinnerPos);
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


            if (taskNumber == 1)
                grandpa.StopTheChase();

            if (taskNumber == 2)   // when topple dishes
                grandpa.StopTheChase();


        }

    }
}
