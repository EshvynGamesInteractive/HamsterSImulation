using UnityEngine;
using UnityEngine.AI;

public class SquirrelAI : MonoBehaviour
{
    [SerializeField] private Transform slippersTarget, endPos; // Target positions
    [SerializeField] private float reachThreshold = 0.5f; // Distance to consider target reached
    [SerializeField] private string runAnimation = "Run"; // Animator trigger for running
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    private bool isActivated;
    private Transform currentTarget; // Tracks current target (slippers or endPos)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent || !animator || !slippersTarget || !endPos)
        {
            Debug.LogError("NavMeshAgent, Animator, slippers target, or end position missing.");
            enabled = false;
            return;
        }
        // gameObject.SetActive(false); // Start inactive (commented as in original)
    }

    public void RunTowardsSlipper()
    {
        Activate(slippersTarget);
    }

    public void RunTowardsCage()
    {
        Activate(endPos);
    }

    public void Activate(Transform target)
    {
        Debug.Log("active " + isActivated);
        if (isActivated) return;
        isActivated = true;
        currentTarget = target; // Set current target
        gameObject.SetActive(true);
        animator.SetTrigger(runAnimation);
        agent.SetDestination(target.position);
        enabled = true;
    }

    void Update()
    {
        Debug.Log(isActivated);
        Debug.Log(agent.pathPending);
        if (!isActivated || agent.pathPending) return;
        if (agent.remainingDistance <= reachThreshold)
        {
            agent.isStopped = true;
            animator.SetTrigger("Idle");
            enabled = false; // Stop updates
            if (currentTarget == slippersTarget)
                OnReachedSlippers();
            else if (currentTarget == endPos)
                OnReachEndPoint();
        }
    }

    private void OnReachEndPoint()
    {
        isActivated = false;
    }

    private void OnReachedSlippers()
    {
        Debug.Log("reached");
        isActivated = false;
        gameObject.SetActive(false);
        MainScript.instance.activeLevel.TaskCompleted(2);
        // Empty method for cutscene trigger
    }
}





















//using UnityEngine;
//using UnityEngine.AI;

//public class SquirrelAI : MonoBehaviour
//{
//    [SerializeField] private Transform slippersTarget, endPos; // Target position (Grandpa's slippers)
//    [SerializeField] private float reachThreshold = 0.5f; // Distance to consider target reached
//    [SerializeField] private string runAnimation = "Run"; // Animator trigger for running
//    [SerializeField] private Animator animator;
//    [SerializeField] 
//    private NavMeshAgent agent;
//    private bool isActivated;

//    void Start()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        if (!agent || !animator || !slippersTarget)
//        {
//            Debug.LogError("NavMeshAgent, Animator, or slippers target missing.");
//            enabled = false;
//            return;
//        }
//        //gameObject.SetActive(false); // Start inactive
//    }

//    public void RunTowardsSlipper()
//    {
//        Activate(slippersTarget);
//    }

//    public void RunTowardsCage()
//    {
//        Activate(endPos);
//    }

//    public void Activate(Transform target)
//    {
//        Debug.Log("active " + isActivated);
//        if (isActivated) return;
//        isActivated = true;
//        gameObject.SetActive(true);
//        animator.SetTrigger(runAnimation);
//        agent.SetDestination(target.position);
//        enabled = true;
//    }

//    void Update()
//    {
//        Debug.Log(isActivated);
//        Debug.Log(agent.pathPending);
//        if (!isActivated || agent.pathPending) return;
//        if (agent.remainingDistance <= reachThreshold)
//        {
//            agent.isStopped = true;
//            animator.SetTrigger("Idle");
//            enabled = false; // Stop updates
//            if (agent.destination == slippersTarget.position)
//                OnReachedSlippers();

//        }
//    }
//    private void OnReachEndPoint()
//    {

//    }
//    private void OnReachedSlippers()
//    {
//        Debug.Log("reached");
//        isActivated = false;
//        gameObject.SetActive(false);
//        MainScript.instance.activeLevel.TaskCompleted(2);
//        // Empty method for cutscene trigger
//    }
//}