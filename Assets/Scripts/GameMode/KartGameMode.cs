using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class KartGameMode : GameMode {

    [SerializeField]
    PlayerControllerKart.DrivingCondition defaultDrivingCondition = PlayerControllerKart.DrivingCondition.Normal;

    public int NumberOfLaps = 5;
    int currentPtsIndex = 4;
    public float maxTime = 75f;
    float playersLeft;

    float timer;

    public float firstFinishTime = -1.0f;

    [HideInInspector]
    private KartArrival arrival;

    public KartArrival Arrival
    {
        get
        {
            return arrival;
        }

        set
        {
            arrival = value;
            arrival.NumberOfLaps = NumberOfLaps;
        }
    }

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this, minigameVersion);
        playersLeft = playerReferences.Count;
        for (int i = 0; i < playerReferences.Count; i++)
        {
            Player player = playerReferences[i].GetComponent<Player>();
            if (player != null)
            {
                player.OnDeathEvent += OnPlayerDeath;
                player.CallOnValueChange(PlayerUIStat.Laps, 1);
            }
            PlayerControllerKart pk = playerReferences[i].GetComponent<PlayerControllerKart>();
            if (pk)
                pk.CurrentCondition = defaultDrivingCondition;
        }
        checkRuneObjective = CheckRuneObjectiveForKart;
        LaunchTimer();
    }

    public void LaunchTimer()
    {
        timer = 0.0f;
        GameManager.Instance.GameFinalTimer = maxTime;
        GameManager.Instance.LaunchFinalTimer();
    }

    protected override void Update()
    {
        base.Update();
        if (GameManager.CurrentState != GameState.Normal)
            return;
        timer += Time.deltaTime;
    }

    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
    }

    public override void ObtainMoneyBasedOnScore()
    {
        float result = 0;
        if (GameManager.Instance.DataContainer != null && GameManager.Instance.DataContainer.launchedFromMinigameScreen)
        {
            int[] minmax = MinigameDataUtils.GetMinMaxGoldTargetValues(this, minigameVersion);
            for (int i = 0; i < curNbPlayers; i++)
            {
                int span = minmax[1] - minmax[0];
                float lerpParam = 1 - (GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().NbPoints - minmax[1]) / (float)span;
                float tmp = Mathf.Lerp(0, 50 + 25 * curNbPlayers, Mathf.Clamp(lerpParam, 0, 1));
                tmp = Mathf.Clamp(tmp, 0, 500);
                result += tmp;
            }
        }

        GameManager.Instance.GlobalMoney += (int)(result / curNbPlayers);
    }

    public override void PlayerHasFinished(Player player)
    {
        if(Mathf.Approximately(firstFinishTime, -1.0f))
        {
            firstFinishTime = timer;
        }

        player.FinishTime = timer;
        player.HasFinishedTheRun = true;
        playersLeft--;
        if(playersLeft <= 0)
        {
            GameManager.Instance.ScoreScreenReference.RankKartPlayers();
        }
    }

    public bool CheckRuneObjectiveForKart()
    {
        return firstFinishTime >= 0f && firstFinishTime <= necessaryTimeForRune;
    }

    public void OnPlayerDeath(Player player)
    {
        player.transform.position = player.respawnPoint.position;
        player.transform.rotation = player.respawnPoint.rotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
