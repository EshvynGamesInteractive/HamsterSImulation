using PaintIn3D;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseColorScript : MonoBehaviour, IPointerClickHandler
{
    public CwPaintSphere paint;
    public Color color;

    public void OnPointerClick(PointerEventData eventData)
    {
        paint.Color = color;
    }
}
