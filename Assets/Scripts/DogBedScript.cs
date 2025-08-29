using DG.Tweening;
using UnityEngine;

public class DogBedScript : Interactable
{
    [SerializeField] private GameObject[] sleepingCutscenes;
    [SerializeField] float cutsceneDuration = 5;
    [SerializeField] private Transform playerAwakePos;

    public override void Interact(PlayerScript player)
    {
        DisableForInteraction(true);

        for (int i = 0; i < sleepingCutscenes.Length; i++)
        {
            sleepingCutscenes[i].SetActive(false);
        }
        
        GameObject sleepingCutscene = sleepingCutscenes[GlobalValues.SelectedDogIndex];
        player.DisablePlayer();
        sleepingCutscene.SetActive(true);
        MainScript.instance.HideIndication();
        DOVirtual.DelayedCall(cutsceneDuration, () =>
        {
            sleepingCutscene.SetActive(false);
            player.EnablePlayer(playerAwakePos);
            MainScript.instance.activeLevel.TaskCompleted(2);
        });
    }
}