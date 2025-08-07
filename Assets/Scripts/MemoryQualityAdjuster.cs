using UnityEngine;
using UnityEngine.Rendering;

public class MemoryQualityAdjuster : MonoBehaviour
{
    [SerializeField] int lowRamThresholdMB = 4000; // 4 GB
    [SerializeField] GameObject postProcessingVolume; // Assign your post processing volume GameObject here

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        int totalRamMB = SystemInfo.systemMemorySize;

        if (totalRamMB < lowRamThresholdMB)
        {
            Debug.Log("[Memory Adjuster] Low RAM detected: " + totalRamMB + " MB. Applying low-quality settings.");

            // Set lowest quality level (0 = lowest in Project Settings)
            QualitySettings.SetQualityLevel(0, true);

            // Disable post-processing
            if (postProcessingVolume != null)
                postProcessingVolume.SetActive(false);
        }
        else
        {
            Debug.Log("[Memory Adjuster] RAM OK: " + totalRamMB + " MB. Keeping default quality.");
        }
#endif
    }
}
