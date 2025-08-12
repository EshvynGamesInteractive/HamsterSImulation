using UnityEngine;

public class CookieJar : Pickable
{
    public override void Interact(PlayerScript player)
    {
        MiniGameManager.Instance.cushionTrampoline.CookieJarPicked();
        MainScript.instance.HideIndication();
        PickItem(player);
    }
}
