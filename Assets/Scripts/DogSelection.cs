using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DogSelection : MonoBehaviour
{
    [SerializeField] private GameObject btnSelect, btnBuy, btnWatchAd;
    [SerializeField] private GameObject btnNext, btnPrevious;
    [SerializeField] private Image dogIcon;
    [SerializeField] private Sprite[] dogSprites;
    [SerializeField] private int[] dogsPrices;
    [SerializeField] private Text txtDogPrice;

    private int dogIndex;

    private void Start()
    {
        // Always unlock the first dog by default
        PlayerPrefs.SetInt("DogNumber0", 1);
        UpdateUI();
    }

    public void OnBtnNext()
    {
        if (dogIndex < dogSprites.Length - 1)
        {
            dogIndex++;
            UpdateUI();
        }
    }

    public void OnBtnPrevious()
    {
        if (dogIndex > 0)
        {
            dogIndex--;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // Update sprite
        dogIcon.sprite = dogSprites[dogIndex];

        dogIcon.transform.DOScale(new Vector2(1.1f, 1.1f), 0.2f).SetUpdate(true);
        dogIcon.transform.DOScale(new Vector2(1f, 1f), 0.1f).SetDelay(0.2f).SetUpdate(true);
        
        
        // Update navigation buttons
        btnPrevious.SetActive(dogIndex > 0);
        btnNext.SetActive(dogIndex < dogSprites.Length - 1);

        // Update price text
        txtDogPrice.text = dogsPrices[dogIndex].ToString();

        // Update lock/unlock buttons
        if (IsDogLocked())
        {
            btnBuy.SetActive(true);
            btnWatchAd.SetActive(true);
            btnSelect.SetActive(false);
        }
        else
        {
            btnBuy.SetActive(false);
            btnWatchAd.SetActive(false);
            btnSelect.SetActive(true);
        }
    }

    private bool IsDogLocked()
    {
        return PlayerPrefs.GetInt("DogNumber" + dogIndex, 0) == 0;
    }

    private void UnlockDog()
    {
        btnSelect.SetActive(true);
        PlayerPrefs.SetInt("DogNumber" + dogIndex, 1);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void UnlockDogViaBones()
    {
        if (GlobalValues.TotalBones >= dogsPrices[dogIndex])
        {
            SoundManager.instance.PlaySound(SoundManager.instance.purchase);
            GlobalValues.TotalBones -= dogsPrices[dogIndex];
            MainScript.instance.UpdateBonesText();
            UnlockDog();
        }
        else
        {
            MainScript.instance.pnlInfo.ShowInfo("Not Enough Bones To Unlock Dog");
        }
    }

    public void OnBtnWatchAdForDog()
    {
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowRewardedAd(() => UnlockDog(), "RewardedDogUnlockAd");
        else
            UnlockDog();
    }

    public void OnBtnSelect()
    {
        GlobalValues.SelectedDogIndex = dogIndex;
        MainScript.instance.pnlInfo.ShowInfo("Dog Selected!");
        ClosePanel();
    }

    public void ClosePanel()
    {
        MainScript.instance.ClosePopup(gameObject);
    }
}

































// using System;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class DogSelection : MonoBehaviour
// {
//     [SerializeField] private GameObject btnSelect, btnBuy, btnWatchAd;
//     [SerializeField] private GameObject btnNext, btnPrevious;
//     [SerializeField] private Image dogIcon;
//     [SerializeField] private Sprite[] dogSprites;
//     [SerializeField] private int[] dogsPrices;
//     [SerializeField] private Text txtDogPrice;
//     private int dogIndex;
//
//
//     private void Start()
//     {
//         UnlockDog();
//     }
//
//     public void OnBtnNext()
//     {
//         if (dogIndex >= dogSprites.Length - 1)
//             return;
//         dogIndex++;
//         dogIcon.sprite = dogSprites[dogIndex];
//         btnPrevious.SetActive(true);
//         if (dogIndex >= dogSprites.Length - 1)
//         {
//             btnNext.SetActive(false);
//         }
//         txtDogPrice.text = dogsPrices[dogIndex].ToString();
//         if (IsDogLocked())
//         {
//             btnBuy.SetActive(true);
//             btnWatchAd.SetActive(true);
//             btnSelect.SetActive(false);
//         }
//         else
//         {
//             btnBuy.SetActive(false);
//             btnWatchAd.SetActive(false);
//             btnSelect.SetActive(true);
//         }
//     }
//
//     public void OnBtnPrevious()
//     {
//         if (dogIndex <= 0)
//             return;
//         dogIndex--;
//         dogIcon.sprite = dogSprites[dogIndex];
//         btnNext.SetActive(true);
//         txtDogPrice.text = dogsPrices[dogIndex].ToString();
//         if (dogIndex <= 0)
//         {
//             btnPrevious.SetActive(false);
//         }
//
//         if (IsDogLocked())
//         {
//             btnBuy.SetActive(true);
//             btnWatchAd.SetActive(true);
//             btnSelect.SetActive(false);
//         }
//         else
//         {
//             btnBuy.SetActive(false);
//             btnWatchAd.SetActive(false);
//             btnSelect.SetActive(true);
//         }
//     }
//
//
//     private bool IsDogLocked()
//     {
//         int x = PlayerPrefs.GetInt("DogNumber" + dogIndex.ToString(), 0);
//
//         if (x == 0)
//             return true;
//         else
//             return false;
//     }
//
//     private void UnlockDog()
//     {
//         PlayerPrefs.SetInt("DogNumber" + dogIndex.ToString(), 1);
//     }
//
//     public void UnlockDogViaBones()
//     {
//         if (GlobalValues.TotalBones >= dogsPrices[dogIndex])
//         {
//             GlobalValues.TotalBones -= dogsPrices[dogIndex];
//             UnlockDog();
//         }
//         else
//         {
//             MainScript.instance.pnlInfo.ShowInfo("Not Enough Bones To Unlock Dog");
//         }
//     }
//
//     public void OnBtnWatchAdForDog()
//     {
//         if (Nicko_ADSManager._Instance)
//             Nicko_ADSManager._Instance.ShowRewardedAd(() => UnlockDog(), "RewardedDogUnlockAd");
//         else
//             UnlockDog();
//     }
//
//     public void OnBtnSelect()
//     {
//         GlobalValues.SelectedDogIndex= dogIndex;
//     }
//
//     public void ClosePanel()
//     {
//         MainScript.instance.ClosePopup(gameObject);
//     }
//     
// }