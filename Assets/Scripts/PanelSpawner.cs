using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelSpawner : MonoBehaviour
{
    public bool isTesting;

    //[SerializeField] private GameObject[] panelsToOpen;
    [SerializeField] GameObject timerAdPanel;
    [SerializeField] private float openInterval = 10f;
    [SerializeField] private GameObject[] cutscenes;
    [SerializeField] private float[] cutsceneDurations;
    [SerializeField] Image panelBG;
    [SerializeField] Sprite[] panelSprites;
    [SerializeField] int selected = 1;

    private bool isWaiting = false;
    private int selectedCutscene;
    public static int previousOpenedPanel = 0;

    private void Start()
    {
        StartCoroutine(PanelLoop());
    }

    private IEnumerator PanelLoop()
    {
        while (true)
        {
            if (!isWaiting && MainScript.instance.canShowRewardedPopup)
            {
                StartPanelTimer();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private Coroutine panelCoroutine;

    public void StartPanelTimer() //reset the curent rutine. play from start
    {
        if (panelCoroutine != null)
            StopCoroutine(panelCoroutine); // cancel current countdown

        panelCoroutine = StartCoroutine(OpenPanelAfterDelay());
    }

    private IEnumerator OpenPanelAfterDelay()
    {
        isWaiting = true;
        float elapsed = 0f;

        while (elapsed < openInterval)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isWaiting = false;
        Debug.Log("can show popup = " + MainScript.instance.canShowRewardedPopup);

        if (MainScript.instance.canShowRewardedPopup)
        {
            isWaiting = true;

            if (isTesting)
            {
                selectedCutscene = selected;
            }
            else
            {
                do
                {
                    selectedCutscene = Random.Range(0, cutscenes.Length);
                } while (selectedCutscene == previousOpenedPanel && cutscenes.Length > 1);
            }

            previousOpenedPanel = selectedCutscene;
            panelBG.sprite = panelSprites[selectedCutscene];

            if (!timerAdPanel.activeSelf)
                MainScript.instance.OpenPopup(timerAdPanel);
        }

        panelCoroutine = null;
    }

    public void OnBtnPlayRewardedCutscene()
    {
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowRewardedAd(() => ShowRewardedCutscene(), "RewardedPrankCutsceneAd");
    }


    private void ShowRewardedCutscene()
    {
        MainScript.instance.ClosePopup(timerAdPanel);
        cutscenes[selectedCutscene].SetActive(true);
        MainScript.instance.player.DisablePlayer();

        DOVirtual.DelayedCall(cutsceneDurations[selectedCutscene], () =>
        {
            MainScript.instance.player.EnablePlayer();
            cutscenes[selectedCutscene].SetActive(false);
            isWaiting = false;
        });
    }

    public void CrossAdPanel()
    {
        isWaiting = false;
    }
}


//using DG.Tweening;
//using System.Collections;
//using UnityEngine;

//public class PanelSpawner : MonoBehaviour
//{
//    [SerializeField] private GameObject[] panelsToOpen;
//    [SerializeField] private float openInterval = 10f;
//    [SerializeField] GameObject[] cutscenes;
//    [SerializeField] float[] cutsceneDurations;

//    private bool isWaiting = false;
//    private int selectedCutscene;

//    private void Start()
//    {
//        StartPanelTimer();
//    }

//    public void StartPanelTimer()
//    {
//        if (!isWaiting)
//            StartCoroutine(OpenPanelAfterDelay());
//    }

//    private IEnumerator OpenPanelAfterDelay()
//    {
//        isWaiting = true;
//        yield return new WaitForSeconds(openInterval);
//        Debug.Log(MainScript.instance.openAdPopup);

//        if (MainScript.instance.openAdPopup)
//        {
//            selectedCutscene = Random.Range(0, panelsToOpen.Length);
//            GameObject chosenPanel = panelsToOpen[selectedCutscene];

//            if (!chosenPanel.activeSelf)
//                MainScript.instance.OpenPopup(chosenPanel);
//            //chosenPanel.SetActive(true);
//        }

//        isWaiting = false;
//    }

//    public void OnBtnPlayRewardedCutscene()
//    {
//        MainScript.instance.ClosePopup(panelsToOpen[selectedCutscene]);
//        cutscenes[selectedCutscene].SetActive(true);
//        MainScript.instance.player.DisablePlayer();
//        DOVirtual.DelayedCall(cutsceneDurations[selectedCutscene], () =>
//        {
//            MainScript.instance.player.EnablePlayer();
//            cutscenes[selectedCutscene].SetActive(false);
//        });
//    }
//}