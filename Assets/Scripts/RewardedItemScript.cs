using System;
using UnityEngine;

public class RewardedItemScript : Pickable
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.TryGetComponent<GrandpaAI>(out _))
            gameObject.SetActive(false);
    }
}