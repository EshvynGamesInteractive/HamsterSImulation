using System;
using UnityEngine;

public class Dogs : MonoBehaviour
{
    [SerializeField] private GameObject[] dogs;
    private void OnEnable()
    {
        int dogIndex = GlobalValues.SelectedDogIndex;
        if(dogIndex>=dogs.Length)
            dogIndex=dogs.Length-1;

        for (int i = 0; i < dogs.Length; i++)
        {
            dogs[i].SetActive(false);
        }
        
        dogs[dogIndex].SetActive(true);
    }

  

    public void PlayDogEatingAnim()
    {
        int dogIndex = GlobalValues.SelectedDogIndex;
        if(dogIndex>=dogs.Length)
            dogIndex=dogs.Length-1;
        SoundManager.instance.PlaySound(SoundManager.instance.eat);
        dogs[dogIndex].GetComponent<Animator>().SetTrigger("Eating");
    }
}
