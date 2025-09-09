using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InterruptiveAd : MonoBehaviour
{
    [Header("Ad Settings")] public float interruptiveAdDelay = 90f; // Ad trigger interval (90 sec)
    public float preAdDelay = 3f; // Canvas show before ad starts

    [Header("UI References")] public Canvas adCanvas;
    public Text adTimerText;
    public float timeRemaining;
    private Coroutine adCycleRoutine;

    void Start()
    {
        adCanvas.gameObject.SetActive(false);
        // Start first cycle
        adCycleRoutine = StartCoroutine(AdCycleRoutine());
    }

    IEnumerator AdCycleRoutine()
    {
        while (true) // infinite loop
        {
            // Wait for adDuration
            timeRemaining = interruptiveAdDelay;

            while (timeRemaining > 0)
            {
                adTimerText.text = "Next Ad in: " + Mathf.Ceil(timeRemaining) + "s";
                timeRemaining -= Time.deltaTime;
                yield return null;
            }

            // Pre-ad delay
            float preTime = preAdDelay;

            while (preTime > 0)
            {
                // Time.timeScale = 0;
                preTime -= Time.deltaTime;
                adTimerText.text = "Ad starts in: " + Mathf.Ceil(preTime) + "s";
                adCanvas.gameObject.SetActive(true);
                yield return null;
            }

            adCanvas.gameObject.SetActive(false);

            // ✅ Call Ad Manager
            print("Showing Ad Now!");

            if (Nicko_ADSManager.instance)
            {
                Nicko_ADSManager.instance.ShowInterstitial("InterruptiveAD");
            }

            // Reset UI
            adCanvas.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    // ✅ Public method for reset button
    public void ResetAdCycle()
    {
        if (adCycleRoutine != null)
            StopCoroutine(adCycleRoutine);

        adCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;

        // ✅ reset timer bhi karo
        timeRemaining = interruptiveAdDelay;

        // Restart cycle
        adCycleRoutine = StartCoroutine(AdCycleRoutine());
        print("Ad cycle has been reset!");
    }
}