using UnityEngine;

public class VacuumCleaner : Interactable
{
    [SerializeField] private GameObject garbagePrefab; // Prefab for garbage objects
    [SerializeField] private int garbageCount = 5; // Number of garbage objects to spawn
    [SerializeField] private float scatterForce = 3f; // Force for scattering garbage
    [SerializeField] private float scatterRadius = 1f; // Radius for spawn position
    [SerializeField] private string breakAnimationTrigger = "Break"; // Animator trigger name

    private Animator animator;
    private bool isBroken;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator || !garbagePrefab)
        {
            Debug.LogError("Animator or garbage prefab missing.");
            enabled = false;
        }
    }

    public override void Interact(PlayerScript player)
    {
        if (isBroken) return;
        isBroken = true;
        SoundManager.instance.PlaySound(SoundManager.instance.spillTrash);
        //animator.SetTrigger(breakAnimationTrigger);
        for (int i = 0; i < garbageCount; i++)
        {
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * scatterRadius;
            spawnPos.y = transform.position.y; // Keep garbage at vacuum height
            GameObject garbage = Instantiate(garbagePrefab, spawnPos, Random.rotation);
            if (garbage.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                Vector3 randomDir = Random.insideUnitSphere.normalized;
                randomDir.y = Mathf.Abs(randomDir.y); // Ensure upward scattering
                rb.AddForce(randomDir * scatterForce, ForceMode.Impulse);
                MainScript.instance.activeLevel.TaskCompleted(7);
            }
        }
    }
}