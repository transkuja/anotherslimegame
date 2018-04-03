using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class KartGameMode : GameMode {
    float timer;

    float firstFinishTime = -1.0f;

    public KartArrival Arrival;

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this);

        for (int i = 0; i < playerReferences.Count; i++)
        {
            Player player = playerReferences[i].GetComponent<Player>();
            if (player != null)
                player.OnDeathEvent += OnPlayerDeath;
        }
        checkRuneObjective = CheckRuneObjectiveForKart;
        LaunchTimer();
    }

    public void LaunchTimer()
    {
        timer = 0.0f;
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

    public override void PlayerHasFinished(Player player)
    {
        if(Mathf.Approximately(firstFinishTime, -1.0f))
        {
            firstFinishTime = timer;
        }

        player.FinishTime = timer;
        GameManager.Instance.ScoreScreenReference.RefreshScores(player, timer, TimeFormat.MinSecMil);
    }

    public bool CheckRuneObjectiveForKart()
    {
        return firstFinishTime <= necessaryTimeForRune;
    }

    public void OnPlayerDeath(Player player)
    {
        player.transform.position = player.respawnPoint.position;
        player.transform.rotation = player.respawnPoint.rotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
