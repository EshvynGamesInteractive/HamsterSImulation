using UnityEngine;

public class DogStretchAbility : MonoBehaviour
{
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private LayerMask pantsLayer;
    [SerializeField] private Vector3 stretchedScale = new Vector3(3f, 1f, 3f);
    [SerializeField] private float smoothSpeed = 5f;

    private Transform pantsTarget;
    private Vector3 originalScale;
    private bool isHit;

 

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.green);

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, pantsLayer))
        {
            if (hit.transform.CompareTag("Pants"))
            {
                if (pantsTarget != hit.transform)
                {
                    pantsTarget = hit.transform;
                    originalScale = pantsTarget.localScale;
                }

                isHit = true;
            }
        }
        else
        {
            isHit = false;
        }

        if (pantsTarget != null)
        {
            Vector3 targetScale = isHit ? stretchedScale : originalScale;
            pantsTarget.localScale = Vector3.Lerp(pantsTarget.localScale, targetScale, Time.deltaTime * smoothSpeed);
        }
    }
}
