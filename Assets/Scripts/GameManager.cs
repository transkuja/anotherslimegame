using UnityEngine;

public enum GameState { Normal, Paused, ForcedPauseMGRules }

public class GameManager : MonoBehaviour {

    private static GameManager instance = null;
    private static EvolutionManager evolutionManager = new EvolutionManager();
    //private static GameModeManager gameModeManager = new GameModeManager();
    private GameMode currentGameMode;
    private static GameState currentState;
    [SerializeField]
    private PlayerStart playerStart;
    private APlayerUI specificPlayerUI;
    private SlimeDataContainer dataContainer;

    // WARNING, should be reset on load scene
    public bool isTimeOver = false;

    float gameFinalTimer = 2.0f;
    // WARNING, should be reset on load scene
    float currentGameFinalTimer;
    // WARNING, should be reset on load scene
    public bool finalTimerInitialized = false;

    public GameObject activeTutoTextForAll;

    // Players persistence
    public bool[][] playerEvolutionTutoShown;
    public bool[] playerCostAreaTutoShown;

    public Vector3 savedPositionInHub = Vector3.zero;
    public Quaternion savedRotationInHub = Quaternion.identity;

    public int[] consecutiveVictories = new int[4];

    public int playerWhoPausedTheGame = -1;

    public string previousScene = "";

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {

                instance = new GameObject("GameManager").AddComponent<GameManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }

        private set { }
    }

    public void DEBUG_IncreaseFinalCountdown(int _increaseValue)
    {
        currentGameFinalTimer += _increaseValue;
    }

    public static float MaxMovementSpeed
    {
        get
        {
            return Instance.maxMovementSpeed;
        }

        set
        {
            Instance.maxMovementSpeed = value;
        }
    }

    public static UI UiReference
    {
        get
        {
            return Instance.uiReference;
        }

        set
        {
            Instance.uiReference = value;
        }
    }

    public static EvolutionManager EvolutionManager
    {
        get
        {
            return evolutionManager;
        }
    }

    public static GameState CurrentState
    {
        get
        {
            return currentState;
        }
    }

    public GameMode CurrentGameMode
    {
        get
        {
            return currentGameMode;
        }
        set
        {
            currentGameMode = value;
        }
    }

    public bool IsInHub()
    {
        return currentGameMode != null && CurrentGameMode.GetType() == typeof(HubMode);
    }

    public PlayerStart PlayerStart
    {
        get
        {
            return playerStart;
        }
    }

    public SlimeDataContainer DataContainer
    {
        get
        {
            return dataContainer;
        }
    }

    public uint ActivePlayersAtStart
    {
        get
        {
            return playerStart.ActivePlayersAtStart;
        }
    }

    public ScoreScreen ScoreScreenReference
    {
        get
        {
            return scoreScreenReference;
        }
    }

    public float GameFinalTimer
    {
        get
        {
            return gameFinalTimer;
        }

        set
        {
            gameFinalTimer = value;
        }
    }

    public int Runes
    {
        get
        {
            return DatabaseManager.Db.NbRunes;
        }

        set
        {
            // Unlock minigame base
            foreach( DatabaseClass.MinigameData minigame in DatabaseManager.Db.minigames)
            {
                if (DatabaseManager.Db.NbRunes >= minigame.nbRunesToUnlock && minigame.nbRunesToUnlock != -1 && !DatabaseManager.Db.IsUnlock<DatabaseClass.MinigameData>(minigame.Id))
                {
                    // TODO: Notifier le joueur
                    DatabaseManager.Db.SetUnlock<DatabaseClass.MinigameData>(minigame.Id, true);
                }
            }
            UiReference.UpdateRunes();
            UiReference.HandleFeedback(CollectableType.Rune);
        }
    }

    public int GlobalMoney
    {
        get
        {
            return DatabaseManager.Db.Money;
        }

        set
        {
            DatabaseManager.Db.Money = value;
            UiReference.HandleFeedback(CollectableType.Money);
        }
    }

    public APlayerUI SpecificPlayerUI
    {
        get
        {
            return specificPlayerUI;
        }

        set
        {
            specificPlayerUI = value;
        }
    }

    public void RegisterPlayerStart(PlayerStart _ps)
    {
        playerStart = _ps;
    }

    public void RegisterDataContainer(SlimeDataContainer _sdc)
    {
        dataContainer = _sdc;
    }

    public void RegisterAPlayerUI(APlayerUI _minigamePUI)
    {
        SpecificPlayerUI = _minigamePUI;
    }

    public void RegisterScoreScreenPanel(ScoreScreen _ss)
    {
        scoreScreenReference = _ss;
    }

    /*
     * Try to change state into the desired new state, else change state into the logical one
     */
    public static void ChangeState(GameState _newState)
    {
        if (_newState == GameState.Paused)
        {
            if (currentState == GameState.Normal)
            {
                if (pauseMenuReference == null)
                    return;

                currentState = GameState.Paused;
                pauseMenuReference.gameObject.SetActive(true);
                UiReference.TooglePersistenceUI(true);
                for (int i = 0; i < instance.playerStart.ActivePlayersAtStart; i++)
                {
                    PlayerCharacterHub curPlayerCharacter = instance.playerStart.PlayersReference[i].GetComponent<PlayerCharacterHub>();
                    if (curPlayerCharacter)
                    {
                        if (curPlayerCharacter.PlayerState == curPlayerCharacter.underwaterState)
                            Utils.CopyUnderwaterStateDataToPausedState(curPlayerCharacter.underwaterState, curPlayerCharacter.pausedState);
                        curPlayerCharacter.PreviousPlayerState = curPlayerCharacter.PlayerState;
                        curPlayerCharacter.PlayerState = curPlayerCharacter.pausedState;
                    }
                }
                
            }
        }

        else if (_newState == GameState.Normal)
        {
            if (currentState == GameState.Paused)
            {
                if (pauseMenuReference == null)
                    return;

                currentState = GameState.Normal;
                pauseMenuReference.gameObject.SetActive(false);
                UiReference.TooglePersistenceUI(false);
                for (int i = 0; i < instance.playerStart.ActivePlayersAtStart; i++)
                {
                    PlayerCharacterHub curPlayerCharacter = instance.playerStart.PlayersReference[i].GetComponent<PlayerCharacterHub>();

                    // TODO: dash missing but we'll see after refacto
                    if (curPlayerCharacter != null)
                    {
                        if (curPlayerCharacter.PreviousPlayerState == curPlayerCharacter.underwaterState)
                        {
                            Utils.GetUnderwaterStateDataFromPausedState(curPlayerCharacter.underwaterState, curPlayerCharacter.pausedState);
                            curPlayerCharacter.PlayerState = curPlayerCharacter.underwaterState;
                        }
                        else
                            curPlayerCharacter.PlayerState = curPlayerCharacter.freeState;
                    }

                }
            }
            // Unlock pause after player has read rules + timer said GO!
            else if (currentState == GameState.ForcedPauseMGRules)
            {
                currentState = GameState.Normal;
                instance.CurrentGameMode.OnReadySetGoEnd();
            }

        }
        else if (_newState == GameState.ForcedPauseMGRules)
        {
            currentState = GameState.ForcedPauseMGRules;
        }

#if UNITY_EDITOR
        DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.GameState, currentState.ToString(), 0);
#endif
    }

    private void Update()
    {
        if (finalTimerInitialized && CurrentState == GameState.Normal)
        {
            currentGameFinalTimer -= Time.deltaTime;
            if (currentGameFinalTimer < 0.0f)
            {
                CleanEndFinalCountdown();
                if (scoreScreenReference && scoreScreenReference.GetComponent<ScoreScreen>()) // @remi think that
                    scoreScreenReference.RankPlayersByPoints();
            }
            else
            {
                // TODO: handle this not every frame but each second
                uiReference.TimerNeedUpdate(currentGameFinalTimer);
            }
        }
    }

    public void CleanEndFinalCountdown()
    {
        finalTimerInitialized = false;
        isTimeOver = true;
        if(UiReference.TimerText)
            UiReference.TimerText.gameObject.SetActive(false);
    }

    public void DEBUG_EndFinalCountdown()
    {
        currentGameFinalTimer = 0.0f;
        CleanEndFinalCountdown();
        uiReference.TimerNeedUpdate(currentGameFinalTimer);
    }

    public void LaunchFinalTimer()
    {
        currentGameFinalTimer = GameFinalTimer;
        finalTimerInitialized = true;
        isTimeOver = false;
    }

    [SerializeField] float maxMovementSpeed = 35.0f;

    [SerializeField]
    UI uiReference;
    [SerializeField]
    public static PauseMenu pauseMenuReference;
    [SerializeField]
    private ScoreScreen scoreScreenReference;
}
