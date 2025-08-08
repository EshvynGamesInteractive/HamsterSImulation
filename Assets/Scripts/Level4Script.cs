using DG.Tweening;
using UnityEngine;

public class Level4Script : LevelScript
{
    [SerializeField] GameObject levelCompleteCutscene, squirrelCutscene;
    [SerializeField] Transform sitPos;
    [SerializeField] float cutsceneDuration = 4;
    [SerializeField] Transform cutsceneCamera;
    [SerializeField] Transform cat;
    [SerializeField] SquirrelAI squirrel;
    [SerializeField] GameObject catTimeline, hensTimeline;
    [SerializeField] float catTimelineDuration, hensTimelineDuration;
    [SerializeField] GameObject squirrelCage;


    private void Start()
    {
        //base.Start();
        DOVirtual.DelayedCall(1, () =>
            {

                grandpa.MakeGrandpaSit(sitPos);
            });

        //TaskCompleted(MainScript.currentTaskNumber);
        grandpa.StopTheChase();

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
            player.DisablePlayer();
        }
        else
        {

            items[taskNumber].EnableForInteraction(true);
            MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);

            if (taskNumber >1)
                squirrelCage.SetActive(false);

            //if (taskNumber == 1)
            //    grandpa.StopTheChase();

            if (taskNumber == 2)   // when release squirrel
                PlaySquirrelCutscene();

            if (taskNumber == 3)   // when throw cat at sock
            {
                catTimeline.SetActive(true);
                player.DisablePlayer();
                DOVirtual.DelayedCall(catTimelineDuration, () =>
                {
                    catTimeline.SetActive(false);
                    player.EnablePlayer();
                    grandpa.ChasePlayerForDuration(30);

                });
                grandpa.StopTheChase();

            }

            if (taskNumber == 4)   // when scare hens
            {
                Typewriter.instance.StartTyping("What’s all that cluckin’?! Dog, stop botherin’ the hens!", 4);
                hensTimeline.SetActive(true);
                player.DisablePlayer();
                DOVirtual.DelayedCall(hensTimelineDuration, () =>
                {
                    hensTimeline.SetActive(false);
                    player.EnablePlayer();
                    grandpa.ChasePlayerForDuration(30);

                });

            }
            MainScript.currentTaskNumber = taskNumber;
        }

    }

    public override void MiniGameEnded()
    {

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
                grandpa.gameObject.SetActive(true);
                grandpa.ChasePlayerForDuration(30);
                squirrelCutscene.SetActive(false);
                squirrel.RunTowardsCage();
            });

        });
    }
}
