using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image loadingBar; // Loading bar fill image
    [SerializeField] private float loadDuration = 3f; // Total load duration in seconds

    public void Start()
    {
        Time.timeScale = 1;
        StartCoroutine(LoadSceneAsync());
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