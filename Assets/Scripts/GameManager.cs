﻿using UnityEngine;
using System.Collections.Generic;

public enum GameState { Normal, Paused, ForcedPause }
public class GameManager : MonoBehaviour {

    private static GameManager instance = null;
    private static EvolutionManager evolutionManager = new EvolutionManager();
    //private static GameModeManager gameModeManager = new GameModeManager();
    private GameMode currentGameMode;
    private static GameState currentState = GameState.Normal;
    [SerializeField]
    private PlayerStart playerStart;
    private PlayerUI playerUI;
    private SlimeDataContainer dataContainer;

    // WARNING, should be reset on load scene
    public bool isTimeOver = false;

    float gameFinalTimer = 2.0f;
    // WARNING, should be reset on load scene
    float currentGameFinalTimer;
    // WARNING, should be reset on load scene
    bool finalTimerInitialized = false;

    public GameObject activeTutoTextForAll;

    public bool[] unlockedMinigames = new bool[(int)MiniGame.Size];

    private int runes = 0;
    private int globalMoney = 0;

    // Players persistence
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

    //public static GameModeManager GameModeManager
    //{
    //    get
    //    {
    //        return gameModeManager;
    //    }
    //}

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
            Debug.Assert(currentGameMode != null, "GameMode must be set on scene");
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

    public int Runes
    {
        get
        {
            return runes;
        }

        set
        {
            runes = Mathf.Clamp(value, 0, Utils.GetMaxValueForCollectable(CollectableType.Rune));
            // TODO: Runes UI update should be handled here
        }
    }

    public int GlobalMoney
    {
        get
        {
            return globalMoney;
        }

        set
        {
            // TODO: clamp it?
            //globalMoney = Mathf.Clamp(value, 0, Utils.GetMaxValueForCollectable(CollectableType.Rune));
            globalMoney = value;
            // TODO: update UI

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
