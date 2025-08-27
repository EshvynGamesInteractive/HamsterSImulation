using UnityEngine;
using System.Collections;

public class HoopTrigger : MonoBehaviour
{
    [SerializeField] private float pushDownForce = 2f; // tweak in Inspector
    [SerializeField] private ParticleSystem confettiParticle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basketball"))
        {
            Debug.Log("Scored!");


            if (other.attachedRigidbody != null)
            {
                other.attachedRigidbody.AddForce(Vector3.down * pushDownForce, ForceMode.Impulse);
            }

            confettiParticle.Play();
            StartCoroutine(DisableTemporarily());


            Level5Script level5 = MainScript.instance.activeLevel.GetComponent<Level5Script>();

            if (level5 != null) //means level5 is active
            {
                level5.BasketScored();
            }
        }
    }

    private IEnumerator DisableTemporarily()
    {
        Collider col = GetComponent<Collider>();
        col.enabled = false;
        yield return new WaitForSeconds(3f); // lockout
        col.enabled = true;
    }
}