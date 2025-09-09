using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardedItemTrigger : MonoBehaviour
{
    [SerializeField] private Pickable itemTOReward;
    [SerializeField]  private Button getRewardBtn;


    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //        getRewardBtn.onClick.Invoke();
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        getRewardBtn.onClick.AddListener(OnBtnGetReward);
        if (getRewardBtn != null)
            getRewardBtn.gameObject.SetActive(true);

      
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        getRewardBtn.onClick.RemoveAllListeners();

        if (getRewardBtn != null)
            getRewardBtn.gameObject.SetActive(false);

    }

    public void OnBtnGetReward()
    {
       
        if(Nicko_ADSManager.instance)
            Nicko_ADSManager.instance.ShowRewardedAd(() => GrantRewardedItem(), "RewardedItemAd");
        else
            GrantRewardedItem();
    }

    private void GrantRewardedItem()
    {
        PlayerScript player = MainScript.instance.player;

        if (player.pickedObject != null)
        {
            Debug.Log(player.pickedObject);
            player.ThrowObject();
        }
        player.PickObject(itemTOReward);
        
        
        gameObject.SetActive(false);
        getRewardBtn.onClick.RemoveAllListeners();
        if (getRewardBtn != null)
            getRewardBtn.gameObject.SetActive(false);
    }
}
