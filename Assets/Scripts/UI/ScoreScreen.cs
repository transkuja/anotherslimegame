using XInputDotNetPure;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ScoreScreen : MonoBehaviour {
    enum ScoreScreenChildren { ScorePanel }

    GameObject scorePanel;

    public GameObject prefabPlayerScore;

    private int valueCoins = 20;
    private int valueTime = 15;

    public Dictionary<Player, GameObject> scorePanelPlayer = new Dictionary<Player, GameObject>();
    public int rank = 0;

    private void Awake()
    {
        GameManager.Instance.RegisterScoreScreenPanel(this);
    }

    public void Init()
    {
        scorePanel = transform.GetChild((int)ScoreScreenChildren.ScorePanel).gameObject;
        gameObject.SetActive(false);

        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameObject playerScore = Instantiate(prefabPlayerScore, scorePanel.transform);
            scorePanelPlayer.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerScore);
        }
    }

    public void RefreshScores(Player player)
    {
        scorePanelPlayer[player].GetComponent<PlayerScore>().Rank.text = rank.ToString();
        scorePanelPlayer[player].GetComponent<PlayerScore>().TextTime.text = player.time.ToString();
        scorePanelPlayer[player].GetComponent<PlayerScore>().TextPointTime.text = (Mathf.RoundToInt(player.time) * valueTime).ToString();
        scorePanelPlayer[player].GetComponent<PlayerScore>().TextCoins.text = player.Collectables[(int)CollectableType.Points].ToString();
        scorePanelPlayer[player].GetComponent<PlayerScore>().TextPointCoins.text = (player.Collectables[(int)CollectableType.Points] * valueCoins).ToString();
        scorePanelPlayer[player].SetActive(true);
        
    }

    void Update()
    {
        // TODO : Multi to be handle
        if (!GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().PlayerIndexSet)
            return;

        if (GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().IsUsingAController)
        {
            
            if (GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().PrevState.Buttons.Start == ButtonState.Released && GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().State.Buttons.Start == ButtonState.Pressed)
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
