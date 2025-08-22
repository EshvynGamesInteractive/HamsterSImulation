using DG.Tweening;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] GameObject cutsceneTOPlay, particle;
    [SerializeField] float cutsceneDuration = 5;
    [SerializeField] Transform placementPos;
    [SerializeField] int taskNumber = 2;
    [SerializeField] string txtTOShow = "Pick a book to bury here";

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerScript>(out PlayerScript player))
        {
            if (player.pickedObject == null)
            {
                MainScript.instance.pnlInfo.ShowInfo(txtTOShow);
                return;
            }

            MainScript.instance.grandPa.StopTheChase();

            Pickable itemTODrop = player.pickedObject;

            GetComponent<Collider>().enabled = false;
            if (placementPos != null)
                player.PlaceObject(placementPos.position);
            MainScript.instance.HideIndication();
            itemTODrop.DisableForInteraction(false);


            itemTODrop.gameObject.SetActive(false);


            placementPos.gameObject.SetActive(true);

            player.DisablePlayer();
            //Debug.Log(cutsceneTOPlay + "ssssssssssssssssssssssssssssssssssssssssssss");
            cutsceneTOPlay.SetActive(true);
            if (particle != null)
                particle.SetActive(false);
            MainScript.instance.RestartRewardedTimer();
            DOVirtual.DelayedCall(cutsceneDuration, () =>
            {
                cutsceneTOPlay.SetActive(false);
                player.EnablePlayer();

                placementPos.gameObject.SetActive(false);

                if (taskNumber == -1) // means no need to increment task for level. just continue the ongoing task
                {
                    MainScript.instance.activeLevel.UpdateTask(MainScript.instance.activeLevel.GetCurrentStageCompletedTaskNumber());
                    return;
                }

                MainScript.instance.activeLevel.TaskCompleted(taskNumber);
            });
        }
    }


    private void OnEnable()
    {
        MainScript.instance.HideIndication();
    }

    private void OnDisable()
    {
        MainScript.instance.ShowIndication();
    }
}