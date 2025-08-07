using UnityEngine;

public class UserInfoCanvasManager : MonoBehaviour
{
    public UserInfoStates currentState;
    public UserInfoStates previousState;
    public GameObject[] allCanvasStates;

    private void OnEnable()
    {
          CurrentStateChanger(UserInfoStates.AvatarSelection);
    }

    public void CurrentStateChanger(UserInfoStates newState)
    {
        previousState = currentState;
        currentState = newState;

        allCanvasStates[(int)previousState].SetActive(false);
        allCanvasStates[(int)currentState].SetActive(true);
    }
}

public enum UserInfoStates
{
    PlayerProfile,
    AvatarSelection,
    CountrySelection,
    SceneLoading
}