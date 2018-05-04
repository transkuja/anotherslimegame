using System.Collections;
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

            playerReferences[i].GetComponent<Rigidbody>().useGravity = true;
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
        GameManager.Instance.PlayerStart.PlayersReference[(int)_controller.PlayerIndex].
            GetComponent<Player>().UpdateCollectableValue(CollectableType.Points, (int)(scoreStep * _controller.CurrentCombo));

        FoodMeterHandler.FoodMeterIncrease((int)_controller.PlayerIndex);

    }
}
