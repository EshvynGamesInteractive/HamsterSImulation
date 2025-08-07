using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

/// <summary>
/// Script which animates progress bar in the UI with current loading bar process.
/// At the completion loads next scene.
/// </summary>
public class SmoothLoading : MonoBehaviour
{
    // Reference to the load operation.
    private AsyncOperation loadOperation;

    // Reference to the progress bar in the UI.
    [SerializeField]
    private Image progressBar;

    // Progress values.
    private float currentValue;
    private float targetValue;

    // Multiplier for progress animation speed.
    [SerializeField]
    [Range(0, 1)]
    private float progressAnimationMultiplier = 0.25f;
    //public int sceneNum=1;
    public RectTransform rotatingImg;
    public RectTransform bg;
    /// <summary>
    /// Unity method called once at the start.
    /// Used here to start the loading progress.
    /// </summary>
    bool isLoading;

    private void Awake()
    {
        //EmptyLoading();
    }

    public GameObject animationPoint;
    private void OnEnable()
    {
        // Set 0 for progress values.
        progressBar.fillAmount = currentValue = targetValue = 0;
    }
    public void StartLoading(int sceneNum)
    {
        CanvasScriptSplash.instance.ChangeCanvas(CanvasStats.Loading);
        //gameObject.SetActive(true);
        // Load the next scene.
        //var currentScene = SceneManager.GetActiveScene();
        bg.DOAnchorPos(Vector3.zero, 1).SetEase(Ease.OutBounce);
        loadOperation = SceneManager.LoadSceneAsync(sceneNum);
        
        // Don't active the scene when it's fully loaded, let the progress bar finish the animation.
        // With this flag set, progress will stop at 0.9f.
        loadOperation.allowSceneActivation = false;

        //AdsManager.Instance.HideBanner();
        isLoading = true;
    }
    public void UnloadScene(int sceneNum)
    {
        SceneManager.UnloadSceneAsync(sceneNum);
        isLoading = true;
    }
    public void RestartScene(int sceneNum)
    {
        if (SceneManager.GetSceneByBuildIndex(sceneNum).isLoaded)
        { 
            SceneManager.UnloadSceneAsync(sceneNum).completed += (AsyncOperation op) =>
            {
                // Reload the scene additively after it's unloaded
                loadOperation = SceneManager.LoadSceneAsync(sceneNum);

                // Don't active the scene when it's fully loaded, let the progress bar finish the animation.
                // With this flag set, progress will stop at 0.9f.
                loadOperation.allowSceneActivation = false;

                //AdsManager.Instance.HideBanner();
                isLoading = true;
            };
        }
        else
        {
            Debug.LogWarning($"Scene 1 is not currently loaded!");
        }
    }
    Action callback = null;
    public void EmptyLoading(Action callBack)
    {
        CanvasScriptSplash.instance.ChangeCanvas(CanvasStats.Loading);
        bg.DOAnchorPos(Vector3.zero, 1).SetEase(Ease.OutBounce);
        loadOperation = null;
        isLoading = true;
        callback = callBack;
    }
    /// <summary>
    /// Unity method called every frame.
    /// Used here to animate progress bar.
    /// </summary>
    private void Update()
    {
        if (!isLoading)
            return;

        if (loadOperation != null)
        {
            // Assign current load progress, divide by 0.9f to stretch it to values between 0 and 1.
            targetValue = loadOperation.progress / 0.9f;
        }
        else
            targetValue = 1;
 
        currentValue = Mathf.MoveTowards(currentValue, targetValue, progressAnimationMultiplier * Time.deltaTime);
        progressBar.fillAmount = currentValue;
        MoveImageAlongLoadingBar();
        // When the progress reaches 1, allow the process to finish by setting the scene activation flag.
        if (Mathf.Approximately(currentValue, 1))
        {
            if (loadOperation != null)
            {
                loadOperation.allowSceneActivation = true;
                loadOperation = null;
            }
            isLoading = false;
            ShutLoading();
            targetValue = 0;
            Debug.Log("Loading Complete");
           // gameObject.SetActive(false);
        }
    }

    public GameObject animationOutPoint;
    void ShutLoading()
    {
        bg.DOAnchorPos(new Vector3(0, 1450, 0), 0.5f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            CanvasScriptSplash.instance.ChangeCanvas(CanvasStats.MainScreen);
            callback?.Invoke();
            callback = null;
        });
    }
    private void MoveImageAlongLoadingBar()
    {
        // Get the width of the loading bar
        float barWidth = progressBar.GetComponent<RectTransform>().rect.width;

        // Calculate the new position for the moving image
        float newXPosition = Mathf.Lerp(0, barWidth, progressBar.fillAmount);

        // Set the position of the moving image
//        rotatingImg.anchoredPosition = new Vector2(newXPosition, rotatingImg.anchoredPosition.y);
    }
}