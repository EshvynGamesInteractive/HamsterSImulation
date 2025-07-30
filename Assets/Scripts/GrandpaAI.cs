using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrandpaAI : MonoBehaviour
{
    [SerializeField] bool scoldPlayerWhenSee;
    [SerializeField] bool shouldCatchPlayer=true;
    public List<Transform> waypoints;
    public float[] waitDurations;
    public float reachThreshold = 0.5f;
    public float detectionRange = 10f;
    public float catchRange = 2.5f;

    private NavMeshAgent agent;
    private int currentIndex = 0;
    private int direction = 1;
    private bool isWaiting = false;
    private bool isChasing = false;
    private Transform player;
    private bool playerHasThrown = false;
    private Animator animator;
    [Range(0, 360)]
    public float viewAngleRight = 90f; // in degrees
    public float viewAngleLeft = 90f; // in degrees
    
    bool stopPatrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (waypoints == null || waypoints.Count < 2)
        {
            Debug.LogError("Need at least 2 waypoints.");
            enabled = false;
            return;
        }
        
        if(scoldPlayerWhenSee)
        {
            catchRange = 5;
        }

        MoveToCurrentWaypoint();
    }

    void Update()
    {
        if (player == null || stopPatrol) return;

        float playerDistance = Vector3.Distance(transform.position, player.position);

        if (shouldCatchPlayer && playerHasThrown && playerDistance <= detectionRange)
        {
            ChasePlayer(playerDistance);
        }
        
        else
        {
            if (isChasing)
            {
                isChasing = false;
                animator.SetBool("isWalking", true);
                MoveToCurrentWaypoint();
            }

            Patrol();
        }
        if(scoldPlayerWhenSee && CanSeePlayer())
        {
            ChasePlayer(playerDistance);
        }
    }

    void Patrol()
    {
        if (isWaiting || agent.pathPending) return;

        if (agent.remainingDistance <= reachThreshold)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);

        float waitTime = waitDurations.Length > currentIndex ? waitDurations[currentIndex] : 1f;
        yield return new WaitForSeconds(waitTime);

        GetNextWaypoint();
        MoveToCurrentWaypoint();

        animator.SetBool("isWalking", true);
        isWaiting = false;
    }

    void GetNextWaypoint()
    {
        currentIndex += direction;

        if (currentIndex >= waypoints.Count)
        {
            direction = -1;
            currentIndex = waypoints.Count - 2;
        }
        else if (currentIndex < 0)
        {
            direction = 1;
            currentIndex = 1;
        }
    }

    void MoveToCurrentWaypoint()
    {
        if (waypoints.Count > 0)
            agent.SetDestination(waypoints[currentIndex].position);
        animator.SetBool("isWalking", true);
    }

    void ChasePlayer(float distance)
    {

        isChasing = true;
        animator.SetBool("isWalking", true);
        agent.SetDestination(player.position);

        if (distance <= catchRange)
        {
            if (CanSeePlayer())
            {
                StartCoroutine(ScoldPlayer());
            }
        }

    }

    bool CanSeePlayer()
    {
        Vector3 origin = transform.position;
        Vector3 target = player.position;
        Vector3 directionToPlayer = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        if (distance > detectionRange)
            return false;

        // Check if player is within view angle
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > viewAngleLeft / 2f)
            return false;

        // Check if anything blocks the view
        if (Physics.Raycast(origin, directionToPlayer, out RaycastHit hit, detectionRange))
        {
            Debug.Log(hit.transform.name);
            Debug.DrawRay(origin, directionToPlayer * detectionRange, Color.red);
            return hit.collider.CompareTag("Player");
        }

        return false;
    }




    IEnumerator ScoldPlayer()
    {
        stopPatrol = true;
        animator.SetBool("isWalking", false);
        agent.ResetPath();

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        animator.SetTrigger("attack");

        player.GetComponent<PlayerScript>()?.PlayerCaught();


        yield return new WaitForSeconds(4f); // Pause after killing before returning to patrol

        isChasing = false;
        playerHasThrown = false;
        //MoveToCurrentWaypoint();
        //animator.SetBool("isWalking", true);
    }

  

    public void NotifyPlayerHasThrown()
    {
        playerHasThrown = true;
    }

    void OnDrawGizmosSelected()
    {
        // Ranges
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, catchRange);

        // FOV Visualization
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        Quaternion leftRayRotation = Quaternion.Euler(0, -viewAngleLeft / 2f, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, viewAngleRight / 2f, 0);

        Vector3 leftRay = leftRayRotation * forward;
        Vector3 rightRay = rightRayRotation * forward;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(origin, leftRay * detectionRange);
        Gizmos.DrawRay(origin, rightRay * detectionRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Water"))
        {
            animator.SetBool("isWalking", false);
            animator.SetTrigger("Fall");
            stopPatrol = true;
            agent.enabled = false;
            GetComponent<Collider>().enabled = false;
            //MainScript.instance.activeLevel
        }
    }
}
