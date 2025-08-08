using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScript : MonoBehaviour
{
    [SerializeField] private Image loadingBar; // Loading bar fill image
    [SerializeField] private float loadDuration = 3f; // Total load duration in seconds
    [SerializeField] RectTransform[] letters;
    [SerializeField] float oneLetterTime = 0.5f;
    [SerializeField] float letterScale = 1.5f;
    [SerializeField] RectTransform dog;

    public IEnumerator Start()
    {
        Time.timeScale = 1;
        StartCoroutine(LoadSceneAsync());

        dog.localScale = Vector3.one * letterScale;
        dog.GetComponent<Image>().DOFade(0, 0);


        foreach (var letter in letters)
        {
            letter.localScale = Vector3.one * letterScale;
            letter.GetComponent<Image>().DOFade(0, 0);
        }

        foreach (var letter in letters)
        {
            letter.localScale = Vector3.one * letterScale;
            letter.GetComponent<Image>().DOFade(1, oneLetterTime);
            letter.DOScale(Vector3.one, oneLetterTime);
            yield return new WaitForSeconds(oneLetterTime);
        }

        dog.DOScale(1, oneLetterTime);
        dog.GetComponent<Image>().DOFade(1, oneLetterTime);
        dog.DOPunchScale(new Vector2(0.2f, 0.2f), 1.5f, 4, 5).SetEase(Ease.Linear);
        if (SoundManager.instance)
            SoundManager.instance.PlaySound(SoundManager.instance.dogBark);
    }


    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(GlobalValues.sceneTOLoad);
        operation.allowSceneActivation = false;

        float elapsedTime = 0f;
        while (elapsedTime < loadDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / loadDuration); // Linear progress over duration
            if (loadingBar != null)
            {
                loadingBar.fillAmount = progress;
            }
            yield return null;
        }

        // Ensure operation completes
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
