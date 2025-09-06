using UnityEngine;
using System.Collections;

public class InterstitialTimerManager : MonoBehaviour
{

    [Header("Ad Cooldown Settings")]
    public float cooldownTime = 30f; // seconds
    public bool canShowInterstitial = true;

  

    public void StartCooldown()
    {
        if (!canShowInterstitial) return;

        canShowInterstitial = false;
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSecondsRealtime(cooldownTime);
        canShowInterstitial = true;
    }
    
    
}