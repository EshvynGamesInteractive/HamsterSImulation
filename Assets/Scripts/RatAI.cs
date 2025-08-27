using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RatAI : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTime = 2f;
    [SerializeField]private Animator animator;

    private NavMeshAgent agent;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private bool isWaiting = false;
    private bool isCaught = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void StartMovement()
    {
        if (waypoints.Length == 0 || isCaught) return;

        isMoving = true;
        currentWaypointIndex = 0;
        MoveToWaypoint();
    }

    private void Update()
    {
        if (!isMoving || isWaiting || isCaught) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAtWaypoint());
        }

        UpdateAnimation();
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        UpdateAnimation();

        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        MoveToWaypoint();

        isWaiting = false;
    }

    private void MoveToWaypoint()
    {
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        bool walking = agent.velocity.magnitude > 0.1f && !isWaiting && !isCaught;
        animator.SetBool("Walk", walking);
        animator.SetBool("Idle", !walking);
    }

    public void Caught()
    {
        isCaught = true;
        isMoving = false;
        agent.isStopped = true;

        animator.SetBool("Walk", false);
        animator.SetBool("Idle", true);
    }
}