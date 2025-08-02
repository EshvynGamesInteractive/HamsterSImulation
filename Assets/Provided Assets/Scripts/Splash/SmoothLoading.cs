using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SmoothLoading : MonoBehaviour
{
    [Header("UI Elements")]
    //public Slider loadingBar;    
    public Image loadingBar;    

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    public void StartLoading(int sceneIndex)
    {
        StartCoroutine(LoadSceneSmooth(sceneIndex));
    }

    public IEnumerator LoadSceneSmooth(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingBar != null)
                loadingBar.fillAmount = progress;

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f); 
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }


}
