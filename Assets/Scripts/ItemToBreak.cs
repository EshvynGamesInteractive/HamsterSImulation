using System;
using DG.Tweening;
using UnityEngine;

public class ItemToBreak : Pickable
{
    [SerializeField] private GameObject[] parts;
    [SerializeField] AudioClip breakSound;
    private bool broken = false;
    private Level5Script level5;
    private void Start()
    {
        level5 = GetComponentInParent<Level5Script>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (broken) return; broken = true;
        Debug.Log(other.gameObject.name);
        if(other.gameObject.CompareTag("Player"))
            return;
        
        SoundManager.instance.PlaySound(breakSound);
        GetComponent<Collider>().enabled = false;
        foreach (GameObject part in parts)
        {
            Debug.Log("Breaking part: " + part.name);

            Rigidbody rb = part.AddComponent<Rigidbody>();
            part.GetComponent<MeshCollider>().enabled = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            // Outward direction from cup center
            Vector3 forceDir = (part.transform.position - transform.position).normalized;

            // Add a little push
            rb.AddForce(forceDir * 1.1f, ForceMode.Impulse);

            // OR: explosion style scatter
            // rb.AddExplosionForce(3f, transform.position, 1f, 0.2f, ForceMode.Impulse);
        }

        DOVirtual.DelayedCall(5, () =>
        {
gameObject.SetActive(false);
        });
        level5.ItemBroken();
    }
}