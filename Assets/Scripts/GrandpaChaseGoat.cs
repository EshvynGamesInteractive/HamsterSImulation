using UnityEngine;
using UnityEngine.AI;

public class GrandpaChaseGoat : MonoBehaviour
{
    [SerializeField] private Transform goat;
    [SerializeField] private string runAnimation = "Run";
    private NavMeshAgent agent;
    //private Animator animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (!agent || !goat)
        {
            Debug.LogError("NavMeshAgent, Animator, or goat missing.");
            enabled = false;
            return;
        }
        //animator.SetTrigger(runAnimation);
        agent.enabled = true;
    }

    void Update()
    {
        if (goat && agent.enabled)
        {
            agent.SetDestination(goat.position);
        }
    }
}