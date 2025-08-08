using System.Collections;
using UnityEngine;

public class PanelSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] panelsToOpen;
    [SerializeField] private float openInterval = 10f;

    private bool isWaiting = false;

    private void Start()
    {
        StartPanelTimer();
    }

    public void StartPanelTimer()
    {
        if (!isWaiting)
            StartCoroutine(OpenPanelAfterDelay());
    }

    private IEnumerator OpenPanelAfterDelay()
    {
        isWaiting = true;
        yield return new WaitForSeconds(openInterval);
        Debug.Log(MainScript.instance.openAdPopup);

        if (MainScript.instance.openAdPopup)
        {
            int index = Random.Range(0, panelsToOpen.Length);
            GameObject chosenPanel = panelsToOpen[index];

            if (!chosenPanel.activeSelf)
                MainScript.instance.OpenPopup(chosenPanel);
                //chosenPanel.SetActive(true);
        }

        isWaiting = false;
    }
}
