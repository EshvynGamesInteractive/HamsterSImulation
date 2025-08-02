using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    public LevelButton[] levels;
    private int selectedLevel = -1;


    private void Start()
    {
        LockLevels();
    }
    public void LockLevels()
    {
        Debug.Log(GlobalValues.UnlockedLevels); 
        for (int i = GlobalValues.UnlockedLevels; i < levels.Length; i++)
        {
            levels[i].LockLevel();
        }
    }

    public void OnSelectLevel(int levelNumber)
    {
        selectedLevel = levelNumber;
        OnBtnPlaylevel();
    }
   

    public void OnBtnPlaylevel()
    {
        GlobalValues.currentLevel = selectedLevel;
        GlobalValues.sceneTOLoad = "Gameplay";
        SceneManager.LoadScene("Loading");
    }

    public void OnBtnBack()
    {
        gameObject.SetActive(false);
    }
}








