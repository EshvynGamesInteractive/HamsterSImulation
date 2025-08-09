using UnityEngine;

public class TimelineScript : MonoBehaviour
{
    [SerializeField] GameObject[] itemsToHide;



    private void OnEnable()
    {
        MainScript.instance.canShowRewardedPopup = false;
        for (int i = 0; i < itemsToHide.Length; i++)
        {
            itemsToHide[i].SetActive(false);
        }
        MainScript.instance.HideIndication();
    }


    private void OnDisable()
    {
        MainScript.instance.canShowRewardedPopup = true;
        for (int i = 0; i < itemsToHide.Length; i++)
        {
            if (itemsToHide[i] != null)
                itemsToHide[i].SetActive(true);
        }
        MainScript.instance.ShowIndication();
    }
}
