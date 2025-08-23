using DG.Tweening;
using UnityEngine;

public class Level2Script : LevelScript
{
    [SerializeField] GameObject levelCompleteCutscene;
    [SerializeField] GameObject shutDoorTimeline, toyThrowTimeline;
    [SerializeField] float shutDoorTimelineDuration, throwTimelineDuration;
    [SerializeField] LaundaryDoor laundaryDoor;

    private void Start()
    {
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


    public override void TaskCompleted(int taskNumber)
    {
        if (taskNumber < GetCurrentStageCompletedTaskNumber())
            return;
        Debug.Log(taskNumber);
        if (taskNumber >= tasks.Length)
        {
            if (levelCompleteCutscene != null)
                levelCompleteCutscene.SetActive(true);
           
            MainScript.instance.AllTasksCompleted();
        }
        else
        {
            float taskUpdateDelay = 0;

            Debug.Log(taskNumber);
            if (taskNumber == 1) //throw toy
            {
                if (throwTimelineDuration > 0)
                {
                    Typewriter.instance.StartTyping("Oww! Hey! That hurt! Waaah!", 1);
                    taskUpdateDelay = throwTimelineDuration;
                    toyThrowTimeline.SetActive(true);
                    player.DisablePlayer();
                    DOVirtual.DelayedCall(throwTimelineDuration, () =>
                    {
                        toyThrowTimeline.SetActive(false);
                        player.EnablePlayer();
                        DOVirtual.DelayedCall(2, () => { grandpa.ChasePlayerForDuration(30); });
                        throwTimelineDuration = 0; //so it only runs the cutscene once in one scene load
                    });
                }
            }

            if (taskNumber == 2) //when salt pour
            {
                //saltTimeline.SetActive(true);
                //player.DisablePlayer();
                //DOVirtual.DelayedCall(saltTimelineDuration, () =>
                //{
                //saltTimeline.SetActive(false);
                //player.EnablePlayer();
                grandpa.ChasePlayerForDuration(2);

                //});
            }

            if (taskNumber == 3) //when tablet hide
            {
                grandpa.ChasePlayerForDuration(2);
            }

            if (taskNumber == 4) // when door shut
            {
                if (shutDoorTimelineDuration > 0)
                {
                    if (Nicko_ADSManager._Instance)
                        Nicko_ADSManager._Instance.ShowInterstitial("LockKids");
                    Typewriter.instance.StartTyping("Waaah! We're locked in! Somebody help!", 1);
                    shutDoorTimeline.SetActive(true);
                    player.DisablePlayer();
                    taskUpdateDelay = shutDoorTimelineDuration;
                    DOVirtual.DelayedCall(shutDoorTimelineDuration, () =>
                    {
                        shutDoorTimeline.SetActive(false);
                        player.EnablePlayer();
                        grandpa.ChasePlayerForDuration(2);
                        shutDoorTimelineDuration = 0; //so it only runs the cutscene once in one scene load
                    });
                }
            }

            if (taskNumber > 4)
            {
                laundaryDoor.kidsLocked = true;
                laundaryDoor.CloseDoor();
                laundaryDoor.EnableForInteraction(false);
            }


            DOVirtual.DelayedCall(taskUpdateDelay, () =>
            {
                SetCurrentLevelCompletedTaskNumber(GetCurrentLevelCompletedTaskNumber()+1);


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
                    Debug.Log((tasks[taskNumber]));
                    items[taskNumber].EnableForInteraction(true);
                    MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);
                }
            });
        }
    }


    public override void UpdateTask(int taskNumber)
    {
        if (taskNumber < GetCurrentStageCompletedTaskNumber())
            return;
        Debug.Log(taskNumber);

        Debug.Log(items[taskNumber].name);

        float taskUpdateDelay = 0;


        if (taskNumber == 1) //throw toy
        {
            grandpa.ChasePlayerForDuration(2);
        }

        if (taskNumber == 2) //when salt pour
        {
            grandpa.ChasePlayerForDuration(2);
        }

        if (taskNumber == 3) //when tablet hide
        {
            grandpa.ChasePlayerForDuration(2);
        }

        if (taskNumber == 4) // when door shut
        {
            grandpa.ChasePlayerForDuration(2);
        }

        if (taskNumber > 4)
        {
            laundaryDoor.kidsLocked = true;
            laundaryDoor.CloseDoor();
            laundaryDoor.EnableForInteraction(false);
        }


        DOVirtual.DelayedCall(taskUpdateDelay, () =>
        {
            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);
        });


        SetCurrentStageTaskNumber(taskNumber);
        int currentLevelTasks = eachLevelTasksCount[GetCurrentStageUnlockedLevels()-1];

        Debug.Log(currentLevelTasks);
        Debug.Log(GetCurrentLevelCompletedTaskNumber());
        MainScript.instance.TaskCompleted(GetCurrentLevelCompletedTaskNumber(), currentLevelTasks);
    }

    public override void StartNextLevel()
    {
       

        OnEnable();
    }

    public override void MiniGameEnded()
    {
    }
}