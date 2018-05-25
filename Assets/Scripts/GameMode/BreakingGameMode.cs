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
    public int activeRabbits = 0;
    public int maxRabbits = 3;

    [SerializeField]
    Material alternativeMaterial;

    int nbDeadPlayers = 0;

    public int ActivePots
    {
        get
        {
            return activePots;
        }

        set
        {
            activePots = value;
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

        if (minigameVersion == 4)
        {
            for (int i = 0; i < playerReferences.Count; i++)
            {
                Player player = playerReferences[i].GetComponent<Player>();
                if (player != null)
                    player.OnDeathEvent += OnPlayerDeath;
            }
        }

        boardReference.GetComponent<BreakingGameSpawner>().withTrappedPots = withTrappedPots;
        FindObjectOfType<BreakingPickupHandler>().InitPickups((minigameVersion < 4), (minigameVersion == 2));

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
            necessaryPointsForRune = 300;
            foreach (BoardFloor f in FindObjectsOfType<BoardFloor>())
            {
                f.GetComponentInChildren<Renderer>().material = alternativeMaterial;
            }


            nbDeadPlayers = 0;
        }
    }

    float minigameTimer;

    public void LaunchTimer()
    {
        if (minigameVersion == 4)
        {
            minigameTimer = 0.0f;
        }
        else
        {
            GameManager.Instance.GameFinalTimer = timer;
            GameManager.Instance.LaunchFinalTimer();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (GameManager.CurrentState != GameState.Normal)
            return;
        minigameTimer += Time.deltaTime;
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

        if (minigameVersion != 4)
        {
            for (int i = 0; i < curNbPlayers; i++)
            {
                curScore += GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().NbPoints;
                pointsObjective += necessaryPointsForRune;
            }
        }
        else
        {
            pointsObjective += necessaryPointsForRune;
            if (curNbPlayers == 2)
                curScore = Mathf.Max(GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<Player>().NbPoints,
                    GameManager.Instance.PlayerStart.PlayersReference[1].GetComponent<Player>().NbPoints);
            else
                curScore = GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<Player>().NbPoints;
        }

        currentScore = curScore;
        return curScore >= pointsObjective;
    }

    public void OnPlayerDeath(Player player)
    {
        player.gameObject.SetActive(false);
        player.NbPoints = (int)minigameTimer * 10;
        nbDeadPlayers++;
        if (nbDeadPlayers == curNbPlayers)
            GameManager.Instance.ScoreScreenReference.RankPlayersByPoints();
    }

}
