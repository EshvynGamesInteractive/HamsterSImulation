using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class BalloonScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem popParticle;
    private Level5Script level5;

    private void Start()
    {
        if (GlobalValues.TutorialPlayed == 1)
            level5 = MainScript.instance.activeLevel.GetComponent<Level5Script>();
        float floatAmount = 0.2f; // how high it floats
        float floatDuration = 2f; // how long one up/down takes


        float animStartDelay = Random.Range(0, 3);

        transform.DOLocalMoveY(transform.localPosition.y + floatAmount, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo).SetDelay(animStartDelay);
    }

    public void PopBalloon()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.balloonPop);
        Sequence popSequence = DOTween.Sequence();
        popSequence.Append(transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f));
        popSequence.Append(transform.DOScale(Vector3.zero, 0.1f));
        popParticle.transform.position = transform.position;
        popParticle.Play();
        Destroy(gameObject, 0.2f);
        if (level5 != null)
            level5.BalloonBurst();
    }
}