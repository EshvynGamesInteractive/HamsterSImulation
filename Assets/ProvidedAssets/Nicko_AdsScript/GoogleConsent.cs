using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Ump.Api;
using System;

public class GoogleConsent : MonoBehaviour
{
    private void Awake()
    {
        GatherConsent(print);
    }

    public bool CanRequestAds => ConsentInformation.CanRequestAds();
 public void GatherConsent(Action<string> onComplete)
    {
        var requestparameters = new ConsentRequestParameters
        {
          ConsentDebugSettings = new ConsentDebugSettings
          {
              DebugGeography = DebugGeography.EEA,
              TestDeviceHashedIds = new()
              {
                  "Test-device-hashed-id"
              }
          }
        };

        ConsentInformation.Update(requestparameters, (FormError updateError) => 
        {
            if (updateError != null)
            {
                onComplete(updateError.Message);
                return;
            }
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
            {
                // consent gathering process has complete
               onComplete?.Invoke(showError?.Message);
            });
        });
    }
}
