using System;
using DG.Tweening;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialUI;
    [SerializeField] string[] tutorialTexts;
    [SerializeField] private int[] instructionDelays;
    [SerializeField] private int tutorialCount;
    [SerializeField] private GameObject btnGotIt;
    [SerializeField] private GameObject dullScreen;
    [SerializeField] private GameObject tutorialLevel;
    [SerializeField] private FP_Controller player;
    [SerializeField] private Pickable vase;

    private void Start()
    {
        StartTutorial();
    }

    private void StartTutorial()
    {
        player.StopPlayerMovement();
        player.GetComponent<PlayerScript>().playerCanvas.SetActive(false);
        Typewriter.instance.autoHideAfterCompletion = false;
         tutorialLevel.SetActive(true);
        gameObject.SetActive(true);
        ShowTutorial();
    }

    public void ShowTutorial()
    {
        btnGotIt.SetActive(false);
        dullScreen.SetActive(true);
        tutorialUI[tutorialCount].SetActive(true);
        Typewriter.instance.StartTyping(tutorialTexts[tutorialCount], 0);

        DOVirtual.DelayedCall(instructionDelays[tutorialCount], () =>
        {
            Debug.Log("sdwedknakjn");
            btnGotIt.SetActive(true);
            btnGotIt.transform.DOScale(new Vector2(1.2f, 1.2f), 0.1f);
            btnGotIt.transform.DOScale(new Vector2(1, 1), 0.1f).SetDelay(0.1f);
        });
    }

    public void OnBtnGotIt()
    {
        tutorialCount++;
        if (tutorialCount < tutorialUI.Length)
            ShowTutorial();
        else
        {
            btnGotIt.SetActive(false);
            UITutorialEnded();
        }

    }

    public void UITutorialEnded()
    {
        Typewriter.instance.HideTypeWriter();
      
      vase.EnableForInteraction(true);
        gameObject.SetActive(false);
        Debug.Log("TutorialEnded");
        player.OnEnable();
        player.GetComponent<PlayerScript>().playerCanvas.SetActive(true);
        MainScript.instance.taskPanel.UpdateTask("Pick up the vase and give it a big toss!");
    }

    public void EndTutorial()
    {
        DOVirtual.DelayedCall(2, () =>
        {
            GlobalValues.TutorialPlayed = 1;
            MainScript.instance.OnBtnRetry();
        });
       
    }
}