using DG.Tweening;
using UnityEngine;

public class TeaCupScript : Interactable
{
    [SerializeField] int taskNumber=5;
    [SerializeField] Transform ketchupBoxPos;
    [SerializeField] ParticleSystem pourParticle;
    [SerializeField] GameObject pourDkn;
    public override void Interact(PlayerScript player)
    {
        if (player.pickedObject != null && player.pickedObject.TryGetComponent<KetchupBoxScript>(out KetchupBoxScript ketchupBox))
        {
            if(pourDkn != null)
                pourDkn.SetActive(false);
            pourParticle.Play();
            float duration = 0.3f;
            Transform boxParent = ketchupBox.transform.parent;

            ketchupBox.transform.SetParent(ketchupBoxPos);
            DisableForInteraction(true);
            ketchupBox.transform.SetParent(ketchupBoxPos);
            SoundManager.instance.PlaySound(SoundManager.instance.pour);
            Sequence ketchupSequence = DOTween.Sequence();
            ketchupSequence.Append(ketchupBox.transform.DOLocalMove(Vector3.zero, duration));
            ketchupSequence.Join(ketchupBox.transform.DOLocalRotateQuaternion(Quaternion.identity, duration));
            player.ChangeObjectLayer(ketchupBox.transform, "Default");
            
            DOVirtual.DelayedCall(1, () =>
            {
                ketchupBox.transform.SetParent(boxParent);
                ketchupBox.transform.DOLocalMove(Vector3.zero, duration);
                ketchupBox.transform.DOLocalRotateQuaternion(Quaternion.identity, duration);
                player.ChangeObjectLayer(ketchupBox.transform, "PickedLayer");
                    MainScript.instance.activeLevel.TaskCompleted(taskNumber);
                //DOVirtual.DelayedCall(2, () =>
                //{
                //});
            });

        }
        else
        {
            MainScript.instance.pnlInfo.ShowInfo("Pick ketchup box to pour");
        }
    }
}
