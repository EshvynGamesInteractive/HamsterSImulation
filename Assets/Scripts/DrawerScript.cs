using UnityEngine;
using DG.Tweening;

public class DrawerScript : Interactable
{
    [SerializeField] private float openPos = 90f;
    [SerializeField] private float closePos = 0f;
    [SerializeField] private Transform drawer;
    [SerializeField] float animationDuration = 0.2f;
    [SerializeField] protected bool isOpen;


    private void Start()
    {
        if(drawer == null)
            drawer = transform;
        
        // if(isOpen)
        //     OpenDrawer();
        // else
        //     CloseDrawer();
    }
    protected void OpenDrawer()
    {
        isOpen = true;
        drawer.DOLocalMoveX(openPos, animationDuration);
        SoundManager.instance.PlaySound(SoundManager.instance.drawerOpen);
    }

    public void CloseDrawer()
    {
        isOpen = false;
        drawer.DOLocalMoveX(closePos, animationDuration);
        SoundManager.instance.PlaySound(SoundManager.instance.drawerClose);
    }

    public override void Interact(PlayerScript player)
    {
        if (isOpen)
        {
            CloseDrawer();
        }
        else
        {
            OpenDrawer();
        }
    }
}
