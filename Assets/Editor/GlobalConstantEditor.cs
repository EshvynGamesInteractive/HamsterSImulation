using UnityEditor;
using UnityEngine;

public class GlobalConstantEditor : EditorWindow
{
    private string rateUsLink;
    private string privacyPoliciesLink;

    private bool isLogger;
    private bool adsOn;
    private bool showAppOpen;
    private bool isMaxOn;
    private bool useAdBidding;

    private int adTimer;

    [MenuItem("Tools/Global Constant Editor")]
    public static void ShowWindow()
    {
        GetWindow<GlobalConstantEditor>("Global Constant Editor");
    }

    private void OnEnable()
    {
        LoadValues();
    }

    private void OnInspectorUpdate()
    {
        // Called ~10 times/second in editor — good for live sync
        LoadValues();
        Repaint();
    }

    private void LoadValues()
    {
        rateUsLink = GlobalConstant.RateUsLink;
        privacyPoliciesLink = GlobalConstant.PrivacyPoliciesLInk;

        isLogger = GlobalConstant.isLogger;
        adsOn = GlobalConstant.AdsON;
        showAppOpen = GlobalConstant.ShowAppOpen;
        isMaxOn = GlobalConstant.ISMAXON;
        useAdBidding = GlobalConstant.UseAdBidding;

        adTimer = GlobalConstant.adTimer;
    }

    private void OnGUI()
    {
        GUILayout.Label("Ad Settings", EditorStyles.boldLabel);

        adTimer = EditorGUILayout.IntField("Ad Timer", adTimer);
        isLogger = EditorGUILayout.Toggle("Is Logger", isLogger);
        adsOn = EditorGUILayout.Toggle("Ads On", adsOn);
        showAppOpen = EditorGUILayout.Toggle("Show App Open", showAppOpen);
        isMaxOn = EditorGUILayout.Toggle("Is Max On", isMaxOn);
        useAdBidding = EditorGUILayout.Toggle("Use Ad Bidding", useAdBidding);

        GUILayout.Space(10);
        GUILayout.Label("Links", EditorStyles.boldLabel);

        rateUsLink = EditorGUILayout.TextField("Rate Us Link", rateUsLink);
        privacyPoliciesLink = EditorGUILayout.TextField("Privacy Policy Link", privacyPoliciesLink);

        GUILayout.Space(10);

        if (GUILayout.Button("Apply Changes"))
        {
            GlobalConstant.adTimer = adTimer;
            GlobalConstant.isLogger = isLogger;
            GlobalConstant.AdsON = adsOn;
            GlobalConstant.ShowAppOpen = showAppOpen;
            GlobalConstant.ISMAXON = isMaxOn;
            GlobalConstant.UseAdBidding = useAdBidding;

            GlobalConstant.RateUsLink = rateUsLink;
            GlobalConstant.PrivacyPoliciesLInk = privacyPoliciesLink;

            Debug.Log("GlobalConstant updated!");
        }

        if (GUILayout.Button("Refresh"))
        {
            LoadValues();
        }
    }
}
