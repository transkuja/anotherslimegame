using UWPAndXInput;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.UI;


public class ScoreScreen : MonoBehaviour {
    public GameObject prefabPlayerScore;

    public Dictionary<Player, GameObject> scorePanelPlayer = new Dictionary<Player, GameObject>();
    public int rank = 0;

    uint nbrOfPlayersAtTheEnd = 0;

    // The root gameobject containing UI for minigame 
    [SerializeField]
    GameObject minigameUI;

    // The child gameobject receiving data about rune objective
    [SerializeField]
    GameObject runeObjectiveUI;

    // The child gameobject containing the "Replay?" screen
    [SerializeField]
    GameObject replayScreen;

    bool goToRuneScreen = false;
    bool canExit = false;
    bool startExitTimer = false;
    float timerCanExit = 1.0f;
    bool objectiveFailedWhenRelevant = false;

    public void DEBUG_SetMinigameReplayable()
    {
        objectiveFailedWhenRelevant = true;
        canExit = true;
    }

    private void Awake()
    {
        GameManager.Instance.RegisterScoreScreenPanel(this);
        gameObject.SetActive(false);
    }

    public void Init()
    {
        goToRuneScreen = false;
        canExit = false;
        startExitTimer = false;
        timerCanExit = 2.0f;
        rank = 0;
        //for (int i = 1; i < podium.transform.childCount - 1; i++)
        //    podium.transform.GetChild(i).gameObject.SetActive(true);
        //podium.transform.GetChild(5).gameObject.SetActive(false);

        for (int i = 1; i < transform.childCount; i++)
            transform.GetChild(i).GetChild(0).gameObject.SetActive(true);

        transform.GetChild(0).gameObject.SetActive(true);

        replayScreen.SetActive(false);
        runeObjectiveUI.SetActive(false);
        gameObject.SetActive(false);

        if (GetComponent<Canvas>().worldCamera == null)
            GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void RefreshScores(Player player, float _time = -1, TimeFormat timeFormat = TimeFormat.MinSec)
    {
        float time = (_time == -1) ? Time.timeSinceLevelLoad : _time;
        SlimeDataContainer container = SlimeDataContainer.instance;
        if (rank == 0)
        {
            container.lastRanks = new int[4];
            container.lastScores = new float[4];
        }

        if (container != null)
        {
            container.lastRanks[rank] = (int)player.GetComponent<PlayerController>().playerIndex;
        }

        rank++;
        if (rank == 1)
            GameManager.Instance.consecutiveVictories[(int)player.PlayerController.playerIndex]++;

        if (rank > 4)
        {
            Debug.LogWarning("RefreshScores should not have been called or rank has not been reset ...");
            rank = 0;
            return;
        }

        string timeStr;
        if(_time == -1f)
        {
            timeStr = "---";
        }
        else
        {
            timeStr = TimeFormatUtils.GetFormattedTime(time, timeFormat);
        }

        if(GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            //if (transform.childCount >= rank - 1) // who did this ugly line?
            //{
            if (GameManager.Instance.CurrentGameMode.GetType() == typeof(KartGameMode))
            {
                player.NbPoints = (int)time;
            }

            container.lastScores[(int)player.GetComponent<PlayerController>().playerIndex] = player.NbPoints;

            //}
        }
        else
        {
            if (rank == 1)
            {
                GameManager.Instance.LaunchFinalTimer();
            }

            //if (transform.childCount >= rank - 1) // who did this ugly line?
            //{
            container.lastScores[(int)player.GetComponent<PlayerController>().playerIndex] = player.NbPoints;

            //}
        }
        player.rank = rank;

        nbrOfPlayersAtTheEnd++;
        CheckEndGame();
    }

    public void RankPlayersByPoints()
    {
        List<GameObject> playersReference = GameManager.Instance.PlayerStart.PlayersReference;
        List<Player> remainingPlayers = new List<Player>();

        // Fix bad ranking shown?
        if (GameManager.Instance.CurrentGameMode.GetType() == typeof(PushGameMode))
        {
            Collectable[] coinscoinscoins = FindObjectsOfType<Collectable>();
            for (int i = 0; i < coinscoinscoins.Length; ++i)
            {
                coinscoinscoins[i].IsAttracted = false;
                coinscoinscoins[i].GetComponent<PoolChild>().ReturnToPool();
            }
        }

        for (int i = 0; i < playersReference.Count; i++)
        {
            Player _curPlayer = playersReference[i].GetComponent<Player>();
            if (!_curPlayer.HasFinishedTheRun)
            {
                _curPlayer.HasFinishedTheRun = true;

                if (remainingPlayers.Count == 0)
                    remainingPlayers.Add(_curPlayer);
                else
                    for (int j = 0; j < remainingPlayers.Count; j++)
                    {
                        if (remainingPlayers[remainingPlayers.Count - (j + 1)].NbPoints > _curPlayer.NbPoints)
                        {
                            remainingPlayers.Insert(remainingPlayers.Count - j, _curPlayer);
                            break;
                        }
                        else if (j == remainingPlayers.Count - 1)
                        {
                            remainingPlayers.Insert(0, _curPlayer);
                            break;
                        }
                        else
                            continue;
                    }
            }
        }
        RefreshScoresTimeOver(remainingPlayers.ToArray());
    }

    public void RankKartPlayers()
    {
        List<GameObject> playersReference = GameManager.Instance.PlayerStart.PlayersReference;
        List<Player> timeOutPlayers = new List<Player>();
        List<Player> finishedInTimePlayers = new List<Player>();

        for (int i = 0; i < playersReference.Count; i++)
        {
            Player _curPlayer = playersReference[i].GetComponent<Player>();
            if (!_curPlayer.HasFinishedTheRun)
            {
                _curPlayer.HasFinishedTheRun = true;
                _curPlayer.NbPoints = _curPlayer.GetComponent<PlayerControllerKart>().checkpointsPassed;

                if (timeOutPlayers.Count == 0)
                    timeOutPlayers.Add(_curPlayer);
                else
                    for (int j = 0; j < timeOutPlayers.Count; j++)
                    {
                        if (timeOutPlayers[timeOutPlayers.Count - (j + 1)].NbPoints > _curPlayer.NbPoints)
                        {
                            timeOutPlayers.Insert(timeOutPlayers.Count - j, _curPlayer);
                            break;
                        }
                        else if (j == timeOutPlayers.Count - 1)
                        {
                            timeOutPlayers.Insert(0, _curPlayer);
                            break;
                        }
                        else
                            continue;
                    }
            }
            else
            {
                if (finishedInTimePlayers.Count == 0)
                    finishedInTimePlayers.Add(_curPlayer);
                else
                    for (int j = 0; j < finishedInTimePlayers.Count; j++)
                    {
                        if (finishedInTimePlayers[finishedInTimePlayers.Count - (j + 1)].FinishTime < _curPlayer.FinishTime)
                        {
                            finishedInTimePlayers.Insert(finishedInTimePlayers.Count - j, _curPlayer);
                            break;
                        }
                        else if (j == finishedInTimePlayers.Count - 1)
                        {
                            finishedInTimePlayers.Insert(0, _curPlayer);
                            break;
                        }
                        else
                            continue;
                    }
            }

        }
        finishedInTimePlayers.AddRange(timeOutPlayers);
        for (int i = 0; i < finishedInTimePlayers.Count; i++)
        {
            RefreshScores(finishedInTimePlayers[i], finishedInTimePlayers[i].FinishTime, TimeFormat.MinSecMil);
        }
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
                players[i].SetActive(false);
                if (players[i].GetComponent<Player>().cameraReference)
                    players[i].GetComponent<Player>().cameraReference.SetActive(false);
            }
            GameManager.Instance.CurrentGameMode.EndMinigame();

            EnablePodium(players);


            RuneObjective runeObjective = GameManager.Instance.CurrentGameMode.runeObjective;
            if (runeObjective != RuneObjective.None)
            {
                if (!GameManager.Instance.CurrentGameMode.IsRuneUnlocked())
                {
                    // Initialize rune objective screen values
                    if (runeObjective == RuneObjective.Points)
                    {
                        runeObjectiveUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "0";
                        runeObjectiveUI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = (GameManager.Instance.CurrentGameMode.necessaryPointsForRune * GameManager.Instance.PlayerStart.ActivePlayersAtStart).ToString();
                        runeObjectiveUI.transform.GetChild(0).GetChild(0).localPosition += Vector3.right * 60;
                        runeObjectiveUI.transform.GetChild(0).GetChild(1).localPosition += Vector3.left * 60;
                    }
                    else if (runeObjective == RuneObjective.Time)
                    {
                        if (GameManager.Instance.CurrentGameMode.GetType() == typeof(KartGameMode) && ((KartGameMode)GameManager.Instance.CurrentGameMode).firstFinishTime < 0f)
                        {
                            //TODO: Skip to failed screen
                            runeObjectiveUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "0";
                            runeObjectiveUI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "20 Try Again";
                        }
                        else
                        {
                            runeObjectiveUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "00:00:00";
                            runeObjectiveUI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = TimeFormatUtils.GetFormattedTime(GameManager.Instance.CurrentGameMode.necessaryTimeForRune, TimeFormat.MinSecMil);
                        }
                    }

                    goToRuneScreen = true;
                }
                else
                    startExitTimer = true;
            }
            else
                startExitTimer = true;
        }
    }

    void EnablePodium(List<GameObject> _players)
    {
        SceneManager.LoadScene("Podium");
    }

    void PlayAddToScoreAnimation(RuneObjective _objectiveType)
    {
        if (_objectiveType == RuneObjective.Points)
        {
            StartCoroutine(AddScoreToTotalPoints());
        }
        else if (_objectiveType == RuneObjective.Time)
        {
            StartCoroutine(AddTimeToTotalTime());
        }
        
    }

    IEnumerator AddScoreToTotalPoints()
    {
        int step = 5;
        int totalScore = 0;
        int currentPlayerIndex = 0;
        float maxTime = 2.0f;

        Player player = GameManager.Instance.PlayerStart.PlayersReference[currentPlayerIndex].GetComponent<Player>();
        transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().fontSize *= 2;
        transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<AnimText>().enabled = true;
        int curPlayerScore = player.NbPoints;
        float tick;
        if (curPlayerScore == 0)
            tick = 0;
        else
            tick = maxTime/(curPlayerScore/(float)step);

        while (currentPlayerIndex < 2)
        {
            if (curPlayerScore - step < 0)
            {
                totalScore += curPlayerScore;
                curPlayerScore = 0;
                transform.GetChild(currentPlayerIndex+1).GetChild(0).GetChild(2).GetComponent<Text>().text = "0pts";
                transform.GetChild(currentPlayerIndex+1).GetChild(0).GetChild(2).GetComponent<Text>().fontSize /= 2;
                transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<AnimText>().enabled = false;

                currentPlayerIndex++;
                if (currentPlayerIndex < GameManager.Instance.ActivePlayersAtStart)
                {
                    transform.GetChild(currentPlayerIndex+1).GetChild(0).GetChild(2).GetComponent<Text>().fontSize *= 2;
                    transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<AnimText>().enabled = true;
                    player = GameManager.Instance.PlayerStart.PlayersReference[currentPlayerIndex].GetComponent<Player>();
                    curPlayerScore = player.NbPoints;
                    if (curPlayerScore == 0)
                        tick = 0;
                    else
                        tick = maxTime / (curPlayerScore / (float)step);
                }
                else
                    tick = 0.0f;
            }
            else
            {
                curPlayerScore -= step;
                totalScore += step;
            }

            transform.GetChild(currentPlayerIndex+1).GetChild(0).GetChild(2).GetComponent<Text>().text = curPlayerScore + "pts";
            runeObjectiveUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = totalScore.ToString();

            yield return new WaitForSeconds(tick);
        }

        if (GameManager.Instance.CurrentGameMode.checkRuneObjective())
        {
            PlayGetRuneAnimation();
            Invoke("ShowUiUpdating", 1.0f);
        }
        else
        {
            PlayFailedObjectiveAnimation();
        }

        startExitTimer = true;
    }

    IEnumerator AddTimeToTotalTime()
    {
        float step = 0.2f;
        float totalTime = 0.0f;
        float maxTime = 2.0f;

        //Find the shortest time from all the players
        float shortestTime = Mathf.Infinity;
        foreach(GameObject pGo in GameManager.Instance.PlayerStart.PlayersReference)
        {
            if(pGo.GetComponent<Player>().FinishTime < shortestTime)
                shortestTime = pGo.GetComponent<Player>().FinishTime;
        }

        float curPlayerTime = shortestTime;
        float tick = maxTime / (curPlayerTime / step);
        bool isDone = false;
        while (!isDone)
        {
            curPlayerTime -= step;
            totalTime += step;

            if (totalTime >= shortestTime)
            {
                totalTime = shortestTime;
                isDone = true;
            }
            
            runeObjectiveUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = TimeFormatUtils.GetFormattedTime(totalTime, TimeFormat.MinSecMil);

            yield return new WaitForSeconds(tick);
        }

        //Temp fix until we know why it was detached in the first place
        if(GameManager.Instance.CurrentGameMode.checkRuneObjective == null && GameManager.Instance.CurrentGameMode.GetType() == typeof(KartGameMode))
        {
            GameManager.Instance.CurrentGameMode.checkRuneObjective = ((KartGameMode)GameManager.Instance.CurrentGameMode).CheckRuneObjectiveForKart;
        }
        if (GameManager.Instance.CurrentGameMode.checkRuneObjective())
        {
            PlayGetRuneAnimation();
            Invoke("ShowUiUpdating", 1.0f);
        }
        else
        {
            PlayFailedObjectiveAnimation();
        }

        startExitTimer = true;
    }

    void ShowUiUpdating()
    {
        GameManager.UiReference.GetComponent<UI>().TooglePersistenceUI(true);
        Invoke("ShowUiUpdatingNextStep", 0.5f);
    }

    void ShowUiUpdatingNextStep()
    {
        GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<Player>().UpdateCollectableValue(CollectableType.Rune);
        GameManager.Instance.CurrentGameMode.UnlockRune();
    }


    void PlayFailedObjectiveAnimation()
    {
        Text failedText = runeObjectiveUI.transform.GetChild(1).GetComponent<Text>();
        failedText.text = "Failed";
        failedText.color = Color.red;

        runeObjectiveUI.transform.GetChild(1).gameObject.SetActive(true);
        objectiveFailedWhenRelevant = true;
    }

    void PlayGetRuneAnimation()
    {
        Text wellDoneText = runeObjectiveUI.transform.GetChild(1).GetComponent<Text>();
        wellDoneText.text = "Well done!";
        wellDoneText.color = Color.green;

        runeObjectiveUI.transform.GetChild(1).gameObject.SetActive(true);
  
    }


    void Update()
    {
        if (startExitTimer)
            timerCanExit -= Time.deltaTime;
        if (timerCanExit < 0.0f)
            canExit = true;

        // TODO : Multi to be handled
        if (!GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().PlayerIndexSet)
            return;

        
        if (goToRuneScreen)
        {
            if (GamePad.GetState(GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().playerIndex).Buttons.A == ButtonState.Pressed
                // Keyboard input
                || Input.GetKeyDown(KeyCode.Return))
            {                
                for (int i = 1; i < transform.childCount-2; i++)
                    transform.GetChild(i).gameObject.SetActive(false);
                // Activate objective texts
                runeObjectiveUI.SetActive(true);
                goToRuneScreen = false;

                PlayAddToScoreAnimation(GameManager.Instance.CurrentGameMode.runeObjective);
            }
        }

        if (canExit)
        {
            if (GamePad.GetState(GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().playerIndex).Buttons.Start == ButtonState.Pressed
                || GamePad.GetState(GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().playerIndex).Buttons.A == ButtonState.Pressed
                // Keyboard input
                || Input.GetKeyDown(KeyCode.Return))
            {
                if (GameManager.Instance.CurrentGameMode.IsMiniGame())
                {
                    if ((GameManager.Instance.DataContainer != null && GameManager.Instance.DataContainer.launchedFromMinigameScreen) || objectiveFailedWhenRelevant)
                    {
                        runeObjectiveUI.SetActive(false);
                        replayScreen.SetActive(true);
                    }
                    else
                    {
                        SceneManager.LoadScene(1); // ugly?
                    }
                }
                //ExitToMainMenu();
            }
        }
        // TODO: handle pause input here?
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
