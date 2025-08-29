using UnityEngine;
using UnityEngine.AI;

public class CatAI : MonoBehaviour
{
    [SerializeField] private float napInterval = 10f; // Time between nap attempts
    [SerializeField] private string napAnimation = "Nap"; // Nap animation trigger
    [SerializeField] private string hitAnimation = "Hit"; // Hit animation trigger
    [SerializeField] private string runAnimation = "Run"; // Run animation trigger
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] waypoints; // Array of waypoints to loop
    [SerializeField] private NavMeshAgent agent; // NavMeshAgent for movement
    [SerializeField] private float reachThreshold = 0.5f; // Distance to consider point reached
    private float napTimer;
    private bool isMoving;
    private int currentWaypointIndex;

    void Start()
    {
        if (!animator || !agent || waypoints.Length < 2)
        {
            Debug.LogError("Animator, NavMeshAgent, or at least 2 waypoints missing.");
            enabled = false;
            return;
        }
        animator.SetTrigger(napAnimation);
        //napTimer = Random.Range(2f, napInterval); // Random initial nap delay

        // if (agent.enabled && agent.isOnNavMesh)
        //     agent.SetDestination(waypoints[0].position); // Start at first waypoint

        agent.enabled = false;
    }

    //void Update()
    //{
    //    napTimer -= Time.deltaTime;
    //    if (napTimer <= 0)
    //    {
    //        animator.SetTrigger(napAnimation);
    //        napTimer = napInterval; // Reset timer
    //    }
    //}

    void Update()
    {
        if (!isMoving || agent.pathPending || !agent.enabled) return;
        if (agent.remainingDistance <= reachThreshold)
        {
            MoveToNextWaypoint();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sock"))
        {
            collision.gameObject.GetComponent<Interactable>().DisableForInteraction(true);
            SoundManager.instance.PlaySound(SoundManager.instance.catMeow);
            MainScript.instance.activeLevel.TaskCompleted(3);
            Invoke(nameof(OnSockHit), 6);
        }
    }

    private void OnSockHit()
    {
        animator.SetTrigger(hitAnimation);
        agent.enabled = true;
        isMoving = true;
        agent.isStopped = false;
        animator.SetTrigger(runAnimation);
        MoveToNextWaypoint();
    }

    private void MoveToNextWaypoint()
    {
        
        animator.SetTrigger(runAnimation);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop through waypoints
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }
}

























//using UnityEngine;

//public class CatAI : MonoBehaviour
//{
//    [SerializeField] private float napInterval = 10f; // Time between nap attempts
//    [SerializeField] private string napAnimation = "Nap"; // Nap animation trigger
//    [SerializeField] private string hitAnimation = "Hit"; // Hit animation trigger

//    [SerializeField]private Animator animator;
//    private float napTimer;

//    void Start()
//    {
//        animator.SetTrigger(napAnimation);
//        //napTimer = Random.Range(2f, napInterval); // Random initial nap delay
//    }

//    //void Update()
//    //{
//    //    napTimer -= Time.deltaTime;
//    //    if (napTimer <= 0)
//    //    {

//    //        napTimer = napInterval; // Reset timer
//    //    }
//    //}

//    void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Sock"))
//        {
//            animator.SetTrigger(hitAnimation);
//            OnSockHit();
//        }
//    }

//    private void OnSockHit()
//    {
//        SoundManager.instance.PlaySound(SoundManager.instance.catMeow);
//        MainScript.instance.activeLevel.TaskCompleted(3);
//        // Empty method for your cutscene/control logic
//    }
//}