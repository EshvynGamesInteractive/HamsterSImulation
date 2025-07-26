using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenReference : MonoBehaviour
{
    public static SplashScreenReference splashScreenReference;
    public SplashScreenManager splashScreen;
    private void Awake()
    {
        splashScreenReference = this;
    }

}
