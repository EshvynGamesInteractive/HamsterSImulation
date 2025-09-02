using UnityEngine;
using DG.Tweening;

public class Level5Script : LevelScript
{
    [SerializeField] Transform sittingPos, trapPos;

    [SerializeField] private int itemsToBreak, paintingsToTilt, balloonsToBurst, cushionsToThrow;
    // [SerializeField] GameObject trapTimeline;
    // [SerializeField] float trapTimelineDuration;

    private void Awake()
    {
        // Invoke(nameof(GrandpaSitOnBed), 0.2f);
    }

    private void Start()
    {
        grandpa.StopTheChase();
    }

    private new void OnEnable()
    {
        base.OnEnable();
        if (GetCurrentStageCompletedTaskNumber() < 0)
            SetCurrentStageTaskNumber(0);
        MainScript.instance.UpdateLevelText(GetCurrentStageUnlockedLevels());
        // DOVirtual.DelayedCall(1, () => { UpdateTask(GetCurrentStageCompletedTaskNumber()); });
        UpdateTask(GetCurrentStageCompletedTaskNumber());
    }

    private void GrandpaSitOnBed()
    {
        grandpa.isSitting = false;
        grandpa.MakeGrandpaSit(sittingPos);
    }

    public override void TaskCompleted(int taskNumber)
    {
        Debug.Log(taskNumber);
        if (taskNumber < GetCurrentStageCompletedTaskNumber())
            return;

        if (taskNumber >= tasks.Length)
        {
            player.DisablePlayer();
            MainScript.instance.AllTasksCompleted();
        }
        else
        {
            Debug.Log(items[taskNumber].name);


            float taskUpdateDelay = 0;

            if (taskNumber >= 1)
            {
                grandpa.isSitting = false;
            }
            else
            {
                GrandpaSitOnBed();
                DOVirtual.DelayedCall(1, () => GrandpaSitOnBed());
            }

            if (taskNumber == 1) //when place mousetrap
            {
                grandpa.StandInjured();
                grandpa.transform.SetPositionAndRotation(trapPos.position, trapPos.rotation);
                grandpa.ChasePlayerForDuration(2);
                // trapTimeline.SetActive(true);
                // DOVirtual.DelayedCall(trapTimelineDuration, () =>
                // {
                //     Typewriter.instance.StartTyping(
                //         "What the!? Water on the floor?! Dog! I am gonna slip and break something!", 2);
                //     trapTimeline.SetActive(false);
                //     player.EnablePlayer();
                //
                // });
            }

            if (taskNumber == 2) // when sleep
                grandpa.StopTheChase();

            if (taskNumber == 3) // 
            {
                grandpa.StopTheChase();
                DOVirtual.DelayedCall(5, () => { grandpa.ChasePlayerForDuration(2); });
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


    public override void UpdateTask(int taskNumber)
    {
        if (taskNumber < GetCurrentStageCompletedTaskNumber())
            return;


        float taskUpdateDelay = 0;

        if (taskNumber >= 1)
        {
            grandpa.isSitting = false;
        }
        else
        {
            GrandpaSitOnBed();
            DOVirtual.DelayedCall(1, () => GrandpaSitOnBed());
        }

        if (taskNumber == 1) //when bury cloth
            grandpa.StopTheChase();

        // if (taskNumber == 2) // when topple dishes
        //     grandpa.ChasePlayerForDuration(2);

        if (taskNumber == 3) // when licking cutlery
        {
            grandpa.StopTheChase();
            // if (Nicko_ADSManager._Instance)
            //
            //     Nicko_ADSManager._Instance.ShowInterstitial("LickCutlery");
            // DOVirtual.DelayedCall(5, () => { grandpa.ChasePlayerForDuration(2); });
        }

        DOVirtual.DelayedCall(taskUpdateDelay, () =>
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

    public override void StartNextLevel()
    {
        OnEnable();
    }

    public override void MiniGameEnded()
    {
    }

    private int brokenCounter = 0;
    private int paintingsCounter = 0;
    private int balloonCounter = 0;
    private int cushionCounter = 0;

    public void ItemBroken()
    {
        if (GetCurrentStageCompletedTaskNumber() != 2)
            return;
        brokenCounter++;
        if (brokenCounter >= itemsToBreak)
        {
            TaskCompleted(3);
        }
    }


    public void PaintingTilted()
    {
        if (GetCurrentStageCompletedTaskNumber() != 4)
            return;
        paintingsCounter++;
        if (paintingsCounter >= paintingsToTilt)
        {
            TaskCompleted(5);
        }
        MainScript.instance.HideIndication();
    }

    public void BalloonBurst()
    {
        if (GetCurrentStageCompletedTaskNumber() != 5)
            return;
        MainScript.instance.HideIndication();
        balloonCounter++;
        if (balloonCounter >= balloonsToBurst)
        {
            TaskCompleted(6);
        }
    }

    public void PillowThrown()
    {
        if (GetCurrentStageCompletedTaskNumber() != 6)
            return;
        cushionCounter++;
        if (cushionCounter >= cushionsToThrow)
        {
            TaskCompleted(7);
        }

        MainScript.instance.HideIndication();
    }

    public void BasketScored()
    {
        if (GetCurrentStageCompletedTaskNumber() != 3)
            return;

        TaskCompleted(4);
    }
}