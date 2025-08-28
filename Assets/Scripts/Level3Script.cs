using DG.Tweening;
using UnityEngine;

public class Level3Script : LevelScript
{
    [SerializeField] Transform dinnerPos;
    [SerializeField] GameObject balloonTimeline;
    [SerializeField] float balloonTimelineDuration;
    [SerializeField] GameObject tableCloth;

    private void Awake()
    {
        Invoke(nameof(GrandpaSitForDinner), 0.2f);
    }

    private void Start()
    {
        //base.Start();
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

    private void GrandpaSitForDinner()
    {
        grandpa.MakeGrandpaSit(dinnerPos);
    }

    public override void TaskCompleted(int taskNumber)
    {
        Debug.Log(taskNumber);
        if (taskNumber < GetCurrentStageCompletedTaskNumber())
            return;

        if (taskNumber >= tasks.Length)
        {
            balloonTimeline.SetActive(true);
            player.DisablePlayer();
            DOVirtual.DelayedCall(balloonTimelineDuration, () =>
            {
                Typewriter.instance.StartTyping(
                    "What the!? Water on the floor?! Dog! I am gonna slip and break something!", 2);
                balloonTimeline.SetActive(false);
                player.EnablePlayer();

              

              
               
                MainScript.instance.AllTasksCompleted();
            });
        }
        else
        {
            Debug.Log(items[taskNumber].name);


            float taskUpdateDelay = 0;

            if (taskNumber >= 1)
            {
                grandpa.isSitting = false;
                tableCloth.SetActive(false);
                //DOVirtual.DelayedCall(1, () =>
                //{
                //    grandpa.ChasePlayerForDuration(1);

                //});
            }

            if (taskNumber == 1) //when bury cloth
                grandpa.StopTheChase();

            if (taskNumber == 2) // when topple dishes
                grandpa.ChasePlayerForDuration(2);

            if (taskNumber == 3) // when licking cutlery
            {
                grandpa.StopTheChase();
                if (Nicko_ADSManager._Instance)

                    Nicko_ADSManager._Instance.ShowInterstitial("LickCutlery");
                DOVirtual.DelayedCall(5, () => { grandpa.ChasePlayerForDuration(2); });
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
        Debug.Log(taskNumber);
        if (taskNumber < GetCurrentStageCompletedTaskNumber())
            return;


        float taskUpdateDelay = 0;

        if (taskNumber >= 1)
        {
            grandpa.isSitting = false;
            tableCloth.SetActive(false);
        }

        if (taskNumber == 1) //when bury cloth
            grandpa.StopTheChase();

        if (taskNumber == 2) // when topple dishes
            grandpa.ChasePlayerForDuration(2);

        if (taskNumber == 3) // when licking cutlery
        {
            grandpa.StopTheChase();
            if (Nicko_ADSManager._Instance)

                Nicko_ADSManager._Instance.ShowInterstitial("LickCutlery");
            DOVirtual.DelayedCall(5, () => { grandpa.ChasePlayerForDuration(2); });
        }

        DOVirtual.DelayedCall(taskUpdateDelay, () =>
        {
            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber], taskIcons[taskNumber]);
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