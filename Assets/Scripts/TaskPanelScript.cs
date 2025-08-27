using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TaskPanelScript : MonoBehaviour
{
    [SerializeField] RectTransform taskBar;
    [SerializeField] Text txtTask;
    [SerializeField] private Image taskIcon;


    public void UpdateTask(string newTask, Sprite icon)
    {
        if (txtTask.text == newTask) return;
        //Debug.Log("tassssssk");
        SoundManager.instance.PlaySound(SoundManager.instance.taskComplete);

        //txtTask.text = newTask;
        float moveDuration = 1;

        Sequence barMoveSeq = DOTween.Sequence();
        barMoveSeq.Append(taskBar.DOAnchorPosY(-150, moveDuration));
        barMoveSeq.Append(txtTask.DOText(newTask, 0));
        barMoveSeq.Append(taskBar.DOAnchorPosY(150, 0));
        barMoveSeq.Append(taskBar.DOAnchorPosY(0, moveDuration));


        taskIcon.DOFade(0, moveDuration);
        DOVirtual.DelayedCall(moveDuration, () => { taskIcon.sprite = icon; });
        taskIcon.DOFade(1, moveDuration).SetDelay(moveDuration);
    }


    public string GetCurrentTask()
    {
        return txtTask.text;
    }
    public Sprite GetCurrentTaskSprite()
    {
        return taskIcon.sprite;
    }
}