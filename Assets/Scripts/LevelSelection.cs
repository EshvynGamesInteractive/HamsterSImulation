using System;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    public LevelButton[] levels;
    private int selectedLevel = -1;
    [SerializeField] GameObject lockPrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollDuration = 0.3f;


    private void Start()
    {
        LockLevels();
    }

    private void OnEnable()
    {
        if (GlobalValues.UnlockedStages > levels.Length)
            GlobalValues.UnlockedStages = levels.Length;

        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.HideBanner();
            Nicko_ADSManager._Instance.HideRecBanner();
        }

        float moveTO = 300;
        float moveDuration = 0.2f;
        
        Debug.Log(GlobalValues.UnlockedStages);
        switch (GlobalValues.UnlockedStages)
        {
            case 1:
            case 2:
                moveDuration = 1;
                moveTO = 0;
                break;
            case 3:
                moveDuration = 0.5f;
                moveTO = 200;
                break;
            case 5:
                moveDuration = 0.2f;
                moveTO = 550;
                break;
        }


        content.DOAnchorPosY(310, 0);
        content.DOAnchorPosY(moveTO, moveDuration);
        Debug.Log(GlobalValues.UnlockedStages);


        levels[GlobalValues.UnlockedStages - 1].transform.DOPunchScale(new Vector2(0.1f, 0.1f), 0.5f, 1, 1)
            .SetDelay(moveDuration);
        //StartCoroutine(FocusOnUnlockedLevel());
    }

    private void OnDisable()
    {
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowBanner("Level selection close");
    }
    //IEnumerator FocusOnUnlockedLevel()
    //{
    //    yield return new WaitForEndOfFrame(); // Wait for layout to build

    //    int index = Mathf.Clamp(GlobalValues.UnlockedLevels - 1, 0, levels.Length - 1);
    //    RectTransform targetBtn = levels[index].GetComponent<RectTransform>();

    //    Debug.Log(levels[index]);
    //    // Convert the button's position to content-local space
    //    Vector2 localPos;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(content, targetBtn.position, null, out localPos);

    //    float targetY = Mathf.Abs(localPos.y);

    //    // Clamp targetY so you don�t scroll beyond bounds
    //    float contentHeight = content.rect.height;
    //    float viewportHeight = scrollRect.viewport.rect.height;
    //    float maxScroll = contentHeight - viewportHeight;
    //    //targetY = Mathf.Clamp(targetY, 0, maxScroll);

    //    // Animate (with DOTween) or set directly
    //    content.DOAnchorPos(new Vector2(content.anchoredPosition.x, targetY), scrollDuration).SetEase(Ease.Linear);

    //    // Without DOTween
    //    //content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetY);
    //}


    public void LockLevels()
    {
        Debug.Log(GlobalValues.UnlockedStages);
        for (int i = GlobalValues.UnlockedStages; i < levels.Length; i++)
        {
            levels[i].LockLevel();
            Instantiate(lockPrefab, levels[i].transform);
        }
    }

    public void OnSelectLevel(int levelNumber)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonClick);
        selectedLevel = levelNumber;
        OnBtnPlaylevel();
    }


    public void OnBtnPlaylevel()
    {
        GlobalValues.currentStage = selectedLevel;
        //GlobalValues.sceneTOLoad = "Gameplay";
        //Debug.Log("gameplay");
        gameObject.SetActive(false);
        CanvasScriptSplash.instance.LoadScene("Gameplay");
        //SceneManager.LoadScene("Loading");
        if (Nicko_ADSManager._Instance)
        {
            Nicko_ADSManager._Instance.ShowInterstitial("PlayButtonAd");

            Nicko_ADSManager._Instance.RecShowBanner("OnBtnPlayLevel");
        }
    }

    public void OnBtnBack()
    {
        gameObject.SetActive(false);
    }
}