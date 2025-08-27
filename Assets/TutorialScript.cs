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
    [SerializeField] private GameObject[] itemsToDisable;
    [SerializeField] private RectTransform highlightedCircle;
    [SerializeField] private Sprite vaseIcon;
    private void Start()
    {
        StartTutorial();
    }

    private void StartTutorial()
    {
        foreach (var t in itemsToDisable)
        {
            t.SetActive(false);
        }

        MainScript.instance.taskPanel.gameObject.SetActive(false);
        player.StopPlayerMovement();
        player.GetComponent<PlayerScript>().playerCanvas.SetActive(false);
        Typewriter.instance.autoHideAfterCompletion = false;
        tutorialLevel.SetActive(true);
        gameObject.SetActive(true);
        ShowTutorial();
    }

    public void ShowTutorial()
    {
        if (tutorialCount != 1)
        {
            highlightedCircle.gameObject.SetActive(true);
            RectTransform target = tutorialUI[tutorialCount].GetComponent<RectTransform>();

            highlightedCircle.pivot = target.pivot;
            highlightedCircle.anchorMax = target.anchorMax;
            highlightedCircle.anchorMin = target.anchorMin;
            highlightedCircle.sizeDelta = target.sizeDelta * 1.5f;
            highlightedCircle.anchoredPosition = target.anchoredPosition;
        }
        else
        {
            highlightedCircle.gameObject.SetActive(false);
        }
        btnGotIt.SetActive(false);
        dullScreen.SetActive(true);
        tutorialUI[tutorialCount].SetActive(true);
        Typewriter.instance.StartTyping(tutorialTexts[tutorialCount], 0);

        DOVirtual.DelayedCall(instructionDelays[tutorialCount], () =>
        {
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
        MainScript.instance.taskPanel.gameObject.SetActive(true);
        vase.EnableForInteraction(true);
        gameObject.SetActive(false);
        Debug.Log("TutorialEnded");
        player.OnEnable();
        player.GetComponent<PlayerScript>().playerCanvas.SetActive(true);
        MainScript.instance.taskPanel.UpdateTask("Pick up the vase and give it a big toss!", vaseIcon);
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