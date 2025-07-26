using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrandpaAI : MonoBehaviour
{
    public List<Transform> waypoints;
    public float[] waitDurations;
    public float reachThreshold = 0.5f;
    public float detectionRange = 10f;
    public float attackRange = 2.5f;

    private NavMeshAgent agent;
    private int currentIndex = 0;
    private int direction = 1;
    private bool isWaiting = false;
    private bool isChasing = false;
    private Transform player;
    private bool playerHasThrown = false;
    private Animator animator;

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
        

        MoveToCurrentWaypoint();
    }

    void Update()
    {
        if (player == null) return;

        float playerDistance = Vector3.Distance(transform.position, player.position);

        if (playerHasThrown && playerDistance <= detectionRange)
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

        if (distance <= attackRange)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    IEnumerator AttackPlayer()
    {
        animator.SetBool("isWalking", false);
        agent.ResetPath();

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(1.5f); // Adjust to match your attack animation timing
        player.GetComponent<PlayerScript>()?.PlayerCaught();


        yield return new WaitForSeconds(1f); // Pause after killing before returning to patrol

        isChasing = false;
        playerHasThrown = false;
        MoveToCurrentWaypoint();
        animator.SetBool("isWalking", true);
    }

    public void PlayerDie()
    {
        player.GetComponent<PlayerScript>()?.PlayerCaught();
    }

    public void NotifyPlayerHasThrown()
    {
        playerHasThrown = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
