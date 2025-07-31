using UnityEngine;

public class DishesScript : Interactable
{
    [SerializeField] Rigidbody[] dishes;
    [SerializeField] float forceAmount = 2f;

    public override void Interact(PlayerScript player)
    {
        DisableForInteraction(true);
        foreach (Rigidbody dish in dishes)
        {
            dish.useGravity = true;
            dish.isKinematic = false;

            // Direction away from player
            Vector3 awayFromPlayer = (dish.position - player.transform.position).normalized;

            // Add slight upward and outward force
            Vector3 force = (awayFromPlayer + Vector3.up * 0.5f).normalized * forceAmount;

            dish.AddForce(force, ForceMode.Impulse);
        }
        MainScript.instance.activeLevel.TaskCompleted(2);
    }
}
