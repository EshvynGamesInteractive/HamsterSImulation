using DG.Tweening;
using UnityEngine;

public class Level3Script : LevelScript
{
    [SerializeField] GameObject levelCompleteCutscene;
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
        if (MainScript.currentTaskNumber < 0)
            MainScript.currentTaskNumber = 0;

        DOVirtual.DelayedCall(1, () => { TaskCompleted(MainScript.currentTaskNumber); });
    }

    private void GrandpaSitForDinner()
    {
        grandpa.MakeGrandpaSit(dinnerPos);
    }

    public override void TaskCompleted(int taskNumber)
    {
        Debug.Log(taskNumber);
        if (taskNumber < MainScript.currentTaskNumber)
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

                if (levelCompleteCutscene != null)
                    levelCompleteCutscene.SetActive(true);
                grandpa.StopTheChase();
                MainScript.instance.AllTasksCompleted();
            });
        }
        else
        {
            Debug.Log(items[taskNumber].name);

            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);

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
                grandpa.ChasePlayerForDuration(30);

            if (taskNumber == 3) // when licking cutlery
            {
                grandpa.StopTheChase();
                if (Nicko_ADSManager._Instance)

                    Nicko_ADSManager._Instance.ShowInterstitial("LickCutlery");
                DOVirtual.DelayedCall(5, () => { grandpa.ChasePlayerForDuration(30); });
            }
            MainScript.instance.TaskCompleted(taskNumber, tasks.Length);
            MainScript.currentTaskNumber = taskNumber;
        }
    }

    public override void MiniGameEnded()
    {
    }
}