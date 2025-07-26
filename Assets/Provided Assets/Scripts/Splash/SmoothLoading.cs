using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SmoothLoading : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup fadePanel; 
    public Slider loadingBar;    

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    public void StartLoading(int sceneIndex)
    {
        StartCoroutine(LoadSceneSmooth(sceneIndex));
    }

    public IEnumerator LoadSceneSmooth(int sceneIndex)
    {
        yield return StartCoroutine(Fade(0f, 1f));

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingBar != null)
                loadingBar.value = progress;

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f); 
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
            
        fadePanel.blocksRaycasts = true;

            while (elapsed < fadeDuration)
            {
                fadePanel.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            fadePanel.alpha = to;
            fadePanel.blocksRaycasts = (to > 0);
        
    }
}
