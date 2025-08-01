using UnityEngine;

public class Level1Script : LevelScript
{
    [SerializeField] Transform grandpaTvPos, drinkTeaPos;
    [SerializeField] GameObject levelCompleteCutscene;
    private bool watchingTV, drinkingTea;
    private void MakeGrandpaWatchTV()
    {
        watchingTV = true;
        grandpa.MakeGrandpaSit(grandpaTvPos);

    }
    private void MakeGrandpaDrinkTea()
    {
        drinkingTea = true;
        grandpa.MakeGrandpaSit(drinkTeaPos);
    }


    public override void TaskCompleted(int taskNumber)
    {
        if (taskNumber >= tasks.Length)
        {
            if (levelCompleteCutscene != null)
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
            else
                watchingTV = false;

            if (taskNumber == 3 || taskNumber == 5)    // when turn tv off, or pour ketchup
                grandpa.StartTheChase();

            if (taskNumber == 4)    // when toss blanket in tub
                MakeGrandpaDrinkTea();
            else
                drinkingTea = true;
        }



   

    }

   

    public override void MiniGameEnded()
    {
        Debug.Log("watchingtv " + watchingTV);
        if (drinkingTea)
            MakeGrandpaDrinkTea();
        if (watchingTV)
            MakeGrandpaWatchTV();
    }




}
