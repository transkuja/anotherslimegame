using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class BreakingGameMode : GameMode {

    [Header("Breaking settings")]
    public float timer;

    public bool withTrappedPots = false;
    public GameObject boardReference;
    public RuntimeAnimatorController RestrainedAnimatorController;

    public int activePots = 0;

    public int ActivePots
    {
        get
        {
            return activePots;
        }

        set
        {
            activePots = value;
            Debug.Log(activePots);
        }
    }

    private void Start()
    {
        if (boardReference == null)
            Debug.LogError("BreakingGameMode: Board reference is not linked to gamemode!");

        if (minigameVersion < 2)
            minigameVersion = 2;
    }

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this, minigameVersion);
        checkRuneObjective = CheckRuneObjectiveForBreaking;

        boardReference.GetComponent<BreakingGameSpawner>().withTrappedPots = withTrappedPots;

        LaunchTimer();
        ColorFloorHandler.Init(GameManager.Instance.ActivePlayersAtStart, boardReference);
    }

    public override void ExtractVersionData(int _minigameVersion)
    {
        // Default version = 2
        if (minigameVersion == 2)
            necessaryPointsForRune = 200;
        if (minigameVersion == 3)
        {
            withTrappedPots = true;
            necessaryPointsForRune = 200;
        }
        // Breakable ground version = 4
        if (minigameVersion == 4)
        {
            necessaryPointsForRune = 90;
        }
    }

    public void LaunchTimer()
    {
        GameManager.Instance.GameFinalTimer = timer;
        GameManager.Instance.LaunchFinalTimer();
    }

    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        base.AttributeCamera(activePlayersAtStart, cameraReferences, playersReference);
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = playersReference[i];

            go.GetComponent<Player>().cameraReference = cameraReferences[i];
            cameraReferences[i].SetActive(true);
        }
    }

    bool CheckRuneObjectiveForBreaking()
    {
        int pointsObjective = 0;
        int curScore = 0;
        for (int i = 0; i < curNbPlayers; i++)
        {
            curScore += GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().NbPoints;
            pointsObjective += necessaryPointsForRune;
        }
        currentScore = curScore;
        return curScore >= pointsObjective;
    }

}
