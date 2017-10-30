using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameModeProperties
{
    [SerializeField] public GameModeType gameMode;
    [SerializeField] public int indexLevel;
}

public class LevelSelection : MonoBehaviour {

    private static LevelSelection instance = null;

    public float countdown = 0.0f;
    private bool isCountdownStarted = false;
    public Menu menuRef;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private List<GameModeProperties> listOfPotentialGame = new List<GameModeProperties>();

    public List<GameModeProperties> ListOfPotentialGame
    {
        get
        {
            return listOfPotentialGame;
        }
    }

    public void ProcessCountdown()
    {
        if (GameManager.Instance.ActivePlayersAtStart == 1)
        {
            LoadLevel(listOfPotentialGame[0].gameMode, listOfPotentialGame[0].indexLevel);
            return;
        }
        if (ListOfPotentialGame.Count == 0)
        {
            instance.IsCountdownStarted = false;
            instance.countdown = 0.0f;
            return;
        }

        if(instance.countdown == 0.0f)
        {
            instance.countdown = 7.0f;
            instance.IsCountdownStarted = true;
        }

    }

    public static LevelSelection Instance
    {
        get
        {
            return instance;
        }
    }

    public bool IsCountdownStarted
    {
        get
        {
            return isCountdownStarted;
        }

        set
        {
            menuRef.GetComponent<Menu>().ToogleCountdownText(value);
            isCountdownStarted = value;
        }
    }

    // Use this for initialization
    void Start () {
        // Je m'assure que ma liste est bien vide
        listOfPotentialGame.Clear();
        IsCountdownStarted = false;
        countdown = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
		if (IsCountdownStarted)
        {
        
            menuRef.GetComponent<Menu>().RefreshCountDown(countdown);
            countdown -= Time.deltaTime;
            if ( countdown < 0.0f)
            {
                countdown = 0.0f;
                int random = UnityEngine.Random.Range(0, listOfPotentialGame.Count-1);
                LoadLevel(listOfPotentialGame[random].gameMode, listOfPotentialGame[random].indexLevel);
            }
        }
	}

    public void LoadLevel(GameModeType gameMode, int indexLevel)
    {
        GameManager.CurrentGameMode = GameManager.GameModeManager.GetGameModeByName(gameMode);
        SceneManager.LoadScene(indexLevel);
    }
}
