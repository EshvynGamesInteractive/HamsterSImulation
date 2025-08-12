using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrandpaAI : MonoBehaviour
{
    public GameObject dogInHand;
    public bool isSitting;
    [SerializeField] private Transform chaseEndPoint;
    public float chaseEndThreshold = 1f;
    [SerializeField] ParticleSystem angryEmote;
    [SerializeField] bool scoldPlayerWhenSee;
    [SerializeField] bool shouldCatchPlayer = true;
    [SerializeField] List<Transform> waypointsGroundFloor, waypointsFirstFloor;
    [SerializeField] Transform grandPaCamera;
    [SerializeField] Material faceMat;
    public List<Transform> waypoints;
    public float[] waitDurations;
    public float reachThreshold = 0.5f;
    public float detectionRange = 10f;
    public float catchRange = 2.5f;
    [SerializeField] private NavMeshAgent agent;
    private int currentIndex = 0;
    private int direction = 1;
    private bool isWaiting = false;
    private bool isChasing = false;
    private PlayerScript player;
    private bool playerHasThrown = false;
    [SerializeField] private Animator animator;
    [Range(0, 180)]
    public float viewAngle = 180;
    bool stopWalking;
    private bool isChasingForDuration; // New: Track duration-based chase
    private float chaseTimer; // New: Timer for duration-based chase



    void Start()
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
        {
            catchRange = 5;
        }
        MoveToCurrentWaypoint();
    }

    void Update()
    {
        if (player == null || stopWalking) return;



        if (isChasingForDuration)
        {
            chaseTimer -= Time.deltaTime;

            // ✅ Stop if Grandpa reaches the end point (like a door)
            if (chaseEndPoint != null && Vector3.Distance(transform.position, chaseEndPoint.position) <= chaseEndThreshold)
            {
                StopTheChase();
                //chaseTimer = 0;
                //isChasingForDuration = false;
                //MoveToCurrentWaypoint();
                return;
            }

            // ✅ Stop if timer runs out
            if (chaseTimer <= 0)
            {
                StopTheChase();
                //isChasingForDuration = false;
                //MoveToCurrentWaypoint();
                return;
            }

            // ✅ Chase the player
            if (agent.enabled)
            {
                agent.SetDestination(player.transform.position);
                EnableWalking(true);
                float playerrDistance = Vector3.Distance(transform.position + Vector3.up * 1, player.transform.position);
                if (playerrDistance <= catchRange && CanSeePlayer())
                {
                    StartCoroutine(ScoldPlayer());
                    isChasingForDuration = false;
                }
            }

            return;
        }




        //if (isChasingForDuration)
        //{
        //    chaseTimer -= Time.deltaTime;
        //    if (chaseTimer <= 0)
        //    {
        //        isChasingForDuration = false;
        //        MoveToCurrentWaypoint();
        //        return;
        //    }
        //    else if (agent.enabled)
        //    {
        //        agent.SetDestination(player.transform.position);
        //        EnableWalking(true);
        //        float playerDistancee = Vector3.Distance(transform.position + Vector3.up * 1, player.transform.position);
        //        if (playerDistancee <= catchRange && CanSeePlayer())
        //        {
        //            StartCoroutine(ScoldPlayer());
        //            isChasingForDuration = false; // Stop chase when caught
        //        }
        //    }
        //    if (chaseEndPoint != null && Vector3.Distance(transform.position, chaseEndPoint.position) <= chaseEndThreshold)
        //    {
        //        chaseTimer = 0;
        //        isChasingForDuration = false;
        //        MoveToCurrentWaypoint();
        //        return;
        //    }
        //    return;
        //}

        float playerDistance = Vector3.Distance(transform.position + Vector3.up * 1, player.transform.position);
        if (shouldCatchPlayer /*&& playerHasThrown*/ && playerDistance <= detectionRange)
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
        {
            ChasePlayer(playerDistance);
        }
    }

    public void StartPatrolOnFirstFloor()
    {
        Debug.Log("fitstFloor");
        waypoints = waypointsFirstFloor;
        EnableWalking(true);
        stopWalking = false;
        agent.enabled = true;
        shouldCatchPlayer = false;
    }

    public void StartPatrolOnGroundFloor()
    {
        waypoints = waypointsGroundFloor;
        EnableWalking(true);
        stopWalking = false;
        agent.enabled = true;
        shouldCatchPlayer = false;
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
        EnableWalking(false);
        float waitTime = waitDurations.Length > currentIndex ? waitDurations[currentIndex] : 1f;
        yield return new WaitForSeconds(waitTime);
        GetNextWaypoint();
        MoveToCurrentWaypoint();
        EnableWalking(true);
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
        //if (agent.enabled)
        //{
        //    if (waypoints.Count > 0)
        //    {
        //        if (currentIndex >= waypoints.Count)
        //            currentIndex = waypoints.Count - 1;
        //        agent.SetDestination(waypoints[currentIndex].position);
        //    }
        //    EnableWalking(true);
        //}


        if (agent.enabled && agent.isOnNavMesh)
        {
            if (waypoints.Count > 0)
            {
                if (currentIndex >= waypoints.Count)
                    currentIndex = waypoints.Count - 1;
                agent.SetDestination(waypoints[currentIndex].position);
            }
            EnableWalking(true);
        }

    }

    void ChasePlayer(float distance)
    {
        if (agent.enabled)
        {
            isChasing = true;
            EnableWalking(true);
            agent.SetDestination(player.transform.position);
            if (distance <= catchRange)
            {
                if (CanSeePlayer())
                {
                    StartCoroutine(ScoldPlayer());
                }
            }
        }
    }

    bool CanSeePlayer()
    {
        Vector3 eyeOffset = Vector3.up * 1.3f; // Eye level 1.3m above feet
        Vector3 origin = transform.position + eyeOffset;
        Vector3 target = player.transform.position;
        Vector3 directionToPlayer = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);
        if (distance > detectionRange)
            return false;
        // Check if player is within view angle
        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);
        if (angleToPlayer > viewAngle / 2f)
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
        stopWalking = true;
        EnableWalking(false);
        agent.ResetPath();
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        animator.SetTrigger("attack");
        player.GetComponent<PlayerScript>()?.PlayerCaught();
        dogInHand.SetActive(true);
        yield return new WaitForSeconds(4f); // Pause after killing before returning to patrol
        isChasing = false;
        playerHasThrown = false;
    }

    public void NotifyPlayerHasThrown()
    {
        playerHasThrown = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            GrandpaFall();
        }
    }

    private void EnableWalking(bool enable)
    {
        animator.SetBool("isWalking", enable);
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
        StopTheChase();
        DOVirtual.DelayedCall(2, () =>
        {
            StopTheChase();
            EnableWalking(false);
            stopWalking = true;
            animator.SetTrigger("Sit");  // force sit
        });
    }

    private void GrandpaFall()
    {
        grandPaCamera.gameObject.SetActive(true);
        grandPaCamera.SetParent(null);
        player.DisablePlayer();
        animator.SetTrigger("Fall");
        EnableWalking(false);
        stopWalking = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        //MainScript.instance.activeLevel.TaskCompleted(1);

        if (faceMat != null)
            faceMat.color = Color.red;

        Typewriter.instance.StartTyping("Dang it! Slipped again?! This dog's gonna be the end of me!", 3);
        DOVirtual.DelayedCall(4, () =>
        {
            angryEmote.Play();
        });

        MainScript.instance.pnlInfo.ShowInfo("Grandpa's down. Run while you can");
        DOVirtual.DelayedCall(8, () =>
        {
            if (faceMat != null)
                faceMat.color = Color.white;
            grandPaCamera.DOMove(player.playerCamera.position, 0.2f);
            grandPaCamera.DORotate(player.playerCamera.eulerAngles, 0.2f).OnComplete(() =>
            {
                grandPaCamera.gameObject.SetActive(false);
                player.EnablePlayer();
                //StartTheChase();
                MainScript.instance.activeLevel.TaskCompleted(1);
                //ChasePlayerForDuration(30);
            });
        });
    }

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
        shouldCatchPlayer = false;
        chaseTimer = 0;
        isChasingForDuration = false;
        waypoints = waypointsGroundFloor;
        MoveToCurrentWaypoint();
    }

    public void ChasePlayerForDuration(float duration)
    {
        if (isSitting)
        {
            Debug.Log(isSitting);
            return;
        }
        MainScript.instance.pnlInfo.ShowInfo("Grandpa’s on the move, better stay out of sight!");
        //isChasingForDuration = true;
        chaseTimer = duration;
        //EnableWalking(true);
        //stopWalking = false;
        //agent.enabled = true;
        StartChase();
        DOVirtual.DelayedCall(2.2f, () =>
        {
            StartChase();
        });
    }

    private void StartChase()
    {
        isChasingForDuration = true;
       
        stopWalking = false;
        agent.enabled = true;
        EnableWalking(true);   //force walk
    }
}





































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
