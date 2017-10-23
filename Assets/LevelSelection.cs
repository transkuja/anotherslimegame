using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameModeProperties
{
    [SerializeField] public Gamemode gameMode;
    [SerializeField] public int indexLevel;
}

public class LevelSelection : MonoBehaviour {

    private static LevelSelection instance = null;

    public float countdown = 0.0f;
    public bool isCountdownStarted = false;
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
        if(ListOfPotentialGame.Count == 0)
        {
            instance.isCountdownStarted = true;
            instance.countdown = 0.0f;
            return;
        }

        if(instance.countdown == 0.0f)
        {
            instance.countdown = 7.0f;
            instance.isCountdownStarted = true;
        }

    }

    public static LevelSelection Instance
    {
        get
        {
            return instance;
        }
    }

    // Use this for initialization
    void Start () {
        // Je m'assure que ma liste est bien vide
        listOfPotentialGame.Clear();
        isCountdownStarted = false;
        countdown = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
		if (isCountdownStarted)
        {
            countdown -= Time.deltaTime;
            menuRef.GetComponent<Menu>().RefreshCountDown(countdown);
            if ( countdown < 0.0f)
            {
                countdown = 0.0f;
                int random = UnityEngine.Random.Range(0, listOfPotentialGame.Count-1);
                LoadLevel(listOfPotentialGame[random].gameMode, listOfPotentialGame[random].indexLevel);
            }
        }
	}

    public void LoadLevel(Gamemode gameMode, int indexLevel)
    {
        GameManager.CurrentGameMode = GameManager.GameModeManager.GetGameModeByName(gameMode);
        SceneManager.LoadScene(indexLevel);
    }
}
