using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] LevelSelection levelSelection;
    [SerializeField] int levelNumber;
    public bool isLocked = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isLocked)
        {
        levelSelection.OnSelectLevel(levelNumber);
        }
    }

    public void LockLevel()
    {
        isLocked = true;
    }
}
