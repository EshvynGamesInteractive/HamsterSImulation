using DG.Tweening;
using UnityEngine;

public class Level2Script : LevelScript
{
    [SerializeField] GameObject levelCompleteCutscene;
    [SerializeField] GameObject shutDoorTimeline, saltTimeline, toyThrowTimeline;
    [SerializeField] float shutDoorTimelineDuration, saltTimelineDuration, throwTimelineDuration;
    [SerializeField] LaundaryDoor laundaryDoor;
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

        DOVirtual.DelayedCall(1, () =>
        {
            TaskCompleted(MainScript.currentTaskNumber);
        });
    }
    //private void Awake()
    //{
    //    MainScript.instance.grandPa.gameObject.SetActive(false);
    //}
    public override void TaskCompleted(int taskNumber)
    {
        if (taskNumber < MainScript.currentTaskNumber)
            return;
        Debug.Log(taskNumber);
        if (taskNumber >= tasks.Length)
        {
            if (levelCompleteCutscene != null)
                levelCompleteCutscene.SetActive(true);
            //grandpa.StopTheChase();
            MainScript.instance.AllTasksCompleted();
        }
        else
        {
            Debug.Log(items[taskNumber].name);

            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);


            if (taskNumber == 1)  //throw toy
            {
                Typewriter.instance.StartTyping("Oww! Hey! That hurt! Waaah!", 1);
                
                toyThrowTimeline.SetActive(true);
                player.DisablePlayer();
                DOVirtual.DelayedCall(throwTimelineDuration, () =>
                {
                    toyThrowTimeline.SetActive(false);
                    player.EnablePlayer();
                    DOVirtual.DelayedCall(2, () =>
                    {
                        grandpa.ChasePlayerForDuration(30);
                    });

                });
            }

            if (taskNumber == 2) //when salt pour
            {
                //saltTimeline.SetActive(true);
                //player.DisablePlayer();
                //DOVirtual.DelayedCall(saltTimelineDuration, () =>
                //{
                //saltTimeline.SetActive(false);
                //player.EnablePlayer();
                grandpa.ChasePlayerForDuration(30);

                //});
            }
            if (taskNumber == 3) //when tablet hide
            {
                grandpa.ChasePlayerForDuration(30);
            }
            if (taskNumber == 4)    // when door shut
            {
        if (Nicko_ADSManager._Instance)
                
                Nicko_ADSManager._Instance.ShowInterstitial("LockKids");
                Typewriter.instance.StartTyping("Waaah! We're locked in! Somebody help!", 1);
                shutDoorTimeline.SetActive(true);
                player.DisablePlayer();
                DOVirtual.DelayedCall(shutDoorTimelineDuration, () =>
                {
                    shutDoorTimeline.SetActive(false);
                    player.EnablePlayer();
                    grandpa.ChasePlayerForDuration(30);
                });
            }

            if (taskNumber > 4)
            {
                laundaryDoor.kidsLocked = true;
                laundaryDoor.CloseDoor();
                laundaryDoor.EnableForInteraction(false);
            }


            MainScript.currentTaskNumber = taskNumber;
        }
    }

    public override void MiniGameEnded()
    {

    }
}
