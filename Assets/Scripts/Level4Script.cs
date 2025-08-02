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
    private new void Start()
    {
        base.Start();
        DOVirtual.DelayedCall(1, () =>
            {

                grandpa.MakeGrandpaSit(sitPos);
            });
    }
    public override void TaskCompleted(int taskNumber)
    {
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


            //if (taskNumber == 1)
            //    grandpa.StopTheChase();

            if (taskNumber == 2)   // when release squirrel
                PlaySquirrelCutscene();

            if (taskNumber == 3)   // when throw cat at sock
            {
                grandpa.StopTheChase();

            }

            if (taskNumber == 4)   // when scare hens
            {
                grandpa.StartTheChase();

            }

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
                grandpa.StartTheChase();
                squirrelCutscene.SetActive(false);
                squirrel.RunTowardsCage();
            });

        });
    }
}
