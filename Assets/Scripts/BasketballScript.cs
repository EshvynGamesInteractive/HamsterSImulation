using System;
using UnityEngine;

public class BasketballScript : Pickable
{
    private Level5Script level5;
    private void Start()
    {
        if (GlobalValues.TutorialPlayed == 1)
            level5 = MainScript.instance.activeLevel.GetComponent<Level5Script>();
    }

    public override void Interact(PlayerScript player)
    {
        Debug.Log(level5);
        if (level5 != null)
            level5.BallPicked();
        base.PickItem(player);
    }
}