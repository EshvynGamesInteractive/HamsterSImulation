using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PanelSpawner : MonoBehaviour
{
    public bool isTesting;

    //[SerializeField] private GameObject[] panelsToOpen;
    private GrandpaAI grandpa;
    [SerializeField] GameObject timerAdPanel;
    [SerializeField] private float openInterval = 10f;
    [SerializeField] private GameObject[] cutscenesGroundFloor, cutscenesFirstFloor;
    [SerializeField] private float[] cutsceneDurations;
    [SerializeField] Image panelBG;
    [SerializeField] Sprite[] panelSprites;
    [SerializeField] int selected = 1;

    private bool isWaiting = false;
    private int selectedCutscene;
    public static int previousOpenedPanel = 0;
    private MainScript mainScript;
    private void Start()
    {
        mainScript = MainScript.instance;
        grandpa = mainScript.grandPa;
        StartCoroutine(PanelLoop());
    }

    private IEnumerator PanelLoop()
    {
        while (true)
        {
           
            if (!isWaiting && mainScript.canShowRewardedPopup)
            {
                StartPanelTimer();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private Coroutine panelCoroutine;

    public void StartPanelTimer() //reset the current routine. play from start
    {
        if (panelCoroutine != null)
            StopCoroutine(panelCoroutine); // cancel current countdown

        panelCoroutine = StartCoroutine(OpenPanelAfterDelay());
    }

    private IEnumerator OpenPanelAfterDelay()
    {
       
        if(GlobalValues.TutorialPlayed==0)
            yield break;
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
                    selectedCutscene = Random.Range(0, cutscenesGroundFloor.Length);
                } while (selectedCutscene == previousOpenedPanel && cutscenesGroundFloor.Length > 1);
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
        else 
            ShowRewardedCutscene();
    }


    private void ShowRewardedCutscene()
    {
        GameObject cutsceneToPlay =cutscenesGroundFloor[selectedCutscene];

        if (grandpa.IsOnGroundFloor())
        {
            cutsceneToPlay = cutscenesGroundFloor[selectedCutscene];
        }
        else if(grandpa.IsOnFirstFloor())
        {
            cutsceneToPlay = cutscenesFirstFloor[selectedCutscene];
        }
        
        MainScript.instance.ClosePopup(timerAdPanel);
        cutsceneToPlay.SetActive(true);
        MainScript.instance.player.DisablePlayer();

        DOVirtual.DelayedCall(cutsceneDurations[selectedCutscene], () =>
        {
            MainScript.instance.player.EnablePlayer();
            cutsceneToPlay.SetActive(false);
            isWaiting = false;
        });
    }

    public void CrossAdPanel()
    {
        isWaiting = false;
        if(Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowInterstitial("CloseCutscenePanel");
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