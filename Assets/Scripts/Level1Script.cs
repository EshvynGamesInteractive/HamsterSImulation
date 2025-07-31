using UnityEngine;

public class Level1Script : LevelScript
{
    [SerializeField] Transform grandpaTvPos, drinkTeaPos;
    [SerializeField] GameObject levelCompleteCutscene;
    private void MakeGrandpaWatchTV()
    {
        grandpa.MakeGrandpaSit(grandpaTvPos);

    }
    private void MakeGrandpaDrinkTea()
    {
        grandpa.MakeGrandpaSit(drinkTeaPos);
    }


    public override void TaskCompleted(int taskNumber)
    {
        if (taskNumber >= tasks.Length)
        {
            if(levelCompleteCutscene != null)
                levelCompleteCutscene.SetActive(true);
            grandpa.StopTheChase();
            MainScript.instance.AllTasksCompleted();
        }
        else
        {
            Debug.Log(items[taskNumber].name);

            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);

            if (taskNumber == 2)   //when bury book
                MakeGrandpaWatchTV();

            if (taskNumber == 3 || taskNumber == 5)    // when turn tv off, or pour ketchup
                grandpa.StartTheChase();

            if (taskNumber == 4)    // when toss blanket in tub
                MakeGrandpaDrinkTea();
        }

    }
}
