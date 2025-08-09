using UnityEngine;
using DG.Tweening;

public class RotationScript : MonoBehaviour
{
    [Range(0f, 100f)]
    public float rotationSpeed = 75f;
    public float bobAmount = 0.2f;       // how high it moves up/down
    public float bobDuration = 1f;       // time to complete one up or down move

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position;

        // Start the up-and-down loop using DOTween
        transform.DOMoveY(originalPos.y + bobAmount, bobDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void Update()
    {
        // Continuous rotation
        if (rotationSpeed > 0f)
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
