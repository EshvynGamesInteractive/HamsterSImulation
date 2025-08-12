using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class KidAI : Interactable
{
    [SerializeField] private string stumbleAnimationTrigger = "Stumble"; // Animator trigger for stumble
    private Animator animator;
    private bool hasStumbled;
    [SerializeField] bool isDancing;
    [SerializeField] Transform sticker;
    [SerializeField] Sprite[] stickers;

    void Start()
    {
        if (sticker != null)
        {
            int r = Random.Range(0, stickers.Length);
            sticker.GetComponent<SpriteRenderer>().sprite = stickers[r];
            sticker.localScale = Vector2.zero;
        }
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("Animator missing on Kid.");
            enabled = false;
        }
        if(isDancing)
        {
            animator.SetTrigger("Dance");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasStumbled || !other.CompareTag("Toy")) return;
        hasStumbled = true;
        Debug.Log(animator);
        animator.SetTrigger(stumbleAnimationTrigger);
        MainScript.instance.activeLevel.TaskCompleted(1);
    }

    public override void Interact(PlayerScript player)
    {
        DisableForInteraction(true);
        player.GetComponent<FP_Controller>().ForceJump();
        sticker.DOScale(new Vector2(1.2f, 1.2f), 0.2f);
        sticker.DOScale(new Vector2(1, 1), 0.1f).SetDelay(0.2f);
        MainScript.instance.activeLevel.TaskCompleted(5);
    }
}