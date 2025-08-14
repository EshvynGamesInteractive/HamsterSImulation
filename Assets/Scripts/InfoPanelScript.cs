using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelScript : MonoBehaviour
{
    [SerializeField] RectTransform infoPanel;
    [SerializeField] Text txtInfo;
    [SerializeField] private float showDuration = 3;

    public void ShowInfo(string info)
    {
        if (MainScript.instance.gameover)
            return;
        if (txtInfo != null)
            txtInfo.text = info;
        infoPanel.DOAnchorPosY(250, 0.5f);
        infoPanel.DOAnchorPosY(220, 0.3f).SetDelay(0.6f);
        infoPanel.DOAnchorPosY(-2000, 0.3f).SetDelay(3);
    }
}
