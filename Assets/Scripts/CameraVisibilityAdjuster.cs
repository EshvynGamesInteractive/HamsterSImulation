//using UnityEngine;

//public class CameraVisibilityAdjuster : MonoBehaviour
//{
//    [SerializeField] private Transform player;
//    [SerializeField] private Transform grandpa;
//    [SerializeField] private Transform lookTarget;
//    [SerializeField] private float minDistance = 5f;
//    [SerializeField] private float maxDistance = 20f;
//    [SerializeField] private float zoomSpeed = 5f;
//    [SerializeField] private float edgePadding = 2f;
//    [SerializeField] private float minFOV = 30f;
//    [SerializeField] private float maxFOV = 60f;
//    [SerializeField] private LayerMask obstructionMask;

//    private Camera cam;

//    void Start()
//    {
//        cam = GetComponent<Camera>();
//        if (!cam || !player || !grandpa)
//        {
//            Debug.LogError("Camera, Player or Grandpa missing.");
//            enabled = false;
//            return;
//        }
//        //PositionCameraSideView();
//    }
//    bool lookingAtMid = false;


//    public float sideViewDistance = 3f;
//    public float heightOffset = 1.5f;

//    //public void PositionCameraSideView()
//    //{
//    //    Vector3 midpoint = (player.position + grandpa.position) / 2f;
//    //    Vector3 direction = (grandpa.position - player.position).normalized;

//    //    // Get perpendicular direction on XZ plane
//    //    Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

//    //    // Final camera position: side of the line, at height
//    //    Vector3 cameraPos = midpoint + perpendicular * sideViewDistance + Vector3.up * heightOffset;

//    //    transform.position = cameraPos;
//    //    transform.LookAt(midpoint + Vector3.up * heightOffset);
//    //}





//    void LateUpdate()
//    {
//        Vector3 midPoint = (player.position + grandpa.position) / 2f;
//        Vector3 dirToMid = (transform.position - midPoint).normalized;

//        float distanceBetween = Vector3.Distance(player.position, grandpa.position);
//        float targetDistance = Mathf.Clamp(distanceBetween + edgePadding, minDistance, maxDistance);

//        Vector3 desiredPos = midPoint + dirToMid * targetDistance;

//        // Obstruction check
//        if (Physics.Raycast(midPoint, dirToMid, out RaycastHit hit, targetDistance, obstructionMask))
//        {
//            transform.position = midPoint + dirToMid * (hit.distance - 0.5f);
//        }
//        else
//        {
//            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * zoomSpeed);
//        }

//        // Optional: Zoom FOV based on distance
//        //float targetFOV = Mathf.Lerp(minFOV, maxFOV, distanceBetween / maxDistance);
//        //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);

//        //transform.LookAt(midPoint);
//        //if (!lookingAtMid)
//        //{
//        //    lookingAtMid = true;
//        //}
//    }
//}



















using UnityEngine;

public class CameraVisibilityAdjuster : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform grandpa;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float heightOffset = 1.5f;
    [SerializeField] private float edgePadding = 0.5f;
    [SerializeField] private LayerMask obstructionMask;

    private Camera cam;
    private Vector3 targetCameraPos;

    void Start()
    {
        cam = GetComponent<Camera>();
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