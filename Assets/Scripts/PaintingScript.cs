using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class PaintingScript : Interactable
{
    [SerializeField] private float endVal;
    private Level5Script level5;

    private void Start()
    {
        // 50% chance left side, 50% chance right side
        if (Random.value < 0.5f)
            endVal = Random.Range(-50, -25); // left tilt
        else
            endVal = Random.Range(25, 50); // right tilt


        level5 = MainScript.instance.activeLevel.GetComponent<Level5Script>();
    }

    public override void Interact(PlayerScript player)
    {
        MainScript.instance.HideIndication();
        Vector3 temp = transform.localEulerAngles;
        temp.z = endVal;
        transform.DOLocalRotate(temp, 0.2f);
        DisableForInteraction(false);
        if (level5 != null)
            level5.PaintingTilted();
    }
}