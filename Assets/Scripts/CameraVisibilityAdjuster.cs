using UnityEngine;

public class CameraVisibilityAdjuster : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float heightOffset = 1.5f;
    [SerializeField] private float edgePadding = 0.5f;
    [SerializeField] private LayerMask obstructionMask;

    private Camera cam;
    private Transform grandpa;
    private Vector3 targetCameraPos;


    void Start()
    {
        cam = GetComponent<Camera>();
        GrandpaAI grandpaAI = FindFirstObjectByType<GrandpaAI>();
        if (grandpaAI)
        {
            grandpa = grandpaAI.transform;
        }
        else
        {
            Debug.LogError("No active GrandpaAI found in scene.");
            enabled = false;
            return;
        }

        if (!cam || !player || !grandpa)
        {
            Debug.LogError("Camera, Player, or Grandpa missing.");
            enabled = false;
            return;
        }

        SetInitialCameraPosition();
    }

    void LateUpdate()
    {
        Vector3 lookTarget = grandpa.position + Vector3.up * heightOffset;
        Vector3 toTarget = lookTarget - transform.position;
        float distance = toTarget.magnitude;

        if (Physics.Raycast(transform.position, toTarget.normalized, out RaycastHit hit, distance, obstructionMask))
        {
            // Recalculate position if view to Grandpa is blocked
            SetInitialCameraPosition();
        }

        // Always look at Grandpa
        transform.LookAt(lookTarget);
    }

    void SetInitialCameraPosition()
    {
        Vector3 directionToGrandpa = (grandpa.position - player.position).normalized;
        targetCameraPos = player.position - directionToGrandpa * cameraDistance + Vector3.up * heightOffset;

        if (Physics.Raycast(player.position, (targetCameraPos - player.position).normalized, out RaycastHit hit, cameraDistance + edgePadding, obstructionMask))
        {
            transform.position = player.position + (targetCameraPos - player.position).normalized * (hit.distance - 0.1f);
        }
        else
        {
            transform.position = targetCameraPos;
        }
    }
}






















//using UnityEngine;

//public class CameraVisibilityAdjuster : MonoBehaviour
//{
//    [SerializeField] private Transform player;
//    [SerializeField] private Transform grandpa;
//    [SerializeField] private float cameraDistance = 5f; // Distance behind player
//    [SerializeField] private float heightOffset = 1.5f; // Height above player
//    [SerializeField] private float edgePadding = 0.5f; // Extra distance to keep grandpa in frame
//    [SerializeField] private LayerMask obstructionMask;

//    private Camera cam;

//    void Start()
//    {
//        cam = GetComponent<Camera>();
//        if (!cam || !player || !grandpa)
//        {
//            Debug.LogError("Camera, Player, or Grandpa missing.");
//            enabled = false;
//            return;
//        }
//        PositionCameraBehindPlayer();
//        transform.LookAt(grandpa.position + Vector3.up * heightOffset);
//    }

//    void LateUpdate()
//    {
//        PositionCameraBehindPlayer();
//    }

//    void PositionCameraBehindPlayer()
//    {
//        // Calculate direction from player to grandpa
//        Vector3 directionToGrandpa = (grandpa.position - player.position).normalized;

//        // Position camera behind player, along the line to grandpa
//        Vector3 cameraPos = player.position - directionToGrandpa * cameraDistance + Vector3.up * heightOffset;

//        // Obstruction check from player to camera position
//        if (Physics.Raycast(player.position, (cameraPos - player.position).normalized, out RaycastHit hit, cameraDistance + edgePadding, obstructionMask))
//        {
//            // Move camera closer if obstructed, just before the hit point
//            transform.position = player.position + (cameraPos - player.position).normalized * (hit.distance - 0.1f);
//        }
//        else
//        {
//            transform.position = cameraPos;
//        }

//        // Look at grandpa, with height offset
//        //transform.LookAt(grandpa.position + Vector3.up * heightOffset);
//    }
//}