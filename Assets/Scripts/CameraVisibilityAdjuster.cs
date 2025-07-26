using UnityEngine;

public class CameraVisibilityAdjuster : MonoBehaviour
{
    [SerializeField] private Transform player;       // Player reference
    [SerializeField] private float minDistance = 2f; // How close the camera can get
    [SerializeField] private float maxDistance = 10f; // How far the camera tries to stay
    [SerializeField] private LayerMask obstructionMask; // Layers that can block view

    private Vector3 offsetDirection;
    private float currentDistance;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned to CameraVisibilityAdjuster.");
            enabled = false;
            return;
        }

        offsetDirection = (transform.position - player.position).normalized;
        currentDistance = Vector3.Distance(transform.position, player.position);
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Desired position based on original offset
        Vector3 desiredPos = player.position + offsetDirection * maxDistance;

        // Check for obstruction
        if (Physics.Raycast(player.position, offsetDirection, out RaycastHit hit, maxDistance, obstructionMask))
        {
            // Something is blocking view, so move camera just before it
            float newDist = Mathf.Clamp(hit.distance - 0.5f, minDistance, maxDistance); // keep some buffer
            transform.position = player.position + offsetDirection * newDist;
        }
        else
        {
            // Nothing blocking, keep at max distance
            transform.position = desiredPos;
        }

        // Always look at player
        transform.LookAt(player);
    }
}
