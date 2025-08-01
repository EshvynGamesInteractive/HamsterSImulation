using DG.Tweening;
using UnityEngine;

public enum BubbleType
{
    StinkBomb,
    RegularBubble,
    PrizeBubble,
    GlodenBubble
}

public class Bubble : MonoBehaviour
{
    public BubbleType bubbleType;
    public GameObject popEffect;
    public int scoreValue = 1;

    [SerializeField] GameObject duck, bone, shampoo;

    [SerializeField] private float minFloatSpeed = 0.5f;
    [SerializeField] private float maxFloatSpeed = 1.5f;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] float maxHeight=5;
    [SerializeField] string height;

    private float floatSpeed;



    private float waveFrequency;
    private float waveAmplitude;
    private Vector3 startScale;




    public void InitBubble(BubbleType bubbleTypee, Material bubbleMat)
    {
        bubbleType = bubbleTypee;
        meshRenderer.material = bubbleMat;



        switch (bubbleType)
        {
            case BubbleType.StinkBomb:
                break;

            case BubbleType.RegularBubble:
                duck.SetActive(true);
                break;

            case BubbleType.PrizeBubble:
                shampoo.SetActive(true);
                break;

            case BubbleType.GlodenBubble:
                bone.SetActive(true);
                break;
        }
        }

    private void Start()
    {
        floatSpeed = Random.Range(minFloatSpeed, maxFloatSpeed);
        waveFrequency = Random.Range(1f, 3f);
        waveAmplitude = Random.Range(0.05f, 0.2f);
        startScale = transform.localScale;
        transform.localScale = startScale*0.5f;

        float randomGrow = Random.Range(0.5f, 1);

        transform.DOScale(startScale * randomGrow, 3f);
    }

    private void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        Vector3 waveMove = new Vector3(waveOffset, floatSpeed, 0f) * Time.deltaTime;
        transform.position += waveMove;

        //// Smooth scale up
        //float scaleProgress = Mathf.InverseLerp(0, maxHeight, transform.position.y); // assuming 3f is max height range
        //transform.localScale = Vector3.Lerp(startScale, startScale * 5, scaleProgress);

        height = transform.position.y.ToString();

        if (transform.position.y >= maxHeight)
            Destroy(gameObject);
    }




    //private void Update()
    //{
    //    transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    //}

    private void OnMouseDown() // Mobile or mouse tap
    {
        HandlePop();
    }

    public void PopFromPlayer() // Called from player action on console
    {
        HandlePop();
    }

    private void HandlePop()
    {
        Instantiate(popEffect, transform.position, Quaternion.identity);

        switch (bubbleType)
        {
            case BubbleType.StinkBomb:
                Debug.Log("Stink bomb! Penalize.");
                MiniGameManager.Instance.bubbleLevel?.ApplyPenalty(2);
                break;

            case BubbleType.RegularBubble:
                Debug.Log("Rubber Duck! +1");
                MiniGameManager.Instance.bubbleLevel?.AddScore(1);
                break;

            case BubbleType.PrizeBubble:
                Debug.Log("Shampoo! +2");
                MiniGameManager.Instance.bubbleLevel?.AddScore(3);
                break;

            case BubbleType.GlodenBubble:
                Debug.Log("Golden Bone! +5");
                MiniGameManager.Instance.bubbleLevel?.AddScore(5);
                break;
        }

        Destroy(gameObject);
    }
}
