using DG.Tweening;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] GameObject cutsceneTOPlay;
    [SerializeField] float cutsceneDuration = 5;
    [SerializeField] Transform placementPos;
    [SerializeField] int taskNumber = 2;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerScript>(out PlayerScript player))
        {
            if (player.pickedObject == null)
            {
                MainScript.instance.pnlInfo.ShowInfo("Pick a book to bury here");
                return;
            }
            Pickable itemTODrop = player.pickedObject;

            GetComponent<Collider>().enabled = false;
            if (placementPos != null)
                player.PlaceObject(placementPos.position);
            MainScript.instance.HideIndication();
            itemTODrop.DisableForInteraction(false);
            player.DisablePlayer();
            cutsceneTOPlay.SetActive(true);
            DOVirtual.DelayedCall(cutsceneDuration, () =>
            {
                cutsceneTOPlay.SetActive(false);
                player.EnablePlayer();
                MainScript.instance.activeLevel.TaskCompleted(taskNumber);
                
            });
        }
    }

   



}
