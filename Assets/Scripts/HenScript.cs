using UnityEngine;

public class HenScript : MonoBehaviour
{
    [SerializeField] Transform[] points;
    [SerializeField] private float moveSpeed = 3f; // Speed for hen movement
    [SerializeField] private string runAnimation = "Run"; // Run animation trigger

    [SerializeField]private Animator animator;
    private int currentPointIndex;
    private bool isMoving;

    void Start()
    {
        if (!animator || points.Length == 0)
        {
            Debug.LogError("Animator or points missing on Hen.");
            enabled = false;
        }
    }

    public void CauseChaos()
    {
        if (isMoving) return;
        isMoving = true;
        animator.SetTrigger(runAnimation);
        currentPointIndex = Random.Range(0, points.Length); // Start at random point
    }

    void Update()
    {
        if (!isMoving) return;

        Vector3 targetPos = points[currentPointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // Face movement direction
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Length; // Loop to next point
        }
    }
}