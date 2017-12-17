using UnityEngine;
using System.Collections.Generic;

public enum GameState { Normal, Paused, ForcedPause }
public class GameManager : MonoBehaviour {

    private static GameManager instance = null;
    private static EvolutionManager evolutionManager = new EvolutionManager();
    private static GameModeManager gameModeManager = new GameModeManager();
    private static GameMode currentGameMode;
    private static GameState currentState = GameState.Normal;
    [SerializeField]
    private PlayerStart playerStart;
    private PlayerUI playerUI;

    // WARNING, should be reset on load scene
    public bool isTimeOver = false;

    float gameFinalTimer = 2.0f;
    // WARNING, should be reset on load scene
    float currentGameFinalTimer;
    // WARNING, should be reset on load scene
    bool finalTimerInitialized = false;

    public GameObject activeTutoTextForAll;

    public bool[] unlockedMinigames = new bool[(int)MiniGame.Size];

    // TODO: move this
    public int[][] playerCollectables;
    public bool[][] playerEvolutionTutoShown;
    public bool[] playerCostAreaTutoShown;

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

    public static GameModeManager GameModeManager
    {
        get
        {
            return gameModeManager;
        }
    }

    public static GameState CurrentState
    {
        get
        {
            return currentState;
        }
    }

    public static GameMode CurrentGameMode
    {
        get
        {
            // Load from another scene than the menu load te escape gameMode fow now
            // TODO : Destroy this ? 
            if (currentGameMode == null)
            {
                currentGameMode = gameModeManager.GetGameModeByName(GameModeType.Escape);
            }
            return currentGameMode;
        }
        set
        {
            currentGameMode = value;
        }
    }

    public PlayerStart PlayerStart
    {
        get
        {
            return playerStart;
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

    public PlayerUI PlayerUI
    {
        get
        {
            return playerUI;
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

    public void RegisterPlayerStart(PlayerStart _ps)
    {
        playerStart = _ps;
    }

    public void RegisterPlayerUI(PlayerUI _pUI)
    {
        playerUI = _pUI;
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
            if (currentState == GameState.Paused)
            {
                currentState = GameState.Normal;
                pauseMenuReference.gameObject.SetActive(false);
                for (int i = 0; i < instance.playerStart.ActivePlayersAtStart; i++)
                    instance.playerStart.cameraPlayerReferences[i].transform.GetChild(0).GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
            }
            else if (currentState == GameState.Normal)
            {
                currentState = GameState.Paused;
                pauseMenuReference.gameObject.SetActive(true);
           
                for (int i = 0; i < instance.playerStart.ActivePlayersAtStart; i++)
                    instance.playerStart.cameraPlayerReferences[i].transform.GetChild(0).GetComponent<Cinemachine.CinemachineBrain>().enabled = false;
            }
        }

        else if (_newState == GameState.Normal)
        {
            if (currentState == GameState.Paused)
            {
                currentState = GameState.Normal;
                pauseMenuReference.gameObject.SetActive(false);
                for (int i = 0; i < instance.playerStart.ActivePlayersAtStart; i++)
                    instance.playerStart.cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
            }
        }
    }

    private void Update()
    {
        if (finalTimerInitialized)
        {
            currentGameFinalTimer -= Time.deltaTime;
            if (currentGameFinalTimer < 0.0f)
            {
                finalTimerInitialized = false;
                isTimeOver = true;              
                scoreScreenReference.RankPlayersByPoints();
            }
            else
            {
                // TODO: handle this not every frame but each second
                uiReference.TimerNeedUpdate(currentGameFinalTimer);
            }
        }
    }

    public void LaunchFinalTimer()
    {
        currentGameFinalTimer = GameFinalTimer;
        finalTimerInitialized = true;
        uiReference.TimerNeedUpdate(currentGameFinalTimer);
    }

    [SerializeField] float maxMovementSpeed = 35.0f;

    [SerializeField]
    UI uiReference;
    [SerializeField]
    public static PauseMenu pauseMenuReference;
    [SerializeField]
    private ScoreScreen scoreScreenReference;
}
