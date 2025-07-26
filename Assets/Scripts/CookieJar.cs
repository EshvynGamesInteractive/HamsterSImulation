using UnityEngine;

public class CookieJar : Interactable
{
    public override void Interact(PlayerScript player)
    {
        gameObject.SetActive(false);
    }
}
