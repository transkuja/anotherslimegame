using UnityEngine;
using System.Collections.Generic;

public enum GameState { Normal, Paused, ForcedPause }
public class GameManager : MonoBehaviour {

    private static GameManager instance = null;
    private static EvolutionManager evolutionManager = new EvolutionManager();
    private static GameModeManager gameModeManager = new GameModeManager();
    private static GameMode currentGameMode;
    private static GameState currentState = GameState.Normal;
    private PlayerStart playerStart;
    List<GameObject> playersReference = new List<GameObject>();

    private void Awake()
    {
        currentGameMode = gameModeManager.GetGameModeByName(Gamemode.Escape);
    }

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
            return currentGameMode;
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

    public List<GameObject> PlayersReference
    {
        get
        {
            return playersReference;
        }
    }

    public void RegisterPlayerStart(PlayerStart _ps)
    {
        playerStart = _ps;
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
                Camera.main.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
            }
            else if (currentState == GameState.Normal)
            {
                currentState = GameState.Paused;
                pauseMenuReference.gameObject.SetActive(true);
                Camera.main.GetComponent<Cinemachine.CinemachineBrain>().enabled = false;
            }
        }

        else if (_newState == GameState.Normal)
        {
            if (currentState == GameState.Paused)
            {
                currentState = GameState.Normal;
                pauseMenuReference.gameObject.SetActive(false);
                Camera.main.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
            }
        }
    }

    [SerializeField] float maxMovementSpeed = 40.0f;

    UI uiReference;
    public static PauseMenu pauseMenuReference;
}
