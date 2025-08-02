using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GoatScript : Interactable
{
    [SerializeField] private Transform[] waypoints; // Predefined waypoints for goat
    [SerializeField] private float reachThreshold = 0.5f; // Distance to consider waypoint reached
    [SerializeField] private string runAnimation = "Run"; // Run animation trigger
    [SerializeField] private GameObject cinematicCamera; // Camera for cutscene
    [SerializeField] private float cutsceneDuration = 5f; // Cutscene length
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] GameObject goatGrandpa;

    private GrandpaAI grandpaAI;
    private Camera mainCamera;
    private int currentWaypointIndex;
    private bool isMoving;
    private bool cutsceneTriggered;

    void Start()
    {
        grandpaAI = MainScript.instance.grandPa;
        if (!agent || !animator || waypoints.Length == 0 || !cinematicCamera)
        {
            Debug.LogError("NavMeshAgent, Animator, waypoints, or cinematic camera missing.");
            enabled = false;
            return;
        }
        mainCamera = Camera.main;
       
    }

    public override void Interact(PlayerScript player)
    {
        DisableForInteraction(true);
        MainScript.instance.HideIndication();
        StartSequence();
        StartCoroutine(PlayCutscene());
    }

    public void StartSequence()
    {
        isMoving = true;
        animator.SetTrigger(runAnimation);
        currentWaypointIndex = -1; // Start at -1 so first waypoint is 0
        MoveToNextWaypoint();
    }

    void Update()
    {
        if (!isMoving || agent.pathPending) return;
        //if (agent.remainingDistance <= reachThreshold && !cutsceneTriggered)
        //{
        //    cutsceneTriggered = true;
        //    StartCoroutine(PlayCutscene());
        //}
        else if (agent.remainingDistance <= reachThreshold)
        {
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop back to 0 at end
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(1);
        grandpaAI.gameObject.SetActive(false);
        goatGrandpa.SetActive(true);
        MainScript.instance.player.DisablePlayer();
        //mainCamera.enabled = false;
        cinematicCamera.SetActive(true);
        yield return new WaitForSeconds(cutsceneDuration);
        //cinematicCamera.SetActive(false);
        //mainCamera.enabled = true;
        MainScript.instance.activeLevel.TaskCompleted(5);
        MoveToNextWaypoint();
    }
}