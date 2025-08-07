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
    public Button miniGameBtn, gameStartBtn;

    [SerializeField] MiniGameTriggerZone[] miniGameTriggerZones;
    public FridgeHeistController fridgeLevel;
    public BubblePopManager bubbleLevel;
    public CushionTrampolineManager cushionTrampoline;
    private string currentTask;

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

        MainScript.instance.HideIndication();

        currentTask = MainScript.instance.taskPanel.GetCurrentTask();

        ActiveMiniGame = type;
        Debug.Log($"Starting mini-game: {type}");
        OnMiniGameStart?.Invoke(type);
        for (int i = 0; i < miniGameTriggerZones.Length; i++)
        {
            miniGameTriggerZones[i].gameObject.SetActive(false);
        }

        MainScript.instance.activeLevel.transform.parent.gameObject.SetActive(false);
    }

    public void EndMiniGame()
    {
        MainScript.instance.activeLevel.transform.parent.gameObject.SetActive(true);
        if (ActiveMiniGame == MiniGameType.None)
        {
            Debug.LogWarning("No mini-game is currently active!");
            return;
        }
        for (int i = 0; i < miniGameTriggerZones.Length; i++)
        {
            miniGameTriggerZones[i].gameObject.SetActive(true);
        }
        SoundManager.instance.PlaySound(SoundManager.instance.timeOut);
        MainScript.instance.ShowIndication();
        MainScript.instance.taskPanel.UpdateTask(currentTask);
        Debug.Log($"Ending mini-game: {ActiveMiniGame}");
        OnMiniGameEnd?.Invoke(ActiveMiniGame);
        ActiveMiniGame = MiniGameType.None;
        MainScript.instance.activeLevel.MiniGameEnded();
        MainScript.instance.activeLevel.transform.parent.gameObject.SetActive(true);
        MainScript.instance.pnlInfo.ShowInfo("Mini game ended");
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