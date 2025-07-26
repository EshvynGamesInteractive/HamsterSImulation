using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{

    public CanvasState currentState;
    public CanvasState prevState;
    public GameObject[] CanvasState;
    public SmoothLoading loading;

    // Start is called before the first frame update
    void ChangeCanvas(CanvasState newState)
    {
        prevState = currentState;
        currentState = newState;
        CanvasState[(int)prevState].SetActive(false);
        CanvasState[(int)currentState].SetActive(true);
    }

    public void LoadScene(int i)
    {
        loading.LoadSceneSmooth(i);
    }

    // Update is called once per frame
}

public enum CanvasState{
    Pause,
    Win,
    Fail,
    Loading
}