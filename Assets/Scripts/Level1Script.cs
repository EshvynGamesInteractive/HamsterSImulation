using DG.Tweening;
using UnityEngine;

public class Level1Script : LevelScript
{
    [SerializeField] Transform grandpaTvPos, drinkTeaPos;
    [SerializeField] GameObject levelCompleteCutscene;
    [SerializeField] GameObject televisionTimeline, ketchupTimeline;
    [SerializeField] float tvTimelineDuration, ketchupTimelineDuration;
    [SerializeField] GameObject tvScreen;
    private bool watchingTV, drinkingTea;
    [SerializeField] GameObject bucket, spilledWater;

    private void Start()
    {
        //base.Start();
        //TaskCompleted(MainScript.currentTaskNumber);
        grandpa.StopTheChase();
        MainScript.instance.pnlInfo.ShowInfo("Player spawned");
    }

    private new void OnEnable()
    {
        base.OnEnable();
        if (MainScript.currentTaskNumber < 0)
            MainScript.currentTaskNumber = 0;

        DOVirtual.DelayedCall(1, () => { TaskCompleted(MainScript.currentTaskNumber); });
    }

    private void MakeGrandpaWatchTV()
    {
        watchingTV = true;
        MainScript.instance.grandPa.MakeGrandpaSit(grandpaTvPos);
    }

    private void MakeGrandpaDrinkTea()
    {
        drinkingTea = true;
        MainScript.instance.grandPa.MakeGrandpaSit(drinkTeaPos);
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
            {
                bucket.GetComponent<Interactable>().EnableForInteraction(true);
            }
            else
            {
                bucket.SetActive(false);
                spilledWater.SetActive(false);
            }

            float waitDuration = 1;

            //Debug.Log(items[taskNumber].name);
            //items[taskNumber].EnableForInteraction(true);
            //MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);
            if (taskNumber == 1)
            {
                // grandpa.isSitting = false;
                DOVirtual.DelayedCall(3, () => grandpa.ChasePlayerForDuration(30));
            }

            if (taskNumber == 2) //when bury book
            {
                MakeGrandpaWatchTV();
                DOVirtual.DelayedCall(1, () => MakeGrandpaWatchTV());
            }
            else
                watchingTV = false;

            if (taskNumber == 3) // when turn tv off
            {
                grandpa.isSitting = false;
                if (tvTimelineDuration > 0)
                {
                    Typewriter.instance.StartTyping(
                        "Hey! Who turned off my TV? I was watching that! Dog? Was that you again? Come back here, you little rascal!",
                        2);
                    waitDuration = tvTimelineDuration;
                    televisionTimeline.SetActive(true);
                    player.DisablePlayer();
                    DOVirtual.DelayedCall(tvTimelineDuration, () =>
                    {
                        tvScreen.SetActive(false);
                        televisionTimeline.SetActive(false);
                        player.EnablePlayer();
                        DOVirtual.DelayedCall(4, () => { grandpa.ChasePlayerForDuration(30); });

                        tvTimelineDuration = 0; //so it only runs the cutscene once in one scene load
                    });
                }
            }

            if (taskNumber == 4) // when toss blanket in tub
            {
                MakeGrandpaDrinkTea();
                DOVirtual.DelayedCall(1, () => MakeGrandpaDrinkTea());
            }
            else
                drinkingTea = false;

            if (taskNumber == 5) // pour ketchup
            {
                grandpa.isSitting = false;
                if (ketchupTimelineDuration > 0)
                {
                    Typewriter.instance.StartTyping(
                        "Blegh! What in tarnation, this ain't tea! Who put ketchup in my cup?! Dog?!", 3);
                    waitDuration = ketchupTimelineDuration;
                    ketchupTimeline.SetActive(true);
                    player.DisablePlayer();
                    DOVirtual.DelayedCall(ketchupTimelineDuration, () =>
                    {
                        ketchupTimeline.SetActive(false);
                        player.EnablePlayer();
                        DOVirtual.DelayedCall(3, () => { grandpa.ChasePlayerForDuration(30); });
                        ketchupTimelineDuration = 0; //so it only runs the cutscene once in one scene load
                    });
                }
            }


            DOVirtual.DelayedCall(waitDuration, () =>
            {
                //Debug.Log(items[taskNumber].name);
                items[taskNumber].EnableForInteraction(true);
                MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);
            });
                MainScript.currentTaskNumber = taskNumber;
          MainScript.instance.TaskCompleted(taskNumber, tasks.Length);
        }
    }


    public override void MiniGameEnded()
    {
        miniGameisActive = false;
        if (drinkingTea)
            MakeGrandpaDrinkTea();
        if (watchingTV)
            MakeGrandpaWatchTV();
    }
}