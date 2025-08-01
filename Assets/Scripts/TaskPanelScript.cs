using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TaskPanelScript : MonoBehaviour
{
    [SerializeField] RectTransform taskBar;
    [SerializeField] Text txtTask;

   
    public void UpdateTask(string newTask)
    {
        if (txtTask.text == newTask) return;

        //txtTask.text = newTask;
        float moveDuration = 1;

        Sequence barMoveSeq = DOTween.Sequence();
        barMoveSeq.Append(taskBar.DOAnchorPosY(-150, moveDuration));
        barMoveSeq.Append(txtTask.DOText(newTask, 0));
        barMoveSeq.Append(taskBar.DOAnchorPosY(150, 0));
        barMoveSeq.Append(taskBar.DOAnchorPosY(0, moveDuration));
    }


   




    public string GetCurrentTask()
    {
        return txtTask.text;
    }
}
