using DG.Tweening;
using UnityEngine;

public class RewardedCutscene : MonoBehaviour
{
    [SerializeField] GameObject[] itemsToHide;
    [SerializeField] GrandpaAI grandpa;
    [SerializeField] bool repositionGrandpaAfterTImeline = false;
    [SerializeField] Transform grandpaPosTOSet;
    [SerializeField] string grandpaDialogue;
    [SerializeField] float dialogueDelay = 3;
    //[SerializeField] ParticleSystem 


    private void OnEnable()
    {
        MainScript.instance.canShowRewardedPopup = false;
        for (int i = 0; i < itemsToHide.Length; i++)
        {
            itemsToHide[i].SetActive(false);
        }
        MainScript.instance.HideIndication();
        grandpa.gameObject.SetActive(false);
        MainScript.instance.activeLevel.transform.parent.gameObject.SetActive(false);
        if (grandpaDialogue != null)
            Typewriter.instance.StartTyping(grandpaDialogue, dialogueDelay);
    }


    private void OnDisable()
    {
        MainScript.instance.canShowRewardedPopup = true;
        MainScript.instance.activeLevel.transform.parent.gameObject.SetActive(true);
        if (grandpa.gameObject)
            grandpa.gameObject.SetActive(true);

        for (int i = 0; i < itemsToHide.Length; i++)
        {
            if (itemsToHide[i] != null)
                itemsToHide[i].SetActive(true);
        }
        MainScript.instance.ShowIndication();
        if (MainScript.instance.grandPa.isSitting) // there is no need to chase player while sitting
            return;
        if (repositionGrandpaAfterTImeline && MainScript.instance.grandPa)
            MainScript.instance.grandPa.transform.SetPositionAndRotation(grandpaPosTOSet.position, grandpaPosTOSet.rotation);

        DOVirtual.DelayedCall(3, () =>
        { grandpa.ChasePlayerForDuration(2); });


    }
}

