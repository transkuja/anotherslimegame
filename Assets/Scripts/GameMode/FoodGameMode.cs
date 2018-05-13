﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class FoodGameMode : GameMode {

    [Header("Food minigame settings")]
    public float timer;

    // Score settings
    public int scoreStep = 10;

    FoodMeterHandler foodMeterHandler;
    public InputTracksHandler inputTracksHandler;

    public GameObject minigameUI;
    public GameObject foodMeterUI;

    public float inputSpeed;
    public float miniTail; // No, it's not deprecated
    public float maxTail;

    public float maxScale;

    public bool enableBadInputs = false;

    public float reactionTime = 1.0f;
    [HideInInspector]
    public Vector3[] startingPositions = new Vector3[4];

    public GameObject platePrefab;
    public Transform platesLocation;

    public FoodMeterHandler FoodMeterHandler
    {
        get
        {
            if (foodMeterHandler == null)
                foodMeterHandler = foodMeterUI.GetComponent<FoodMeterHandler>();
            return foodMeterHandler;
        }

        set
        {
            foodMeterHandler = value;
        }
    }

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this, minigameVersion);

        for (int i = 0; i < playerReferences.Count; i++)
        {
            Destroy(playerReferences[i].GetComponent<PlayerControllerHub>());
            PlayerController pc = playerReferences[i].AddComponent<PlayerController>();
            pc.playerIndex = (PlayerIndex)i;
            pc.PlayerIndexSet = true;

            playerReferences[i].GetComponent<Rigidbody>().useGravity = false;
            playerReferences[i].GetComponent<Rigidbody>().isKinematic = true;
            playerReferences[i].GetComponent<Player>().NbPoints = 0;
        }
    

        LaunchTimer();

        if (GameManager.Instance.PlayerStart.DEBUG_SkipMinigamesRuleScreen)
        {
            minigameUI.SetActive(true);
            // Food meter init
            FoodMeterHandler = foodMeterUI.GetComponent<FoodMeterHandler>();

            inputTracksHandler.StartGame();
        }
    }

    public override void RepositionPlayers()
    {
        if (curNbPlayers == 4)
        {
            for (int i = 0; i < 4; i++)
                startingPositions[i] = GameManager.Instance.PlayerStart.PlayersReference[i].transform.position;
            return;
        }

        Vector3[] positions = inputTracksHandler.ComputePlayerStartingPositions(curNbPlayers);

        for (int i = 0; i < curNbPlayers; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.position = positions[i] + Vector3.up;
            startingPositions[i] = GameManager.Instance.PlayerStart.PlayersReference[i].transform.position;
        }
    }
    public override void ExtractVersionData(int _minigameVersion)
    {
        if (_minigameVersion >= 2)
        {
            enableBadInputs = true;
            _minigameVersion -= 2;
        }

        if (_minigameVersion >= 1)
        {
            miniTail = 0.75f;
            maxTail = 1.5f;
            inputSpeed = 60.0f;
            _minigameVersion -= 1;
        }
        else
        {
            miniTail = 1.0f;
            maxTail = 2.0f;
            inputSpeed = 50.0f;
        }
    }

    public override void OnReadySetGoBegin()
    {
        base.OnReadySetGoBegin();
        minigameUI.SetActive(true);
        // Food meter init
        FoodMeterHandler = foodMeterUI.GetComponent<FoodMeterHandler>();

        inputTracksHandler.StartGame();
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
            playersReference[i].GetComponent<Player>().cameraReference = cameraReferences[i];
        }
    }

    public void GoodInput(PlayerControllerFood _controller)
    {
        // score ++
        Player currentPlayer = GameManager.Instance.PlayerStart.PlayersReference[(int)_controller.PlayerIndex].GetComponent<Player>();
        currentPlayer.UpdateCollectableValue(CollectableType.Points, (int)(scoreStep * _controller.CurrentCombo));

        if (platesLocation.GetChild((int)_controller.PlayerIndex).childCount < currentPlayer.NbPoints / 150)
        {
            GameObject plate = Instantiate(platePrefab, platesLocation.GetChild((int)_controller.PlayerIndex));
            plate.transform.localPosition = currentPlayer.NbPoints / 150 * Vector3.up * 0.2f;
        }

        FoodMeterHandler.FoodMeterIncrease((int)_controller.PlayerIndex);

    }

    public void BadInput(PlayerControllerFood _controller)
    {
        // score --
        Player currentPlayer = GameManager.Instance.PlayerStart.PlayersReference[(int)_controller.PlayerIndex].GetComponent<Player>();
        currentPlayer.UpdateCollectableValue(CollectableType.Points, -scoreStep);

        int platesCount = platesLocation.GetChild((int)_controller.PlayerIndex).childCount;
        if (platesCount > currentPlayer.NbPoints / 150)
            Destroy(platesLocation.GetChild((int)_controller.PlayerIndex).GetChild(platesCount - 1).gameObject);

    }

    public override void EndMinigame()
    {
        minigameUI.SetActive(false);
    }
}
