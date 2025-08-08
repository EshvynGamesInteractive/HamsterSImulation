using UnityEngine;

public class RotationScript : MonoBehaviour
{
    #region Variables
    [Range(0f, 100f)]
    public float rotationSpeed = 75f;
    #endregion


    #region Main Methods
    void Update()
    {
        // rotate prism
        if (rotationSpeed > 0f)
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
    #endregion
}