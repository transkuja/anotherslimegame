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
    InputsMeterHandler inputsMeterHandler;

    public GameObject foodMeterUI;
    public GameObject inputsUI;

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

    public InputsMeterHandler InputsMeterHandler
    {
        get
        {
            if (inputsMeterHandler == null)
                inputsMeterHandler = inputsUI.GetComponent<InputsMeterHandler>();
            return inputsMeterHandler;
        }

        set
        {
            inputsMeterHandler = value;
        }
    }

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this, minigameVersion);

        for (int i = 0; i < playerReferences.Count; i++)
        {
            Destroy(playerReferences[i].GetComponent<PlayerControllerHub>());
            PlayerControllerFood pc = playerReferences[i].AddComponent<PlayerControllerFood>();
            pc.playerIndex = (PlayerIndex)i;
            playerReferences[i].GetComponent<Rigidbody>().useGravity = true;
            playerReferences[i].GetComponent<Player>().NbPoints = 0;
        }
    

        LaunchTimer();
        // Food meter init
        FoodMeterHandler = foodMeterUI.GetComponent<FoodMeterHandler>();

        // Init Inputs meter
        InputsMeterHandler = inputsUI.GetComponent<InputsMeterHandler>();

    }

    public void LaunchTimer()
    {
        GameManager.Instance.GameFinalTimer = timer;
        GameManager.Instance.LaunchFinalTimer();
    }

    // TODO: should have the player index as a parameter to play with 4P
    public void GoodInput(PlayerControllerFood _controller)
    {
        // score ++
        GameManager.Instance.PlayerStart.PlayersReference[(int)_controller.PlayerIndex].
            GetComponent<Player>().UpdateCollectableValue(CollectableType.Points, scoreStep * _controller.CurrentCombo);

        // food meter ++
        // Faire gonfler le slime
        FoodMeterHandler.FoodMeterIncrease((int)_controller.PlayerIndex);

        // input gauge ++
        // Osef?
        // DEPRECATED: old GP
        //InputsMeterHandler.InputMeterIncrease((int)_controller.PlayerIndex);

        // Update Combo
        _controller.CurrentCombo++;
    }
}
