using XInputDotNetPure;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreScreen : MonoBehaviour {
    enum ScoreScreenChildren { ScorePanel }

    GameObject scorePanel;

    public GameObject prefabPlayerScore;

    private int valueCoins = 20;
    private int valueTime = 15;


    void Start()
    {
        GameManager.scoreScreenReference = this;
        scorePanel = transform.GetChild((int)ScoreScreenChildren.ScorePanel).gameObject;
        gameObject.SetActive(false);
    }

    public void RefreshScores()
    {
        // foreach player
        GameObject playerScore = Instantiate(prefabPlayerScore, scorePanel.transform);
        playerScore.GetComponent<PlayerScore>().Rank.text = "1";
        playerScore.GetComponent<PlayerScore>().TextTime.text = GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<Player>().time.ToString();
        playerScore.GetComponent<PlayerScore>().TextPointTime.text = (Mathf.RoundToInt(GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<Player>().time) * valueTime).ToString();
        playerScore.GetComponent<PlayerScore>().TextCoins.text = GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<Player>().Collectables[(int)CollectableType.Points].ToString();
        playerScore.GetComponent<PlayerScore>().TextPointCoins.text = (GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<Player>().Collectables[(int)CollectableType.Points] * valueCoins).ToString();
        playerScore.SetActive(true);
        
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
