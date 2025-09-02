using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RatAI : Pickable
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private bool isWaiting = false;
    private bool isCaught = false;
    public float currentSpeed;

    public override void Interact(PlayerScript player)
    {
        MainScript.instance.HideIndication();
        base.PickItem(player);

        StopMovement();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    public void StartMovement()
    {
        if (waypoints.Length == 0 || isCaught) return;
        agent.enabled = true;
        isMoving = true;
        currentWaypointIndex = 0;
        MoveToWaypoint();
    }

    public void StopMovement()
    {
        agent.enabled = false;
        isMoving = false;
        currentWaypointIndex = 0;

        // UpdateAnimation();
    }
    private void OnEnable()
    {
        InvokeRepeating(nameof(UpdateAnimation), 0f, 0.2f); // every 0.1s
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(UpdateAnimation));
    }
    private void Update()
    {
        if (!isMoving || isWaiting || isCaught) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAtWaypoint());
        }

        //UpdateAnimation();
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        //    UpdateAnimation();

        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        MoveToWaypoint();

        isWaiting = false;
    }

    private void MoveToWaypoint()
    {
        if (agent.enabled)
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        //  UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        currentSpeed = agent.velocity.magnitude;

        // Debug.Log("speed " + agent.velocity.magnitude);

        bool walking = agent.velocity.magnitude > 0.01f && !isWaiting && !isCaught;
        animator.SetBool("Walk", walking);
    }

    public void Caught()
    {
        isCaught = true;
        isMoving = false;
        agent.isStopped = true;
        agent.enabled = false;
        animator.SetBool("Walk", false);
    }
}