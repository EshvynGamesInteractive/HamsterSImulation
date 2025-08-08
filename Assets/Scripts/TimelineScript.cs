using UnityEngine;

public class TimelineScript : MonoBehaviour
{
    [SerializeField] GameObject[] itemsToHide;


    private void OnEnable()
    {
        for(int i = 0; i < itemsToHide.Length; i++)
        {
            itemsToHide[i].SetActive(false);
        }
        MainScript.instance.HideIndication();
    }


    private void OnDisable()
    {
        for (int i = 0; i < itemsToHide.Length; i++)
        {
            itemsToHide[i].SetActive(true);
        }
        MainScript.instance.ShowIndication();
    }
}
