using DG.Tweening;
using UnityEngine;

public class Level4Script : LevelScript
{
    [SerializeField] GameObject  squirrelCutscene;
    [SerializeField] Transform sitPos;
    [SerializeField] float cutsceneDuration = 4;
    [SerializeField] Transform cutsceneCamera;
    [SerializeField] Transform cat;
    [SerializeField] SquirrelAI squirrel;
    [SerializeField] GameObject catTimeline, hensTimeline;
    [SerializeField] float catTimelineDuration, hensTimelineDuration;
    [SerializeField] GameObject squirrelCage;
    private bool isSittingForSquirrel = true;


    private void Start()
    {
        //DOVirtual.DelayedCall(1, () =>
        //    {

        //        grandpa.MakeGrandpaSit(sitPos);
        //    });


        //grandpa.StopTheChase();
        MakeGrandpaSitForSquirrel();
    }

    private void MakeGrandpaSitForSquirrel()
    {
        grandpa.MakeGrandpaSit(sitPos);
        DOVirtual.DelayedCall(1, () => { grandpa.MakeGrandpaSit(sitPos); });

        //TaskCompleted(MainScript.currentTaskNumber);
        grandpa.StopTheChase();
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
        if (taskNumber >= tasks.Length)
        {
            if (levelCompleteCutscene != null)
                levelCompleteCutscene.SetActive(true);
          
            MainScript.instance.AllTasksCompleted();
            player.DisablePlayer();
        }
        else
        {
            //if (taskNumber == 0)
            //{
            //    //DOVirtual.DelayedCall(1, () => MakeGrandpaSitForSquirrel());

            //    //isSittingForSquirrel = true;
            //}
            //else
            //{
            //    //isSittingForSquirrel = false;
            //    //grandpa.isSitting = false;
            //}

            float taskUpdateDelay = 0;


            if (taskNumber > 1)
            {
                isSittingForSquirrel = false;
                grandpa.isSitting = false;
                squirrelCage.SetActive(false);
            }
            else
            {
                DOVirtual.DelayedCall(1, () => MakeGrandpaSitForSquirrel());

                isSittingForSquirrel = true;
            }

            //if (taskNumber == 1)
            //    grandpa.StopTheChase();

            if (taskNumber == 2) // when release squirrel
            {
                PlaySquirrelCutscene();
                taskUpdateDelay = cutsceneDuration;
            }

            if (taskNumber == 3) // when throw cat at sock
            {
                if (catTimelineDuration > 0)
                {
                    catTimeline.SetActive(true);
                    player.DisablePlayer();

                    DOVirtual.DelayedCall(catTimelineDuration, () =>
                    {
                        catTimeline.SetActive(false);
                        player.EnablePlayer();
                        grandpa.ChasePlayerForDuration(2);
                        catTimelineDuration = 0; //so it only runs the cutscene once in one scene load
                    });
                }

                grandpa.StopTheChase();
            }

            if (taskNumber == 4) // when scare hens
            {
                if (hensTimelineDuration > 0)
                {
                    Typewriter.instance.StartTyping("What's all that clucking?! Dog, stop bothering the hens!", 4);
                    hensTimeline.SetActive(true);
                    player.DisablePlayer();
                    DOVirtual.DelayedCall(hensTimelineDuration, () =>
                    {
                        hensTimeline.SetActive(false);
                        player.EnablePlayer();
                        grandpa.ChasePlayerForDuration(2);
                        hensTimelineDuration = 0; //so it only runs the cutscene once in one scene load
                    });
                }
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
                    items[taskNumber].EnableForInteraction(true);
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


        if (taskNumber > 1)
        {
            isSittingForSquirrel = false;
            grandpa.isSitting = false;
            squirrelCage.SetActive(false);
        }
        else
        {
            DOVirtual.DelayedCall(1, () => MakeGrandpaSitForSquirrel());

            isSittingForSquirrel = true;
        }


        if (taskNumber == 2) // when release squirrel
        {
            grandpa.transform.SetPositionAndRotation(sitPos.position, sitPos.rotation);
            grandpa.gameObject.SetActive(true);
            grandpa.isSitting = false;
            grandpa.ChasePlayerForDuration(2);
        }

        if (taskNumber == 3) // when throw cat at sock
        {
            grandpa.ChasePlayerForDuration(2);
        }

        if (taskNumber == 4) // when scare hens
        {
            grandpa.ChasePlayerForDuration(2);
        }


        DOVirtual.DelayedCall(taskUpdateDelay, () =>
        {
           
            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber], taskIcons[taskNumber]);
        });
        SetCurrentStageTaskNumber(taskNumber);
        int currentLevelTasks = eachLevelTasksCount[GetCurrentStageUnlockedLevels() - 1];
        Debug.Log(currentLevelTasks);
        Debug.Log(GetCurrentLevelCompletedTaskNumber());

        MainScript.instance.TaskCompleted(GetCurrentLevelCompletedTaskNumber(), currentLevelTasks);
    }


    public override void StartNextLevel()
    {
        SetCurrentLevelCompletedTaskNumber(0);

        OnEnable();
    }


    public override void MiniGameEnded()
    {
        if (isSittingForSquirrel)
            MakeGrandpaSitForSquirrel();
    }


    public void PlaySquirrelCutscene()
    {
        squirrelCutscene.SetActive(true);
        player.DisablePlayer();
        grandpa.gameObject.SetActive(false);
        DOVirtual.DelayedCall(cutsceneDuration, () =>
        {
            cutsceneCamera.DOMove(player.playerCamera.position, 0.3f);
            cutsceneCamera.DORotate(player.playerCamera.eulerAngles, 0.1f).SetDelay(0.2f).OnComplete(() =>
            {
                player.EnablePlayer();
                grandpa.transform.SetPositionAndRotation(sitPos.position, sitPos.rotation);
                grandpa.gameObject.SetActive(true);
                grandpa.isSitting = false;
                grandpa.ChasePlayerForDuration(2);
                squirrelCutscene.SetActive(false);
                squirrel.RunTowardsCage();
            });
        });
    }
}