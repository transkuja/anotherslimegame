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

    // A GameObject that contains children + a camera 
    [SerializeField]
    Transform podium;

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

        rank++;
        if (rank == 1)
            GameManager.Instance.consecutiveVictories[(int)player.PlayerController.playerIndex]++;

        if (rank > 4)
        {
            Debug.LogWarning("RefreshScores should not have been called or rank has not been reset ...");
            rank = 0;
            return;
        }

        String timeStr = TimeFormatUtils.GetFormattedTime(time, timeFormat);

        if(GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            //if (transform.childCount >= rank - 1) // who did this ugly line?
            //{
                if (GameManager.Instance.CurrentGameMode.GetType() == typeof(KartGameMode))
                {
                    transform.GetChild(rank).GetComponent<PlayerScore>().SetScoreMiniGameTimeOnly(
                        (int)player.PlayerController.PlayerIndex,
                        timeStr,
                        (GameManager.Instance.ActivePlayersAtStart == 1)
                    );
                }
                else
                {
                    transform.GetChild(rank).GetComponent<PlayerScore>().SetScoreMiniGamePtsOnly(
                        (int)player.PlayerController.PlayerIndex,
                        player.NbPoints.ToString(),
                        (GameManager.Instance.ActivePlayersAtStart == 1)
                    );
                }

                transform.GetChild(rank).gameObject.SetActive(true);

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
                transform.GetChild(rank).GetComponent<PlayerScore>().SetScoreDefault(
                    (int)player.PlayerController.PlayerIndex,
                    GameManager.Instance.isTimeOver ? "Timeout" : timeStr,
                    player.NbPoints.ToString()
                );

                transform.GetChild(rank).gameObject.SetActive(true);

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
                players[i].SetActive(false);
                if (players[i].GetComponent<Player>().cameraReference)
                    players[i].GetComponent<Player>().cameraReference.SetActive(false);
            }

            EnablePodium(players);

            // Hide timer
            GameManager.UiReference.transform.GetChild(1).gameObject.SetActive(false);

            // Hide cursors
            GameManager.Instance.CurrentGameMode.cursors.DisableCursors();

            // Change render mode so we can see the UI updating
            GameManager.UiReference.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            if(minigameUI)
                minigameUI.SetActive(false);
            gameObject.SetActive(true);

            RuneObjective runeObjective = GameManager.Instance.CurrentGameMode.runeObjective;
            if (runeObjective != RuneObjective.None)
            {
                if (!GameManager.Instance.CurrentGameMode.IsRuneUnlocked())
                {
                    // Initialize rune objective screen values
                    if (runeObjective == RuneObjective.Points)
                    {
                        runeObjectiveUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "0";
                        runeObjectiveUI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = GameManager.Instance.CurrentGameMode.necessaryPointsForRune.ToString();
                        runeObjectiveUI.transform.GetChild(0).GetChild(0).localPosition += Vector3.right * 60;
                        runeObjectiveUI.transform.GetChild(0).GetChild(1).localPosition += Vector3.left * 60;
                    }
                    else if (runeObjective == RuneObjective.Time)
                    {
                        runeObjectiveUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "00:00:00";
                        runeObjectiveUI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = TimeFormatUtils.GetFormattedTime(GameManager.Instance.CurrentGameMode.necessaryTimeForRune, TimeFormat.MinSecMil);
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
        for (int i = 1; i < nbrOfPlayersAtTheEnd + 1; i++)
            podium.GetChild(i).gameObject.SetActive(true);

        foreach (GameObject curPlayer in _players)
        {
            int curPlayerRank = curPlayer.GetComponent<Player>().rank;
            GameObject podiumPlayer = podium.GetChild(curPlayerRank).gameObject;
            PlayerCosmetics curPlayerCosmetics = curPlayer.GetComponentInChildren<PlayerCosmetics>();
            PlayerCosmetics podiumPlayerCosmetics = podiumPlayer.GetComponentInChildren<PlayerCosmetics>();

            // Set color and face
            podiumPlayerCosmetics.UseColorFade = curPlayerCosmetics.UseColorFade;
            podiumPlayerCosmetics.SetUniqueColor(curPlayerCosmetics.BodyColor);
            podiumPlayerCosmetics.FaceType = curPlayerCosmetics.FaceType;
            podiumPlayerCosmetics.FaceEmotion = (curPlayerRank == 1 || curPlayerRank == 2) ? FaceEmotion.Winner : FaceEmotion.Loser;

            // Set customizables
            CustomizableSockets podiumCustomSockets = podiumPlayer.GetComponentInChildren<CustomizableSockets>();

            for (int i = 0; i < (int)CustomizableType.Size - 2; i++)
            {
                if (curPlayer.GetComponentInChildren<CustomizableSockets>().transform.GetChild(i).childCount > 0)
                {
                    GameObject customizable = Instantiate(curPlayer.GetComponentInChildren<CustomizableSockets>().transform.GetChild(i).GetChild(0).gameObject,
                        podiumCustomSockets.transform.GetChild(i));
                    customizable.GetComponent<ICustomizable>().Init(podiumCustomSockets.GetComponentInParent<Rigidbody>());
                    foreach (Rigidbody rb in customizable.GetComponentsInChildren<Rigidbody>())
                        rb.isKinematic = true;

                }
            }

            if (curPlayerRank == 1)
                podiumPlayer.GetComponentInChildren<Animator>().SetBool("hasFinishedFirst", true);
            else if (curPlayerRank == 4)
                podiumPlayer.GetComponentInChildren<Animator>().SetBool("hasFinishedLast", true);
            else
                podiumPlayer.GetComponentInChildren<Animator>().SetBool("hasFinished", true);
        }

        GetComponent<Canvas>().worldCamera.gameObject.SetActive(true);
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

        GameObject runePreview = podium.transform.GetChild(0).gameObject;

        Material mat = runePreview.GetComponentInChildren<MeshRenderer>().material;
        Color newColor = mat.color;
        newColor.a = 1.0f;
        mat.color = newColor;
        mat.SetColor("_EmissionColor", Color.magenta);

        runePreview.transform.GetChild(1).gameObject.SetActive(true);
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
                for (int i = 1; i < podium.transform.childCount; i++)
                    podium.transform.GetChild(i).gameObject.SetActive(false);

                // Activate rune preview
                podium.transform.GetChild(0).gameObject.SetActive(true);

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
                        podium.transform.GetChild(0).gameObject.SetActive(false);
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
