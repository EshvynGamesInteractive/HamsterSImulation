using System;
using UnityEngine;

public class BasketBallHoop : MonoBehaviour
{
   [SerializeField] private Transform basketBallPos;
   
   // private void OnCollisionEnter(Collision other)
   // {
   //    if (other.gameObject.CompareTag("BasketBall"))
   //    {
   //       Transform basketBall = other.transform;
   //       
   //       basketBall.position = basketBallPos.position;
   //    }
   // }
}
