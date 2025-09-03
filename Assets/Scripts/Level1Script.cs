using DG.Tweening;
using UnityEngine;

public class Level1Script : LevelScript
{
    [SerializeField] Transform grandpaTvPos, drinkTeaPos;
    [SerializeField] GameObject televisionTimeline, ketchupTimeline;
    [SerializeField] float tvTimelineDuration, ketchupTimelineDuration;
    [SerializeField] GameObject tvScreen;
    private bool watchingTV, drinkingTea;
    [SerializeField] GameObject bucket, spilledWater;
    [SerializeField] private GameObject levelCompleteCutscene;

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
        if (GetCurrentStageCompletedTaskNumber() < 0)
            SetCurrentStageTaskNumber(0);
        MainScript.instance.UpdateLevelText(GetCurrentStageUnlockedLevels());
        DOVirtual.DelayedCall(1, () => { UpdateTask(GetCurrentStageCompletedTaskNumber()); });
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
        Debug.Log("TaskCompleted");
        if (taskNumber < GetCurrentStageCompletedTaskNumber())
            return;
        if (taskNumber >= tasks.Length)
        {
            if (levelCompleteCutscene != null)
                levelCompleteCutscene.SetActive(true);


            MainScript.instance.AllTasksCompleted();
        }
        else
        {
            Debug.Log(taskNumber);
            // if (taskNumber == 0)
            // {
            //     bucket.GetComponent<Interactable>().EnableForInteraction(true);
            // }
            // else
            if (taskNumber >= 1)
            {
                bucket.SetActive(false);
                spilledWater.SetActive(false);
            }

            float taskUpdateDelay = 1;


            if (taskNumber == 1)
            {
                grandpa.ChasePlayerForDuration(2);
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
                    taskUpdateDelay = tvTimelineDuration;
                    televisionTimeline.SetActive(true);
                    player.DisablePlayer();
                    DOVirtual.DelayedCall(tvTimelineDuration, () =>
                    {
                        tvScreen.SetActive(false);
                        televisionTimeline.SetActive(false);
                        player.EnablePlayer();
                        grandpa.ChasePlayerForDuration(2);

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
                    taskUpdateDelay = ketchupTimelineDuration;
                    ketchupTimeline.SetActive(true);
                    player.DisablePlayer();
                    DOVirtual.DelayedCall(ketchupTimelineDuration, () =>
                    {
                        ketchupTimeline.SetActive(false);
                        player.EnablePlayer();
                        grandpa.ChasePlayerForDuration(2);
                        ketchupTimelineDuration = 0; //so it only runs the cutscene once in one scene load
                    });
                }
            }


            DOVirtual.DelayedCall(taskUpdateDelay, () =>
            {
                SetCurrentLevelCompletedTaskNumber(GetCurrentLevelCompletedTaskNumber() + 1);


                SetCurrentStageTaskNumber(taskNumber);
                int currentLevelTasks = eachLevelTasksCount[GetCurrentStageUnlockedLevels() - 1];

                Debug.Log(GetCurrentStageUnlockedLevels());
                Debug.Log(currentLevelTasks);
                Debug.Log(GetCurrentLevelCompletedTaskNumber());

                MainScript.instance.TaskCompleted(GetCurrentLevelCompletedTaskNumber(), currentLevelTasks);

                if (GetCurrentLevelCompletedTaskNumber() >= currentLevelTasks)
                {
                    MainScript.instance.CurrentLevelTasksCompleted();
                    SetCurrentStageUnlockedLevels(GetCurrentStageUnlockedLevels() + 1);
                }
                else
                {
                    // items[taskNumber].EnableForInteraction(true);
                    EnableNextItem(taskNumber);
                    MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber], taskIcons[taskNumber]);
                }
            });
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

    public override void StartNextLevel()
    {
        OnEnable();
    }

    public override void UpdateTask(int taskNumber)
    {
        if (taskNumber < GetCurrentStageCompletedTaskNumber())
            return;

        // if (taskNumber == 0)
        // {
        //     bucket.GetComponent<Interactable>().EnableForInteraction(true);
        // }
       
        if (taskNumber >= 1)
        {
            bucket.SetActive(false);
            spilledWater.SetActive(false);
        }
        

        float waitDuration = 1;


        // if (taskNumber == 1) //when grandpa slip
        // {
        //     // grandpa.isSitting = false;
        //     DOVirtual.DelayedCall(3, () => grandpa.ChasePlayerForDuration(2));
        // }

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
        }


        DOVirtual.DelayedCall(waitDuration, () =>
        {
            // items[taskNumber].EnableForInteraction(true);
                EnableNextItem(taskNumber);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber], taskIcons[taskNumber]);
        });

        SetCurrentStageTaskNumber(taskNumber);
        int currentLevelTasks = eachLevelTasksCount[GetCurrentStageUnlockedLevels() - 1];

        // Debug.Log(currentLevelTasks);
        // Debug.Log(GetCurrentLevelCompletedTaskNumber());
        MainScript.instance.TaskCompleted(GetCurrentLevelCompletedTaskNumber(), currentLevelTasks);
    }
}