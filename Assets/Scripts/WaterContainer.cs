using DG.Tweening;
using UnityEngine;

public class WaterContainer : Interactable
{
    [SerializeField] Transform spilledWater;
    [SerializeField] Transform fallPos;

    private void Start()
    {
        spilledWater.localScale = Vector3.zero;
        spilledWater.gameObject.SetActive(false);
    }
    public override void Interact(PlayerScript player)
    {
        player.AnimatePawToInteract();
        DisableForInteraction(true);
        Invoke(nameof(SpillWater), 0.5f);
    }

    private void SpillWater()
    {
        MainScript.instance.HideIndication();
        SoundManager.instance.PlaySound(SoundManager.instance.dropBucket);
        spilledWater.gameObject.SetActive(true);
        transform.DORotateQuaternion(fallPos.rotation, 0.3f);
        transform.DOMove(fallPos.position, 0.3f);
        SoundManager.instance.PlaySound(SoundManager.instance.spillWater);
        spilledWater.DOScale(Vector3.one, 0.5f).SetDelay(0.3f);
    }
}
