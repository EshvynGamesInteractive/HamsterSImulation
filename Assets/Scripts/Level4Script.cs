using DG.Tweening;
using UnityEngine;

public class Level4Script : LevelScript
{
    [SerializeField] GameObject levelCompleteCutscene;
    [SerializeField] Transform sitPos;

    private new void Start()
    {
        base.Start();
        DOVirtual.DelayedCall(1, () =>
            {

                grandpa.MakeGrandpaSit(sitPos);
            });
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

            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);


            //if (taskNumber == 1)
            //    grandpa.StopTheChase();

            //if (taskNumber == 2)   // when topple dishes
            //    grandpa.StartTheChase();

            //if (taskNumber == 3)   // when licking cutlery
            //{
            //    grandpa.StopTheChase();

            //}

        }

    }

    public override void MiniGameEnded()
    {
        throw new System.NotImplementedException();
    }
}
