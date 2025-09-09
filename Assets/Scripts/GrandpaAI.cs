using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrandpaAI : MonoBehaviour
{
    #region Inspector Fields

    [Header("General References")] public GameObject dogInHand;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform grandPaCamera;
    [SerializeField] private Material faceMat;
    [SerializeField] private ParticleSystem angryEmote;
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private GameObject electrocutedParticle, beesParticle;

    [Header("Chase Settings")] [SerializeField]
    private Transform chaseEndPoint;


    public float chaseEndThreshold = 1f;
    [SerializeField] private bool scoldPlayerWhenSee;
    [SerializeField] private bool shouldCatchPlayer = true;
    public float detectionRange = 10f;
    public float catchRange = 2.5f;
    [Range(0, 180)] public float viewAngle = 180;

    [Header("Patrol Settings")] [SerializeField]
    private List<Transform> waypointsGroundFloor;

    [SerializeField] private List<Transform> waypointsFirstFloor;
    [SerializeField] private List<Transform> waypointsGroundFloor2;
    public List<Transform> waypoints;
    public float[] waitDurations;
    public float reachThreshold = 0.5f;

    [Header("Floor Settings")] [SerializeField]
    private float floorHeightThreshold = 3f;

    #endregion

    #region Private State

    private PlayerScript player;
    private int currentIndex = 0;
    private int direction = 1;
    private bool isWaiting = false;
    private bool isRunning = false;
    private bool isChasing = false;
    private bool stopWalking = false;
    public bool isSitting = false;

    // Duration-based chase
    private bool isChasingForDuration;
    private float chaseTimer;


    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float checkInterval = 0.2f;
    [SerializeField] private float navMeshSearchRadius = 2f;

    private float checkTimer;

    #endregion

    #region Unity Methods

    private void Start()
    {
        player = MainScript.instance.player;
        StartPatrolOnGroundFloor();

        if (waypoints == null || waypoints.Count < 2)
        {
            Debug.LogError("Need at least 2 waypoints.");
            enabled = false;
            return;
        }

        if (scoldPlayerWhenSee)
            catchRange = 5;

        MoveToCurrentWaypoint();
    }

    private void Update()
    {
        // Debug.Log(IsOnFirstFloor() ? "Grandpa is on the FIRST floor" : "Grandpa is on the GROUND floor");

        if (player == null || stopWalking) return;

        if (isChasingForDuration)
        {
            HandleChaseDuration();
            return;
        }

        float playerDistance = Vector3.Distance(transform.position + Vector3.up, player.transform.position);

        if (shouldCatchPlayer && playerDistance <= detectionRange)
        {
            ChasePlayer(playerDistance);
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                EnableWalking(true);
                MoveToCurrentWaypoint();
            }

            Patrol();
        }

        if (scoldPlayerWhenSee && CanSeePlayer())
            ChasePlayer(playerDistance);
    }

    private void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= chaseRange)
        {
            NavMeshHit hit;
            // Check if player is on navmesh
            if (NavMesh.SamplePosition(player.transform.position, out hit, navMeshSearchRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                // Player is in non-walkable zone, go to closest navmesh point
                if (NavMesh.SamplePosition(player.transform.position, out hit, 10f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
        }
    }

    #endregion

    #region Patrol Logic

    public void StartPatrolOnFirstFloor()
    {
        waypoints = waypointsFirstFloor;
        EnableWalking(true);
        stopWalking = false;
        agent.enabled = true;
        shouldCatchPlayer = false;
    }

    public void StartPatrolOnGroundFloor()
    {
        if (GlobalValues.currentStage == 1)
            waypoints = waypointsGroundFloor;
        else
            waypoints = waypointsGroundFloor2;
        EnableWalking(true);
        stopWalking = false;
        agent.enabled = true;
        shouldCatchPlayer = false;
    }

    private void Patrol()
    {
        if (isWaiting || agent.pathPending) return;
        if (agent.remainingDistance <= reachThreshold)
            StartCoroutine(WaitAtWaypoint());
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;

        EnableWalking(false);

        float waitTime = waitDurations.Length > currentIndex ? waitDurations[currentIndex] : 1f;
        if (isRunning)
            waitTime = 0;
        yield return new WaitForSeconds(waitTime);

        GetNextWaypoint();
        MoveToCurrentWaypoint();

        EnableWalking(true);
        isWaiting = false;
    }

    private void GetNextWaypoint()
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

    private void MoveToCurrentWaypoint()
    {
        if (agent.enabled && agent.isOnNavMesh && waypoints.Count > 0)
        {
            if (currentIndex >= waypoints.Count)
                currentIndex = waypoints.Count - 1;

            agent.SetDestination(waypoints[currentIndex].position);

            EnableWalking(true);
        }
    }

    #endregion

    #region Chase Logic

    private void ChasePlayer(float distance)
    {
        if (!agent.enabled) return;

        isChasing = true;
        EnableWalking(true);

        SetDestinationToPlayer();

        if (distance <= catchRange && CanSeePlayer())
            StartCoroutine(ScoldPlayer());
    }

    private float updateTimer;
    [SerializeField] private float updateInterval = 0.25f; // 4 times a second

    private void HandleChaseDuration()
    {
        chaseTimer -= Time.deltaTime;

        if (chaseEndPoint != null && Vector3.Distance(transform.position, chaseEndPoint.position) <= chaseEndThreshold)
        {
            StopTheChase();
            return;
        }

        if (chaseTimer <= 0)
        {
            StopTheChase();
            return;
        }

        if (!agent.enabled) return;

        EnableWalking(true);

        float playerDistance = Vector3.Distance(transform.position + Vector3.up, player.transform.position);
        if (playerDistance <= catchRange && CanSeePlayer())
        {
            StartCoroutine(ScoldPlayer());
            isChasingForDuration = false;
        }

        // Only update destination at set intervals
        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f)
        {
            updateTimer = updateInterval;
            SetDestinationToPlayer();
        }
    }

    /// <summary>
    /// Tries to set Grandpa's destination to the player's position,
    /// snapping to nearest NavMesh point if the player is off-navmesh.
    /// </summary>
    private Vector3? lastSampledPoint = null; // store for gizmos

    private bool lastSampledSuccess = false;


    private void SetDestinationToPlayer()
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(player.transform.position, out hit, navMeshSearchRadius, NavMesh.AllAreas) ||
            NavMesh.SamplePosition(player.transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            if (!lastSampledPoint.HasValue || Vector3.Distance(lastSampledPoint.Value, hit.position) > 0.1f)
            {
                agent.SetDestination(hit.position);
                lastSampledPoint = hit.position;
            }
        }
        else if (lastSampledPoint.HasValue)
        {
            agent.SetDestination(lastSampledPoint.Value);
        }
    }


// Optional: Gizmos for debugging
    // private void OnDrawGizmos()
    // {
    //     if (lastSampledPoint.HasValue)
    //     {
    //         Gizmos.color = lastSampledSuccess ? Color.green : Color.yellow;
    //         Gizmos.DrawSphere(lastSampledPoint.Value, 0.2f);
    //     }
    //
    //     if (player != null)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawSphere(player.transform.position, 0.15f);
    //     }
    // }


// Optional: visualize in Scene view
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(player.transform.position, 0.2f);
        }

        if (lastSampledSuccess && lastSampledPoint.HasValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(lastSampledPoint.Value, 0.2f);

            if (agent != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(agent.transform.position, lastSampledPoint.Value);
            }
        }
    }


    // private void ChasePlayer(float distance)
    // {
    //     if (agent.enabled)
    //     {
    //         isChasing = true;
    //         EnableWalking(true);
    //
    //         NavMeshHit hit;
    //         if (NavMesh.SamplePosition(player.transform.position, out hit, navMeshSearchRadius, NavMesh.AllAreas))
    //         {
    //             agent.SetDestination(hit.position);
    //         }
    //         else if (NavMesh.SamplePosition(player.transform.position, out hit, 10f, NavMesh.AllAreas))
    //         {
    //             agent.SetDestination(hit.position);
    //         }
    //
    //         if (distance <= catchRange && CanSeePlayer())
    //             StartCoroutine(ScoldPlayer());
    //     }
    // }
    //
    //
    // private void HandleChaseDuration()
    // {
    //     chaseTimer -= Time.deltaTime;
    //
    //     if (chaseEndPoint != null && Vector3.Distance(transform.position, chaseEndPoint.position) <= chaseEndThreshold)
    //     {
    //         StopTheChase();
    //         return;
    //     }
    //
    //     if (chaseTimer <= 0)
    //     {
    //         StopTheChase();
    //         return;
    //     }
    //
    //     if (agent.enabled)
    //     {
    //         agent.SetDestination(player.transform.position);
    //         EnableWalking(true);
    //
    //         float playerDistance = Vector3.Distance(transform.position + Vector3.up, player.transform.position);
    //         if (playerDistance <= catchRange && CanSeePlayer())
    //         {
    //             StartCoroutine(ScoldPlayer());
    //             isChasingForDuration = false;
    //         }
    //         checkTimer += Time.deltaTime;
    //         if (checkTimer >= checkInterval)
    //         {
    //             checkTimer = 0f;
    //             ChasePlayer();
    //         }
    //     }
    // }

    public void StartTheChase()
    {
        MainScript.instance.pnlInfo.ShowInfo("Grandpa is coming to catch you");
        EnableWalking(true);
        stopWalking = false;
        agent.enabled = true;
        shouldCatchPlayer = true;
    }

    public void StopTheChase()
    {
        if (shouldCatchPlayer || isChasingForDuration)
            MainScript.instance.pnlInfo.ShowInfo("Grandpa gave up the chase.");
        isChasing = false;
        shouldCatchPlayer = false;
        chaseTimer = 0;
        isChasingForDuration = false;
        if (GlobalValues.currentStage == 1)
            waypoints = waypointsGroundFloor;
        else
            waypoints = waypointsGroundFloor2;
        MoveToCurrentWaypoint();
    }

    public void ChasePlayerForDuration(int index)
    {
        if (isSitting || isChasing) return;
        MainScript.instance.pnlInfo.ShowInfo("Grandpa’s coming for you, better stay out of sight!");

        StopWalking();

        if (index == 1)
            chaseTimer = 10;
        else if (index == 2)
            chaseTimer = 15;
        else
            chaseTimer = 20;

        float startChaseDelay = 3;
         
        DOVirtual.DelayedCall(startChaseDelay, () =>
            {
                if (!isSitting)
                    StartChase();
                else
                {
                    StopTheChase();
                }
            }
        );
    }

    private void StopWalking()
    {
        isChasing = false;
        isChasingForDuration = false;
        stopWalking = true;
        agent.enabled = false;
        EnableWalking(false);
    }
    private void StartChase()
    {
        isChasing = true;
        isChasingForDuration = true;
        stopWalking = false;
        agent.enabled = true;
        EnableWalking(true);
    }

    #endregion

    #region Interaction Logic

    private bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 1.3f;
        Vector3 target = player.transform.position;
        Vector3 directionToPlayer = (target - origin).normalized;

        float distance = Vector3.Distance(origin, target);
        if (distance > detectionRange) return false;

        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);
        if (angleToPlayer > viewAngle / 2f) return false;

        if (Physics.Raycast(origin, directionToPlayer, out RaycastHit hit, detectionRange))
        {
            // Debug.DrawRay(origin, directionToPlayer * detectionRange, Color.red);
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    private IEnumerator ScoldPlayer()
    {
        stopWalking = true;
        EnableWalking(false);
        agent.ResetPath();

        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        animator.SetTrigger("attack");
        player.GetComponent<PlayerScript>()?.PlayerCaught(false);
        dogInHand.SetActive(true);

        yield return new WaitForSeconds(4f);

        isChasing = false;
    }

    public void MakeGrandpaSit(Transform sitPos)
    {
        isSitting = true;
        transform.position = sitPos.position;
        transform.rotation = sitPos.rotation;

        EnableWalking(false);
        stopWalking = true;
        agent.enabled = false;

        StopTheChase();
        animator.SetTrigger("Sit");

        DOVirtual.DelayedCall(2, () =>
        {
            StopTheChase();
            EnableWalking(false);
            stopWalking = true;
            animator.SetTrigger("Sit");
        });
    }

    private void GrandpaFall()
    {
        MainScript.instance.RestartRewardedTimer();
        grandPaCamera.gameObject.SetActive(true);
        grandPaCamera.SetParent(null);

        player.DisablePlayer();
        animator.SetTrigger("Fall");
        EnableWalking(false);
        stopWalking = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        isChasing = false;
        if (faceMat != null) faceMat.color = Color.red;

        Typewriter.instance.StartTyping("Dang it! Slipped again?! This dog's gonna be the end of me!", 3);

        DOVirtual.DelayedCall(4, angryEmote.Play);

        MainScript.instance.pnlInfo.ShowInfo("Grandpa's down. Run while you can");

        DOVirtual.DelayedCall(8, () =>
        {
            if (faceMat != null) faceMat.color = Color.white;
            grandPaCamera.DOMove(player.playerCamera.position, 0.2f);
            grandPaCamera.DORotate(player.playerCamera.eulerAngles, 0.2f).OnComplete(() =>
            {
                GetComponent<Collider>().enabled = true;
                grandPaCamera.gameObject.SetActive(false);
                player.EnablePlayer();
                MainScript.instance.activeLevel.TaskCompleted(1);
            });
        });
    }

    public void StandInjured()
    {
        animator.SetTrigger("InjuredStand");
    }
    private void GrandpaSlip()
    {
        MainScript.instance.RestartRewardedTimer();


        animator.SetTrigger("Slip");
        EnableWalking(false);
        stopWalking = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        isChasing = false;
        if (faceMat != null) faceMat.color = Color.red;

        Typewriter.instance.StartTyping("Dang it! Slipped again?! This dog's gonna be the end of me!", 3);

        DOVirtual.DelayedCall(4, angryEmote.Play);

        MainScript.instance.pnlInfo.ShowInfo("Grandpa's down. Run while you can");

        DOVirtual.DelayedCall(4, () =>
        {
            if (faceMat != null) faceMat.color = Color.white;

            GetComponent<Collider>().enabled = true;

            ChasePlayerForDuration(1);
        });
    }

    public void MakeGrandpaAngry()
    {
        if (isSitting)
        {
            MainScript.instance.pnlInfo.ShowInfo("Don't disturb grandpa when he is resting");
            return;
        }

        if (isChasing)
            StopTheChase();
        transform.LookAt(player.transform);
        MainScript.instance.RestartRewardedTimer();
        Debug.Log("Angryyyyy");

        animator.SetTrigger("Angry");
        EnableWalking(false);
        stopWalking = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        if (faceMat != null) faceMat.color = Color.red;

        Typewriter.instance.StartTyping("Ouch! You naughty dog, I’ll catch you!", 1);

        DOVirtual.DelayedCall(4, angryEmote.Play);

        MainScript.instance.pnlInfo.ShowInfo("You made Grandpa angry, he’s coming after you!");

        DOVirtual.DelayedCall(3, () =>
        {
            if (faceMat != null) faceMat.color = Color.white;
            GetComponent<Collider>().enabled = true;
            ChasePlayerForDuration(1);
        });
    }

    public void ElectrocuteGrandpa()
    {
        if (isSitting)
        {
            MainScript.instance.pnlInfo.ShowInfo("Don't disturb grandpa when he is resting");
            return;
        }

        if (isChasing)
            StopTheChase();
        
        SoundManager.instance.PlaySound(SoundManager.instance.electrocute);
        
        transform.LookAt(player.transform);
        MainScript.instance.RestartRewardedTimer();
        Debug.Log("electrocuted");
        electrocutedParticle.SetActive(true);
        animator.SetTrigger("Electrocuted");
        EnableWalking(false);
        stopWalking = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false; //so no other thing get hit and exeuted

        if (faceMat != null) faceMat.color = Color.red;

        Typewriter.instance.StartTyping("Ouch! You naughty dog, I’ll catch you!", 1);

        // DOVirtual.DelayedCall(8, angryEmote.Play);

        MainScript.instance.pnlInfo.ShowInfo("You made Grandpa angry, he’s coming after you!");

        DOVirtual.DelayedCall(4, () =>
        {
            electrocutedParticle.SetActive(false);
            if (faceMat != null) faceMat.color = Color.white;
            GetComponent<Collider>().enabled = true;
            ChasePlayerForDuration(1);
        });
    }

    private void MakeGrandpaRun()
    {
        Debug.Log("isChasing = " + isChasing);
        if (isChasing)
            StopTheChase();
        isRunning = true;
        animator.SetBool("isRunning", true);
        float runDuration = 3;
        EnableWalking(false);
        waypoints = waypointsGroundFloor2;
        agent.speed = 2.5f;
        DOVirtual.DelayedCall(6, () =>
        {
            if (GlobalValues.currentStage == 1)
                waypoints = waypointsGroundFloor;
            else
                waypoints = waypointsGroundFloor2;
            isRunning = false;
            agent.speed = 0.7f;
            // EnableWalking(true);
            animator.SetBool("isRunning", false);
            ChasePlayerForDuration(1);
            beesParticle.SetActive(false);
        });
    }

    #endregion

    #region Helpers & Triggers

    public bool IsOnFirstFloor() => transform.position.y > floorHeightThreshold;
    public bool IsOnGroundFloor() => transform.position.y <= floorHeightThreshold;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
            GrandpaFall();
        // if (other.gameObject.TryGetComponent<Pickable>(out Pickable pickable))
        // {
        //     MakeGrandpaAngry();
        //     StopTheChase();
        // }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<Pickable>(out Pickable pickable))
        {
            ContactPoint contact = other.contacts[0];

            // Place particle a tiny bit outward along the surface normal
            hitParticle.transform.position = contact.point + contact.normal * 0.01f;
            hitParticle.transform.rotation = Quaternion.LookRotation(contact.normal);

            hitParticle.Play();
            MainScript.instance.RestartRewardedTimer();
            if (isSitting)
            {
                MainScript.instance.pnlInfo.ShowInfo("Don't disturb grandpa when he is resting");
                return;
            }
            else if (other.gameObject.CompareTag("Lizard"))
                MakeGrandpaRun();
            else if (other.gameObject.CompareTag("Beehive"))
            {
                beesParticle.SetActive(true);
                MakeGrandpaRun();
            }
            else if (other.gameObject.CompareTag("ShockGun"))
                ElectrocuteGrandpa();
            else if (other.gameObject.CompareTag("BananaPeel"))
                GrandpaSlip();
            else
                MakeGrandpaAngry();
        }
    }

    private void EnableWalking(bool enable)
    {
         // Debug.Log(enable + " Walking");

        animator.SetBool("isWalking", enable);
    }

    public void NotifyPlayerHasThrown()
    {
    }

    #endregion
}


// using DG.Tweening;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AI;
//
// public class GrandpaAI : MonoBehaviour
// {
//     public GameObject dogInHand;
//     public bool isSitting;
//     [SerializeField] private Transform chaseEndPoint;
//     public float chaseEndThreshold = 1f;
//     [SerializeField] ParticleSystem angryEmote;
//     [SerializeField] bool scoldPlayerWhenSee;
//     [SerializeField] bool shouldCatchPlayer = true;
//     [SerializeField] List<Transform> waypointsGroundFloor, waypointsFirstFloor;
//     [SerializeField] Transform grandPaCamera;
//     [SerializeField] Material faceMat;
//     public List<Transform> waypoints;
//     public float[] waitDurations;
//     public float reachThreshold = 0.5f;
//     public float detectionRange = 10f;
//     public float catchRange = 2.5f;
//     [SerializeField] private NavMeshAgent agent;
//     private int currentIndex = 0;
//     private int direction = 1;
//     private bool isWaiting = false;
//     private bool isChasing = false;
//     private PlayerScript player;
//   
//     [SerializeField] private Animator animator;
//     [Range(0, 180)]
//     public float viewAngle = 180;
//     bool stopWalking;
//     private bool isChasingForDuration; // New: Track duration-based chase
//     private float chaseTimer; // New: Timer for duration-based chase
//
//     [SerializeField] private float floorHeightThreshold = 3f; // adjust in Inspector
//
//     public bool IsOnFirstFloor()
//     {
//         return transform.position.y > floorHeightThreshold;
//     }
//
//     public bool IsOnGroundFloor()
//     {
//         return transform.position.y <= floorHeightThreshold;
//     }
//
//     void Start()
//     {
//         player = MainScript.instance.player;
//         StartPatrolOnGroundFloor();
//         if (waypoints == null || waypoints.Count < 2)
//         {
//             Debug.LogError("Need at least 2 waypoints.");
//             enabled = false;
//             return;
//         }
//         if (scoldPlayerWhenSee)
//         {
//             catchRange = 5;
//         }
//         MoveToCurrentWaypoint();
//     }
//
//     void Update()
//     {
//         
//         if (IsOnFirstFloor())
//         {
//             Debug.Log("Grandpa is on the FIRST floor");
//         }
//         else
//         {
//             Debug.Log("Grandpa is on the GROUND floor");
//         }
//         if (player == null || stopWalking) return;
//
//
//
//         if (isChasingForDuration)
//         {
//             chaseTimer -= Time.deltaTime;
//
//             // ✅ Stop if Grandpa reaches the end point (like a door)
//             if (chaseEndPoint != null && Vector3.Distance(transform.position, chaseEndPoint.position) <= chaseEndThreshold)
//             {
//                 StopTheChase();
//                 //chaseTimer = 0;
//                 //isChasingForDuration = false;
//                 //MoveToCurrentWaypoint();
//                 return;
//             }
//
//             // ✅ Stop if timer runs out
//             if (chaseTimer <= 0)
//             {
//                 StopTheChase();
//                 //isChasingForDuration = false;
//                 //MoveToCurrentWaypoint();
//                 return;
//             }
//
//             // ✅ Chase the player
//             if (agent.enabled)
//             {
//                 agent.SetDestination(player.transform.position);
//                 EnableWalking(true);
//                 float playerrDistance = Vector3.Distance(transform.position + Vector3.up * 1, player.transform.position);
//                 if (playerrDistance <= catchRange && CanSeePlayer())
//                 {
//                     StartCoroutine(ScoldPlayer());
//                     isChasingForDuration = false;
//                 }
//             }
//
//             return;
//         }
//
//
//
//
//         //if (isChasingForDuration)
//         //{
//         //    chaseTimer -= Time.deltaTime;
//         //    if (chaseTimer <= 0)
//         //    {
//         //        isChasingForDuration = false;
//         //        MoveToCurrentWaypoint();
//         //        return;
//         //    }
//         //    else if (agent.enabled)
//         //    {
//         //        agent.SetDestination(player.transform.position);
//         //        EnableWalking(true);
//         //        float playerDistancee = Vector3.Distance(transform.position + Vector3.up * 1, player.transform.position);
//         //        if (playerDistancee <= catchRange && CanSeePlayer())
//         //        {
//         //            StartCoroutine(ScoldPlayer());
//         //            isChasingForDuration = false; // Stop chase when caught
//         //        }
//         //    }
//         //    if (chaseEndPoint != null && Vector3.Distance(transform.position, chaseEndPoint.position) <= chaseEndThreshold)
//         //    {
//         //        chaseTimer = 0;
//         //        isChasingForDuration = false;
//         //        MoveToCurrentWaypoint();
//         //        return;
//         //    }
//         //    return;
//         //}
//
//         float playerDistance = Vector3.Distance(transform.position + Vector3.up * 1, player.transform.position);
//         if (shouldCatchPlayer /*&& playerHasThrown*/ && playerDistance <= detectionRange)
//         {
//             ChasePlayer(playerDistance);
//         }
//         else
//         {
//             if (isChasing)
//             {
//                 isChasing = false;
//                 EnableWalking(true);
//                 MoveToCurrentWaypoint();
//             }
//             Patrol();
//         }
//         if (scoldPlayerWhenSee && CanSeePlayer())
//         {
//             ChasePlayer(playerDistance);
//         }
//     }
//
//     public void StartPatrolOnFirstFloor()
//     {
//         Debug.Log("fitstFloor");
//         waypoints = waypointsFirstFloor;
//         EnableWalking(true);
//         stopWalking = false;
//         agent.enabled = true;
//         shouldCatchPlayer = false;
//     }
//
//     public void StartPatrolOnGroundFloor()
//     {
//         waypoints = waypointsGroundFloor;
//         EnableWalking(true);
//         stopWalking = false;
//         agent.enabled = true;
//         shouldCatchPlayer = false;
//     }
//
//     void Patrol()
//     {
//         if (isWaiting || agent.pathPending) return;
//         if (agent.remainingDistance <= reachThreshold)
//         {
//             StartCoroutine(WaitAtWaypoint());
//         }
//     }
//
//     IEnumerator WaitAtWaypoint()
//     {
//         isWaiting = true;
//         EnableWalking(false);
//         float waitTime = waitDurations.Length > currentIndex ? waitDurations[currentIndex] : 1f;
//         yield return new WaitForSeconds(waitTime);
//         GetNextWaypoint();
//         MoveToCurrentWaypoint();
//         EnableWalking(true);
//         isWaiting = false;
//     }
//
//     void GetNextWaypoint()
//     {
//         currentIndex += direction;
//         if (currentIndex >= waypoints.Count)
//         {
//             direction = -1;
//             currentIndex = waypoints.Count - 2;
//         }
//         else if (currentIndex < 0)
//         {
//             direction = 1;
//             currentIndex = 1;
//         }
//     }
//
//     void MoveToCurrentWaypoint()
//     {
//         //if (agent.enabled)
//         //{
//         //    if (waypoints.Count > 0)
//         //    {
//         //        if (currentIndex >= waypoints.Count)
//         //            currentIndex = waypoints.Count - 1;
//         //        agent.SetDestination(waypoints[currentIndex].position);
//         //    }
//         //    EnableWalking(true);
//         //}
//
//
//         if (agent.enabled && agent.isOnNavMesh)
//         {
//             if (waypoints.Count > 0)
//             {
//                 if (currentIndex >= waypoints.Count)
//                     currentIndex = waypoints.Count - 1;
//                 agent.SetDestination(waypoints[currentIndex].position);
//             }
//             EnableWalking(true);
//         }
//
//     }
//
//     void ChasePlayer(float distance)
//     {
//         if (agent.enabled)
//         {
//             isChasing = true;
//             EnableWalking(true);
//             agent.SetDestination(player.transform.position);
//             if (distance <= catchRange)
//             {
//                 if (CanSeePlayer())
//                 {
//                     StartCoroutine(ScoldPlayer());
//                 }
//             }
//         }
//     }
//
//     bool CanSeePlayer()
//     {
//         Vector3 eyeOffset = Vector3.up * 1.3f; // Eye level 1.3m above feet
//         Vector3 origin = transform.position + eyeOffset;
//         Vector3 target = player.transform.position;
//         Vector3 directionToPlayer = (target - origin).normalized;
//         float distance = Vector3.Distance(origin, target);
//         if (distance > detectionRange)
//             return false;
//         // Check if player is within view angle
//         float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);
//         if (angleToPlayer > viewAngle / 2f)
//             return false;
//         // Check if anything blocks the view
//         if (Physics.Raycast(origin, directionToPlayer, out RaycastHit hit, detectionRange))
//         {
//             Debug.Log(hit.transform.name);
//             Debug.DrawRay(origin, directionToPlayer * detectionRange, Color.red);
//             return hit.collider.CompareTag("Player");
//         }
//         return false;
//     }
//
//     IEnumerator ScoldPlayer()
//     {
//         stopWalking = true;
//         EnableWalking(false);
//         agent.ResetPath();
//         transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
//         animator.SetTrigger("attack");
//         player.GetComponent<PlayerScript>()?.PlayerCaught();
//         dogInHand.SetActive(true);
//         yield return new WaitForSeconds(4f); // Pause after killing before returning to patrol
//         isChasing = false;
//         // playerHasThrown = false;
//     }
//
//     public void NotifyPlayerHasThrown()
//     {
//         // playerHasThrown = true;
//     }
//
//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Water"))
//         {
//             GrandpaFall();
//         }
//     }
//
//     private void EnableWalking(bool enable)
//     {
//         animator.SetBool("isWalking", enable);
//     }
//
//     public void MakeGrandpaSit(Transform sitPos)
//     {
//         isSitting = true;
//         transform.position = sitPos.position;
//         transform.rotation = sitPos.rotation;
//         EnableWalking(false);
//         stopWalking = true;
//         agent.enabled = false;
//         StopTheChase();
//         animator.SetTrigger("Sit");
//         StopTheChase();
//         DOVirtual.DelayedCall(2, () =>
//         {
//             StopTheChase();
//             EnableWalking(false);
//             stopWalking = true;
//             animator.SetTrigger("Sit");  // force sit
//         });
//     }
//
//     private void GrandpaFall()
//     {
//         MainScript.instance.RestartRewardedTimer();
//         grandPaCamera.gameObject.SetActive(true);
//         grandPaCamera.SetParent(null);
//         player.DisablePlayer();
//         animator.SetTrigger("Fall");
//         EnableWalking(false);
//         stopWalking = true;
//         agent.enabled = false;
//         GetComponent<Collider>().enabled = false;
//         //MainScript.instance.activeLevel.TaskCompleted(1);
//
//         if (faceMat != null)
//             faceMat.color = Color.red;
//
//         Typewriter.instance.StartTyping("Dang it! Slipped again?! This dog's gonna be the end of me!", 3);
//         DOVirtual.DelayedCall(4, () =>
//         {
//             angryEmote.Play();
//         });
//
//         MainScript.instance.pnlInfo.ShowInfo("Grandpa's down. Run while you can");
//         DOVirtual.DelayedCall(8, () =>
//         {
//             if (faceMat != null)
//                 faceMat.color = Color.white;
//             grandPaCamera.DOMove(player.playerCamera.position, 0.2f);
//             grandPaCamera.DORotate(player.playerCamera.eulerAngles, 0.2f).OnComplete(() =>
//             {
//                 grandPaCamera.gameObject.SetActive(false);
//                 player.EnablePlayer();
//                 //StartTheChase();
//                 MainScript.instance.activeLevel.TaskCompleted(1);
//                 //ChasePlayerForDuration(30);
//             });
//         });
//     }
//
//     public void StartTheChase()
//     {
//         MainScript.instance.pnlInfo.ShowInfo("Grandpa is coming to catch you");
//         EnableWalking(true);
//         stopWalking = false;
//         agent.enabled = true;
//         shouldCatchPlayer = true;
//     }
//
//     public void StopTheChase()
//     {
//        
//         if (shouldCatchPlayer || isChasingForDuration)
//             MainScript.instance.pnlInfo.ShowInfo("Grandpa gave up the chase.");
//         shouldCatchPlayer = false;
//         chaseTimer = 0;
//         isChasingForDuration = false;
//         waypoints = waypointsGroundFloor;
//         MoveToCurrentWaypoint();
//     }
//
//     public void ChasePlayerForDuration(float duration)
//     {
//         if (isSitting)
//         {
//             Debug.Log(isSitting);
//             return;
//         }
//         MainScript.instance.pnlInfo.ShowInfo("Grandpa’s on the move, better stay out of sight!");
//         //isChasingForDuration = true;
//         chaseTimer = duration;
//         //EnableWalking(true);
//         //stopWalking = false;
//         //agent.enabled = true;
//         StartChase();
//         DOVirtual.DelayedCall(2.2f, () =>
//         {
//             StartChase();
//         });
//     }
//
//     private void StartChase()
//     {
//         isChasingForDuration = true;
//        
//         stopWalking = false;
//         agent.enabled = true;
//         EnableWalking(true);   //force walk
//     }
// }
//
//


//using DG.Tweening;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class GrandpaAI : MonoBehaviour
//{
//    public GameObject dogInHand;
//    [SerializeField] bool scoldPlayerWhenSee;
//    [SerializeField] bool shouldCatchPlayer = true;

//    [SerializeField] List<Transform> waypointsGroundFloor, waypointsFirstFloor;
//    [SerializeField] Transform grandPaCamera;
//    //[SerializeField] LayerMask playerLayerMask;
//    public List<Transform> waypoints;
//    public float[] waitDurations;
//    public float reachThreshold = 0.5f;
//    public float detectionRange = 10f;
//    public float catchRange = 2.5f;

//    private NavMeshAgent agent;
//    private int currentIndex = 0;
//    private int direction = 1;
//    private bool isWaiting = false;
//    private bool isChasing = false;
//    private PlayerScript player;
//    private bool playerHasThrown = false;
//    private Animator animator;
//    [Range(0, 180)]
//    public float viewAngle = 180;

//    bool stopWalking;

//    private void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        animator = GetComponent<Animator>();
//    }
//    void Start()
//    {

//        player = MainScript.instance.player;
//        StartPatrolOnGroundFloor();
//        if (waypoints == null || waypoints.Count < 2)
//        {
//            Debug.LogError("Need at least 2 waypoints.");
//            enabled = false;
//            return;
//        }

//        if (scoldPlayerWhenSee)
//        {
//            catchRange = 5;
//        }


//        MoveToCurrentWaypoint();

//    }

//    void Update()
//    {
//        if (player == null || stopWalking) return;

//        float playerDistance = Vector3.Distance(transform.position + Vector3.up * 1, player.transform.position);

//        if (shouldCatchPlayer /*&& playerHasThrown*/ && playerDistance <= detectionRange)
//        {
//            ChasePlayer(playerDistance);
//        }

//        else
//        {
//            if (isChasing)
//            {
//                isChasing = false;
//                EnableWalking(true);
//                MoveToCurrentWaypoint();
//            }

//            Patrol();
//        }
//        if (scoldPlayerWhenSee && CanSeePlayer())
//        {
//            ChasePlayer(playerDistance);
//        }
//    }


//    public void StartPatrolOnFirstFloor()
//    {
//        Debug.Log("fitstFloor");
//        shouldCatchPlayer = true;
//        waypoints = waypointsFirstFloor;
//    }

//    public void StartPatrolOnGroundFloor()
//    {
//        shouldCatchPlayer = false;
//        waypoints = waypointsGroundFloor;

//        EnableWalking(true);
//        stopWalking = false;
//        agent.enabled = true;
//        shouldCatchPlayer = false;

//    }
//    void Patrol()
//    {
//        if (isWaiting || agent.pathPending) return;

//        if (agent.remainingDistance <= reachThreshold)
//        {
//            StartCoroutine(WaitAtWaypoint());
//        }
//    }

//    IEnumerator WaitAtWaypoint()
//    {
//        isWaiting = true;
//        EnableWalking(false);

//        float waitTime = waitDurations.Length > currentIndex ? waitDurations[currentIndex] : 1f;
//        yield return new WaitForSeconds(waitTime);

//        GetNextWaypoint();
//        MoveToCurrentWaypoint();

//        EnableWalking(true);
//        isWaiting = false;
//    }

//    void GetNextWaypoint()
//    {
//        currentIndex += direction;

//        if (currentIndex >= waypoints.Count)
//        {
//            direction = -1;
//            currentIndex = waypoints.Count - 2;
//        }
//        else if (currentIndex < 0)
//        {
//            direction = 1;
//            currentIndex = 1;
//        }
//    }

//    void MoveToCurrentWaypoint()
//    {
//        if (agent.enabled)
//        {
//            if (waypoints.Count > 0)
//            {
//                if (currentIndex >= waypoints.Count)
//                    currentIndex = waypoints.Count - 1;
//                agent.SetDestination(waypoints[currentIndex].position);
//            }
//            EnableWalking(true);
//        }
//    }

//    void ChasePlayer(float distance)
//    {
//        if (agent.enabled)
//        {
//            isChasing = true;
//            EnableWalking(true);
//            agent.SetDestination(player.transform.position);

//            if (distance <= catchRange)
//            {
//                if (CanSeePlayer())
//                {
//                    StartCoroutine(ScoldPlayer());
//                }
//            }

//            //Vector3 sphereCenter = transform.position + transform.forward * 0.5f + Vector3.up * 0.5f;
//            //Collider[] hits = Physics.OverlapSphere(sphereCenter, catchRange, playerLayerMask);


//            //foreach (var hit in hits)
//            //{
//            //    Debug.Log(hit.transform.name);
//            //    if (hit.CompareTag("Player"))
//            //    {
//            //        if (!stopWalking && CanSeePlayer())
//            //        {
//            //            StartCoroutine(ScoldPlayer());
//            //            break; // Exit loop early since we already found and scolded the player
//            //        }

//            //    }
//            //}
//        }
//    }

//    bool CanSeePlayer()
//    {
//        Vector3 eyeOffset = Vector3.up * 1.3f; // Eye level 1.3m above feet
//        Vector3 origin = transform.position + eyeOffset;
//        Vector3 target = player.transform.position;
//        Vector3 directionToPlayer = (target - origin).normalized;
//        float distance = Vector3.Distance(origin, target);

//        if (distance > detectionRange)
//            return false;

//        // Check if player is within view angle
//        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);
//        if (angleToPlayer > viewAngle / 2f)
//            return false;

//        // Check if anything blocks the view
//        if (Physics.Raycast(origin, directionToPlayer, out RaycastHit hit, detectionRange))
//        {
//            Debug.Log(hit.transform.name);
//            Debug.DrawRay(origin, directionToPlayer * detectionRange, Color.red);
//            return hit.collider.CompareTag("Player");
//        }

//        return false;
//    }

//    //bool CanSeePlayer()
//    //{
//    //    Vector3 origin = transform.position;
//    //    Vector3 target = player.position;
//    //    Vector3 directionToPlayer = (target - origin).normalized;
//    //    float distance = Vector3.Distance(origin, target);

//    //    if (distance > detectionRange)
//    //        return false;

//    //    // Check if player is within view angle
//    //    float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
//    //    if (angleToPlayer > viewAngleLeft / 2f)
//    //        return false;

//    //    // Check if anything blocks the view
//    //    if (Physics.Raycast(origin, directionToPlayer, out RaycastHit hit, detectionRange))
//    //    {
//    //        Debug.Log(hit.transform.name);
//    //        Debug.DrawRay(origin, directionToPlayer * detectionRange, Color.red);
//    //        return hit.collider.CompareTag("Player");
//    //    }

//    //    return false;
//    //}


//    IEnumerator ScoldPlayer()
//    {
//        stopWalking = true;
//        EnableWalking(false);
//        agent.ResetPath();

//        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
//        animator.SetTrigger("attack");

//        player.GetComponent<PlayerScript>()?.PlayerCaught();
//        dogInHand.SetActive(true);

//        yield return new WaitForSeconds(4f); // Pause after killing before returning to patrol

//        isChasing = false;
//        playerHasThrown = false;
//        //MoveToCurrentWaypoint();
//        //animator.SetBool("isWalking", true);
//    }


//    public void NotifyPlayerHasThrown()
//    {
//        playerHasThrown = true;
//    }

//    //void OnDrawGizmosSelected()
//    //{
//    //    // Detection range
//    //    Gizmos.color = Color.red;
//    //    Gizmos.DrawWireSphere(transform.position, detectionRange);

//    //    // Catch range (3D sphere)
//    //    Gizmos.color = Color.yellow;
//    //    Gizmos.DrawWireSphere(transform.position + Vector3.up * 1, catchRange);

//    //    // Field of view
//    //    Vector3 origin = transform.position + Vector3.up * 1.3f;
//    //    Vector3 target = player.position;
//    //    Vector3 directionToPlayer = (target - origin).normalized;

//    //    Quaternion leftRayRotation = Quaternion.Euler(0, -viewAngle / 2f, 0);
//    //    Quaternion rightRayRotation = Quaternion.Euler(0, viewAngle / 2f, 0);

//    //    Vector3 leftRay = leftRayRotation * directionToPlayer;
//    //    Vector3 rightRay = rightRayRotation * directionToPlayer;

//    //    Gizmos.color = Color.green;
//    //    Gizmos.DrawRay(origin, leftRay * detectionRange);
//    //    Gizmos.DrawRay(origin, rightRay * detectionRange);
//    //}


//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Water"))
//        {
//            GrandpaFall();
//        }
//    }

//    private void EnableWalking(bool enable)
//    {
//        //Debug.Log("Walk" + enable);


//        animator.SetBool("isWalking", enable);
//    }

//    public void MakeGrandpaSit(Transform sitPos)
//    {
//        transform.position = sitPos.position;
//        transform.rotation = sitPos.rotation;
//        EnableWalking(false);
//        stopWalking = true;
//        agent.enabled = false;
//        StopTheChase();
//        animator.SetTrigger("Sit");

//        DOVirtual.DelayedCall(1, () =>
//        {
//            animator.SetTrigger("Sit");
//            EnableWalking(false);
//        });
//    }

//    private void GrandpaFall()
//    {
//        grandPaCamera.gameObject.SetActive(true);
//        grandPaCamera.SetParent(null);
//        player.DisablePlayer();
//        animator.SetTrigger("Fall");
//        EnableWalking(false);
//        stopWalking = true;
//        agent.enabled = false;
//        GetComponent<Collider>().enabled = false;
//        MainScript.instance.activeLevel.TaskCompleted(1);
//        MainScript.instance.pnlInfo.ShowInfo("Grandpa's down. Run while you can");

//        DOVirtual.DelayedCall(8, () =>
//        {
//            grandPaCamera.DOMove(player.playerCamera.position, 0.2f);
//            grandPaCamera.DORotate(player.playerCamera.eulerAngles, 0.2f).OnComplete(() =>
//            {
//                grandPaCamera.gameObject.SetActive(false);
//                player.EnablePlayer();
//                StartTheChase();
//            });

//        });
//    }


//    public void StartTheChase()
//    {
//        MainScript.instance.pnlInfo.ShowInfo("Grandpa is coming to catch you");
//        EnableWalking(true);
//        stopWalking = false;
//        agent.enabled = true;
//        shouldCatchPlayer = true;
//    }

//    public void StopTheChase()
//    {
//        shouldCatchPlayer = false;
//    }
//}