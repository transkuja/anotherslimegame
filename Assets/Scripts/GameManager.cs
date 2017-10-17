
using UnityEngine;

public enum GameState { Normal, Paused, ForcedPause }
public class GameManager : MonoBehaviour {

    private static GameManager instance = null;
    private static EvolutionManager evolutionManager = new EvolutionManager();
    private static GameState currentState = GameState.Normal;
   
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

    public static float JumpUnit
    {
        get
        {
            return Instance.jumpUnit;
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

    public static int GameplayType
    {
        get
        {
            return Instance.gameplayType;
        }

        set
        {
            Instance.gameplayType = value;
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

    [Header("Jump Settings")]
    [Tooltip("Jump unit is used to calibrate full charge jump")]
    [SerializeField] float jumpUnit = 500.0f;
    [SerializeField] float maxMovementSpeed = 40.0f;


    [SerializeField]
    [Range(1, 2)]
    private int gameplayType = 1;

    UI uiReference;
    public static PauseMenu pauseMenuReference;
}
