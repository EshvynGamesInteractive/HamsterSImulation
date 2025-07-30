using UnityEngine;
using UnityEngine.UI;

public class GrandpaPeekController : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] Transform headTransform;
    [SerializeField] Transform standPos;

    [Header("Look Settings")]
    [SerializeField] private float lookInterval = 5f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private FP_Controller playerController;

    [Header("Moving Target Settings")]
    [SerializeField] private Transform lookTarget;
    [SerializeField] private Transform fromPoint;
    [SerializeField] private Transform toPoint;
    [SerializeField] private float targetMoveSpeed = 1f;



    [SerializeField] private Transform player;
    [SerializeField] private LayerMask obstructionMask;





    private Vector3 currentTargetPos;
    private bool movingToToPoint = true;

    private float timer;
    private bool isLooking = false;





    [SerializeField] private float coneHeight = 3f;
    [SerializeField] private float coneRadius = 2f;
    [SerializeField] private int coneSegments = 24;
    [SerializeField] private Material coneMaterial; // Assign in Inspector for visibility

    private bool playerCaught;






    private void Start()
    {
        if (lookTarget != null && fromPoint != null)
            lookTarget.position = fromPoint.position;

        currentTargetPos = toPoint.position;

        GenerateLookCone();
    }
    GameObject cone;
    private void GenerateLookCone()
    {
        cone = new GameObject("LookCone");
        cone.transform.SetParent(headTransform);
        cone.transform.localPosition = Vector3.zero;
        cone.transform.localRotation = Quaternion.identity;

        MeshFilter mf = cone.AddComponent<MeshFilter>();
        MeshRenderer mr = cone.AddComponent<MeshRenderer>();
        if (coneMaterial != null)
            mr.material = coneMaterial;

        float coneHeight = viewDistance;
        float coneRadius = Mathf.Tan((viewAngle * 0.5f) * Mathf.Deg2Rad) * coneHeight;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[coneSegments + 2];
        int[] triangles = new int[coneSegments * 3 * 2];

        // Tip
        vertices[0] = Vector3.zero;

        // Base center
        vertices[vertices.Length - 1] = Vector3.forward * coneHeight;

        float angleStep = 2 * Mathf.PI / coneSegments;

        // Base ring
        for (int i = 0; i < coneSegments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * coneRadius;
            float y = Mathf.Sin(angle) * coneRadius;
            vertices[i + 1] = new Vector3(x, y, coneHeight);
        }

        // Side triangles
        for (int i = 0; i < coneSegments; i++)
        {
            int next = (i + 1) % coneSegments;

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = next + 1;
        }

        // Base triangles
        int baseOffset = coneSegments * 3;
        for (int i = 0; i < coneSegments; i++)
        {
            int next = (i + 1) % coneSegments;

            triangles[baseOffset + i * 3] = vertices.Length - 1;
            triangles[baseOffset + i * 3 + 1] = next + 1;
            triangles[baseOffset + i * 3 + 2] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        if (cone != null && cone.activeSelf)
            cone.SetActive(false);
    }





    private void Update()
    {
        if (playerCaught)
        {

            return;
        }

        timer += Time.deltaTime;

        if (!isLooking && timer >= lookInterval)
        {
            StartLooking();
        }

        if (isLooking)
        {
            MoveLookTarget();

            // Check for player movement
            //if (playerController.controller.velocity.magnitude > 0.1f)
            //{
            //    Debug.Log("Level Failed: Grandpa caught you moving!");
            //    // TODO: Add fail logic
            //}
        }

        if (isLooking && CanSeePlayer())
        {
            if (playerController.controller.velocity.magnitude > 0.1f && !playerCaught)
            {
                CatchPlayer();
            }
            //Debug.Log("Level Failed: Grandpa saw you!");
            // TODO: Fail logic
        }
    }

    public void GameStarted()
    {
        gameObject.SetActive(true);
        if (cone != null && !cone.activeSelf)
            cone.SetActive(true);
    }

    public void GameEnd()
    {
        gameObject.SetActive(false);
        if (cone != null && cone.activeSelf)
            cone.SetActive(false);
    }
    private void CatchPlayer()
    {
        playerCaught = true;
        Debug.Log("Level Failed: Grandpa caught you moving!");
        StopLooking();
        animator.SetTrigger("Stand");
        transform.position = standPos.position;

        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0; // Ignore vertical difference
        transform.rotation = Quaternion.LookRotation(lookPos);



        MainScript.instance.player.PlayerCaught();

    }


    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = (player.position - headTransform.position).normalized;
        float distanceToPlayer = Vector3.Distance(headTransform.position, player.position);

        // Angle check
        float angle = Vector3.Angle(headTransform.forward, dirToPlayer);
        if (angle > viewAngle / 2f) return false;

        // Distance check
        if (distanceToPlayer > viewDistance) return false;

        // Line of sight check
        if (Physics.Raycast(headTransform.position, dirToPlayer, out RaycastHit hit, viewDistance, obstructionMask))
        {
            //Debug.Log("Ray hit: " + hit.transform.name); // Debug the hit object

            if (hit.transform == player)
                return true;
        }

        return false;
    }


    private void StartLooking()
    {
        isLooking = true;
        timer = 0f;
        Debug.Log("Grandpa started watching permanently.");
    }


    private void StopLooking()
    {
        isLooking = false;
        Debug.Log("Grandpa stopped watching");
    }

    private void MoveLookTarget()
    {
        if (lookTarget == null || fromPoint == null || toPoint == null) return;

        lookTarget.position = Vector3.MoveTowards(
            lookTarget.position,
            currentTargetPos,
            targetMoveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(lookTarget.position, currentTargetPos) < 0.05f)
        {
            movingToToPoint = !movingToToPoint;
            currentTargetPos = movingToToPoint ? toPoint.position : fromPoint.position;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (isLooking && lookTarget != null && animator)
        {
            animator.SetLookAtWeight(1f);
            animator.SetLookAtPosition(lookTarget.position);
        }
        else
        {
            animator.SetLookAtWeight(0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (headTransform == null) return;

        Gizmos.color = Color.yellow;
        Vector3 forward = headTransform.forward;

        // Draw vision cone
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.DrawRay(headTransform.position, leftBoundary * viewDistance);
        Gizmos.DrawRay(headTransform.position, rightBoundary * viewDistance);

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawWireSphere(headTransform.position, viewDistance);

        if (headTransform == null || player == null) return;

        Gizmos.color = Color.red;
        Vector3 dirToPlayer = (player.position - headTransform.position).normalized;

        Gizmos.DrawRay(headTransform.position, dirToPlayer * viewDistance);
    }

}
