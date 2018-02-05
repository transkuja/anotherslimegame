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

    // A GameObject that contains children + a camera 
    [SerializeField]
    Transform podium;

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

    public void RefreshScores(Player player, float _time = -1, TimeFormat timeFormat = TimeFormat.MinSec)
    {
        float time = (_time == -1) ? Time.timeSinceLevelLoad : _time;

        rank++;
        if (rank > 4)
        {
            Debug.LogWarning("RefreshScores should not have been called or rank has not been reset ...");
            rank = 0;
            return;
        }

        String timeStr = TimeFormatUtils.GetFormattedTime(time, timeFormat);

        if(GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            if (transform.childCount >= rank - 1) // who did this ugly line?
            {
                if (GameManager.Instance.CurrentGameMode.GetType() == typeof(KartGameMode))
                {
                    transform.GetChild(rank - 1).GetComponent<PlayerScore>().SetScoreMiniGameTimeOnly(
                        (int)player.PlayerController.PlayerIndex,
                        timeStr
                    );
                }
                else
                {
                    transform.GetChild(rank - 1).GetComponent<PlayerScore>().SetScoreMiniGamePtsOnly(
                        (int)player.PlayerController.PlayerIndex,
                        player.NbPoints.ToString()
                    );
                }

                transform.GetChild(rank - 1).gameObject.SetActive(true);

            }
        }
        else
        {
            if (rank == 1)
            {
                GameManager.Instance.LaunchFinalTimer();
            }

            if (transform.childCount >= rank - 1) // who did this ugly line?
            {
                transform.GetChild(rank - 1).GetComponent<PlayerScore>().SetScoreDefault(
                    (int)player.PlayerController.PlayerIndex,
                    GameManager.Instance.isTimeOver ? "Timeout" : timeStr,
                    player.NbPoints.ToString()
                );

                transform.GetChild(rank - 1).gameObject.SetActive(true);

            }
        }
        player.rank = rank;

        if (rank == 1)
            player.Anim.SetBool("hasFinishedFirst", true);
        else if (rank == 4)
            player.Anim.SetBool("hasFinishedLast", true);
        else
            player.Anim.SetBool("hasFinished", true);

        nbrOfPlayersAtTheEnd++;
        CheckEndGame();
    }

    public void RankPlayersByPoints()
    {
        List<GameObject> playersReference = GameManager.Instance.PlayerStart.PlayersReference;
        List<Player> remainingPlayers = new List<Player>();

        for (int i = 0; i < playersReference.Count; i++)
        {
            Player _curPlayer = playersReference[i].GetComponent<Player>();
            if (!_curPlayer.HasFinishedTheRun)
            {
                _curPlayer.HasFinishedTheRun = true;
                if (remainingPlayers.Count == 0
                    || remainingPlayers[remainingPlayers.Count - 1].NbPoints > _curPlayer.NbPoints)
                {
                    remainingPlayers.Add(_curPlayer);
                }
                else
                    remainingPlayers.Insert(remainingPlayers.Count - 1, _curPlayer);
            }
        }
        RefreshScoresTimeOver(remainingPlayers.ToArray());
    }

    void RefreshScoresTimeOver(Player[] _remainingPlayers)
    {
        for (int i = 0; i < _remainingPlayers.Length; i++)
            RefreshScores(_remainingPlayers[i]);
    }

    void CheckEndGame()
    {
        if (nbrOfPlayersAtTheEnd == GameManager.Instance.PlayerStart.ActivePlayersAtStart)
        {
            List<GameObject> players = GameManager.Instance.PlayerStart.PlayersReference;
            for (int i = 0; i < players.Count; i++)
            {
                Player curPlayer = players[i].GetComponent<Player>();
                if(curPlayer.cameraReference)
                    curPlayer.cameraReference.SetActive(false);
                curPlayer.transform.SetParent(podium.GetChild(curPlayer.rank));
                curPlayer.transform.localPosition = Vector3.zero;
                curPlayer.transform.localRotation = Quaternion.identity;
            }
            podium.GetChild(5).gameObject.SetActive(true);
            GameManager.UiReference.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }

    void Update()
    {

        // TODO : Multi to be handled
        if (!GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().PlayerIndexSet)
            return;

        if (GamePad.GetState(GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().playerIndex).Buttons.Start == ButtonState.Pressed)
        {
            if (GameManager.Instance.CurrentGameMode.IsMiniGame())
            {
                SceneManager.LoadScene(1); // ugly?
            }
            //ExitToMainMenu();
        }
        // TODO: handle pause input here?
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
