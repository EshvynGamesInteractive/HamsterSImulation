using UnityEngine;

public class KidAI : MonoBehaviour
{
    [SerializeField] private string stumbleAnimationTrigger = "Stumble"; // Animator trigger for stumble
    private Animator animator;
    private bool hasStumbled;
    [SerializeField] bool isDancing;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("Animator missing on Kid.");
            enabled = false;
        }
        if(isDancing)
        {
            animator.SetTrigger("Dance");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasStumbled || !other.CompareTag("Toy")) return;
        hasStumbled = true;
        Debug.Log(animator);
        animator.SetTrigger(stumbleAnimationTrigger);
        MainScript.instance.activeLevel.TaskCompleted(1);
    }
}