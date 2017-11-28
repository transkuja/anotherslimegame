using UWPAndXInput;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class ScoreScreen : MonoBehaviour {
    enum ScoreScreenChildren { ScorePanel }

    GameObject scorePanel;

    public GameObject prefabPlayerScore;

    private int valueCoins = 5;
    private int valueTime = 100;

    public Dictionary<Player, GameObject> scorePanelPlayer = new Dictionary<Player, GameObject>();
    public int rank = 0;

    uint nbrOfPlayersAtTheEnd = 0;

    private void Awake()
    {
        GameManager.Instance.RegisterScoreScreenPanel(this);
        scorePanel = transform.GetChild((int)ScoreScreenChildren.ScorePanel).gameObject;
        gameObject.SetActive(false);

    }

    public void Init()
    {

        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameObject playerScore = Instantiate(prefabPlayerScore, scorePanel.transform);
            scorePanelPlayer.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerScore);
        }
    }

    public void RefreshScores(Player player)
    {
        float time = Time.timeSinceLevelLoad;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = (int)time % 60;
     
        String timeStr = string.Format("{0:00} : {1:00}", minutes, seconds);
        scorePanelPlayer[player].GetComponent<PlayerScore>().Rank.text = rank.ToString();
        scorePanelPlayer[player].GetComponent<PlayerScore>().TextTime.text = timeStr;
        scorePanelPlayer[player].GetComponent<PlayerScore>().TextPointTime.text = (((SpawnManager.Instance.SpawnedItemsCount* valueCoins )/ Mathf.RoundToInt(time)) * valueTime).ToString();
        scorePanelPlayer[player].GetComponent<PlayerScore>().TextCoins.text = player.Collectables[(int)CollectableType.Points].ToString();
        scorePanelPlayer[player].GetComponent<PlayerScore>().TextPointCoins.text = (player.Collectables[(int)CollectableType.Points] * valueCoins).ToString();
        scorePanelPlayer[player].SetActive(true);

        player.Anim.SetBool("hasFinished", true);
        nbrOfPlayersAtTheEnd++;
        CheckEndGame();
    }

    void CheckEndGame()
    {
        if (nbrOfPlayersAtTheEnd == GameManager.Instance.PlayerStart.ActivePlayersAtStart)
        {
            gameObject.SetActive(true);
        }
    }

    void Update()
    {

        // TODO : Multi to be handle
        if (!GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().PlayerIndexSet)
            return;

        if (GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().IsUsingAController)
        {
            if (GamePad.GetState(GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().playerIndex).Buttons.Start == ButtonState.Pressed)
                ExitToMainMenu();

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ExitToMainMenu();
        }
        // TODO: handle pause input here?
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
