using System;
using UnityEngine;
using System.Collections;

public class InterstitialTimerManager : MonoBehaviour
{

    [Header("Ad Cooldown Settings")]
    public float interstitialCooldownTime = 30f; // seconds
    public bool canShowInterstitial = true;

    [SerializeField, Tooltip("Shows remaining cooldown during gameplay")]
    private float timeRemaining; // this will update in Inspector

    
    private Coroutine cooldownRoutine;

    // private void Start()
    // {
    //     StartCooldown();
    // }

    public void StartCooldown()
    {
        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
        }
        // if (!canShowInterstitial) return;
        timeRemaining = interstitialCooldownTime;
        canShowInterstitial = false;
        cooldownRoutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        // Debug.LogError();
        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime; 
            yield return null;
        }
        // yield return new WaitForSecondsRealtime(interstitialCooldownTime);
        canShowInterstitial = true;
        timeRemaining = 0f;
        cooldownRoutine = null;
    }
    
    
}