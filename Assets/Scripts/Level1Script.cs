using DG.Tweening;
using UnityEngine;

public class Level1Script : LevelScript
{
    [SerializeField] Transform grandpaTvPos, drinkTeaPos;
    [SerializeField] GameObject levelCompleteCutscene;
    [SerializeField] GameObject televisionTimeline, ketchupTimeline;
    [SerializeField] float tvTimelineDuration, ketchupTimelineDuration;
    [SerializeField] GameObject tv;
    private bool watchingTV, drinkingTea;
    [SerializeField] GameObject bucket;

    private new void Start()
    {
        base.Start();
        TaskCompleted(MainScript.currentTaskNumber);
        grandpa.StopTheChase();
        MainScript.instance.pnlInfo.ShowInfo("Player spawned");
    }

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
        if (taskNumber < MainScript.currentTaskNumber)
            return;
        if (taskNumber >= tasks.Length)
        {
            if (levelCompleteCutscene != null)
                levelCompleteCutscene.SetActive(true);
            grandpa.StopTheChase();
            MainScript.instance.AllTasksCompleted();
        }
        else
        {
            if (taskNumber == 0)
                bucket.GetComponent<Interactable>().EnableForInteraction(true);
            else
                bucket.SetActive(false);

            Debug.Log(items[taskNumber].name);
            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);

            if (taskNumber == 2)   //when bury book
                MakeGrandpaWatchTV();
            else
                watchingTV = false;

            if (taskNumber == 3)    // when turn tv off
            {
                televisionTimeline.SetActive(true);
                player.DisablePlayer();
                DOVirtual.DelayedCall(tvTimelineDuration, () =>
                {
                    televisionTimeline.SetActive(false);
                    player.EnablePlayer();
                    DOVirtual.DelayedCall(4, () =>
                    {
                        grandpa.ChasePlayerForDuration(30);
                    });

                });
            }

            if (taskNumber == 4)    // when toss blanket in tub
                MakeGrandpaDrinkTea();
            else
                drinkingTea = true;

            if (taskNumber == 5)    // pour ketchup
            {
                ketchupTimeline.SetActive(true);
                player.DisablePlayer();
                DOVirtual.DelayedCall(ketchupTimelineDuration, () =>
                {
                    ketchupTimeline.SetActive(false);
                    player.EnablePlayer();
                    DOVirtual.DelayedCall(4, () =>
                    {
                        grandpa.ChasePlayerForDuration(30);
                    });

                });
            }


            MainScript.currentTaskNumber = taskNumber;
        }





    }



    public override void MiniGameEnded()
    {
        if (drinkingTea)
            MakeGrandpaDrinkTea();
        if (watchingTV)
            MakeGrandpaWatchTV();
    }




}
