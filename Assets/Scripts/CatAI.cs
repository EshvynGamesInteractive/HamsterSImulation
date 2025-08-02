using UnityEngine;

public class CatAI : MonoBehaviour
{
    [SerializeField] private float napInterval = 10f; // Time between nap attempts
    [SerializeField] private string napAnimation = "Nap"; // Nap animation trigger
    [SerializeField] private string hitAnimation = "Hit"; // Hit animation trigger

    [SerializeField]private Animator animator;
    private float napTimer;

    void Start()
    {
        animator.SetTrigger(napAnimation);
        //napTimer = Random.Range(2f, napInterval); // Random initial nap delay
    }

    //void Update()
    //{
    //    napTimer -= Time.deltaTime;
    //    if (napTimer <= 0)
    //    {
           
    //        napTimer = napInterval; // Reset timer
    //    }
    //}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sock"))
        {
            animator.SetTrigger(hitAnimation);
            OnSockHit();
        }
    }

    private void OnSockHit()
    {
        MainScript.instance.activeLevel.TaskCompleted(3);
        // Empty method for your cutscene/control logic
    }
}