using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public enum MiniGameType
{
    None,
    FetchFrenzy,
    FridgeHeist,
    BubblePopBlitz,
    CushionTrampoline,
    WallArtWhirl,
    BubblePopBlitzGround,
    Piano,
    Drum
}

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }
    public Button miniGameBtn, gameStartBtn;

    [SerializeField] MiniGameTriggerZone[] miniGameTriggerZones;
    public FridgeHeistController fridgeLevel;
    public BubblePopManager bubbleLevel;
    public CushionTrampolineManager cushionTrampoline;
    public BallFetchGameController ballFetchGameController;
    public MusicGameController pianoGameController;
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

        // OnMiniGameStart += MiniGameStarter;
        // OnMiniGameEnd += MiniGameEnded;
    }

    private void OnDisable()
    {
        // OnMiniGameStart -= MiniGameStarter;
        // OnMiniGameEnd -= MiniGameEnded;
        //Debug.Log("MiniGameManager disabled");
    }

    public void StartMiniGame(MiniGameType type)
    {
        if (ActiveMiniGame != MiniGameType.None)
        {
            Debug.LogWarning("A mini-game is already active!");
            return;
        }

        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowInterstitial("MiniGameStart");
        MainScript.instance.grandPa.StopTheChase();
        MainScript.instance.canShowRewardedPopup = false;
        MainScript.instance.HideIndication();
        MainScript.instance.RestartRewardedTimer();
        MainScript.instance.activeLevel.MiniGameEnded();
        currentTask = MainScript.instance.taskPanel.GetCurrentTask();

        ActiveMiniGame = type;
        //Debug.Log($"Starting mini-game: {type}");
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

       
        MainScript.instance.canShowRewardedPopup = true;
        SoundManager.instance.PlaySound(SoundManager.instance.timeOut);
        MainScript.instance.ShowIndication();
        MainScript.instance.taskPanel.UpdateTask(currentTask);
        Debug.Log($"Ending mini-game: {ActiveMiniGame}");
        OnMiniGameEnd?.Invoke(ActiveMiniGame);
        ActiveMiniGame = MiniGameType.None;
        MainScript.instance.activeLevel.MiniGameEnded();
        MainScript.instance.activeLevel.transform.parent.gameObject.SetActive(true);
        MainScript.instance.pnlInfo.ShowInfo("Mini game ended");

        DOVirtual.DelayedCall(5, () =>
        {
            for (int i = 0; i < miniGameTriggerZones.Length; i++)
            {
                miniGameTriggerZones[i].gameObject.SetActive(true);
            }
        });
        if (Nicko_ADSManager._Instance)
            Nicko_ADSManager._Instance.ShowInterstitial("MiniGameEnd");
    }

    public void MiniGameStarter(MiniGameType miniGameType)
    {
        //Debug.Log(miniGameType);
    
    
        //Debug.Log("minigamestarted");
    }

    public void MiniGameEnded(MiniGameType miniGameType)
    {
        //Debug.Log("ended");
        //Debug.Log(miniGameType);
    }

    public bool IsPlaying(MiniGameType type)
    {
        return ActiveMiniGame == type;
    }
}