using UnityEngine;
using System;
using UnityEngine.UI;

public enum MiniGameType
{
    None,
    FetchFrenzy,
    FridgeHeist,
    BubblePopBlitz,
    CushionTrampoline,
    WallArtWhirl
}

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }
    public Button gameStartBtn;

    public FridgeHeistController fridgeLevel;
    public BubblePopManager bubbleLevel;
    public CushionTrampolineManager cushionTrampoline;
  

    public MiniGameType ActiveMiniGame { get; private set; } = MiniGameType.None;

    public event Action<MiniGameType> OnMiniGameStart;
    public event Action<MiniGameType> OnMiniGameEnd;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // Optional: Subscribe to global events here
        //Debug.Log("MiniGameManager enabled");

        OnMiniGameStart += MiniGameStarter;
        OnMiniGameEnd += MiniGameEnded;
    }

    private void OnDisable()
    {
        OnMiniGameStart -= MiniGameStarter;
        OnMiniGameEnd -= MiniGameEnded;
        //Debug.Log("MiniGameManager disabled");
    }

    public void StartMiniGame(MiniGameType type)
    {
        if (ActiveMiniGame != MiniGameType.None)
        {
            Debug.LogWarning("A mini-game is already active!");
            return;
        }

        ActiveMiniGame = type;
        Debug.Log($"Starting mini-game: {type}");
        OnMiniGameStart?.Invoke(type);
    }

    public void EndMiniGame()
    {
        if (ActiveMiniGame == MiniGameType.None)
        {
            Debug.LogWarning("No mini-game is currently active!");
            return;
        }

        Debug.Log($"Ending mini-game: {ActiveMiniGame}");
        OnMiniGameEnd?.Invoke(ActiveMiniGame);
        ActiveMiniGame = MiniGameType.None;
    }

    public void MiniGameStarter(MiniGameType miniGameType)
    {
        //Debug.Log(miniGameType);
    }
    
    public void MiniGameEnded(MiniGameType miniGameType)
    {
        //Debug.Log(miniGameType);
    }
    
    public bool IsPlaying(MiniGameType type)
    {
        return ActiveMiniGame == type;
    }
}