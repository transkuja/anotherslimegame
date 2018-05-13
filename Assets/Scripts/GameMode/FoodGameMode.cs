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

    public float reactionTime = 1.0f;
    [HideInInspector]
    public Vector3[] startingPositions = new Vector3[4];

    [Header("Pancakes settings")]
    [SerializeField]
    GameObject platePrefab;
    [SerializeField]
    Transform platesLocation;
    [SerializeField]
    Transform pileOfPancakesLocation;
    [SerializeField]
    GameObject pancakeTopPrefab;
    [SerializeField]
    GameObject pancakePrefab;

    [SerializeField]
    int numberOfPancakesPerPile;
    bool[] eatPancakeOnNextInput = new bool[4];

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

    void CreateAPileOfPancakes(int _playerIndex, bool _init = false)
    {
        GameObject pile = new GameObject("Pile of Pancakes");
        pile.transform.SetParent(pileOfPancakesLocation.GetChild(_playerIndex));
        pile.transform.localPosition = Vector3.zero;

        GameObject plate = Instantiate(platePrefab, pile.transform);
        plate.transform.localPosition = Vector3.up * 0.2f;

        for (int i = 0; i < numberOfPancakesPerPile - 1; i++)
        {
            GameObject pancake = Instantiate(pancakePrefab, pile.transform);
            pancake.transform.localPosition = (i + 1) * Vector3.up * 0.2f;
        }
        GameObject topPancake = Instantiate(pancakeTopPrefab, pile.transform);
        topPancake.transform.localPosition = numberOfPancakesPerPile * Vector3.up * 0.2f;
        if (!_init)
            ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, pile.transform.position, Quaternion.identity, true, true);
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

            CreateAPileOfPancakes(i, true);
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
        Transform pileOfPancake = pileOfPancakesLocation.GetChild((int)_controller.PlayerIndex).GetChild(0);

        if (eatPancakeOnNextInput[(int)_controller.PlayerIndex])
        {
            DestroyImmediate(pileOfPancake.GetChild(pileOfPancake.childCount - 1).gameObject);
        }
        eatPancakeOnNextInput[(int)_controller.PlayerIndex] = !eatPancakeOnNextInput[(int)_controller.PlayerIndex];

        //if (platesLocation.GetChild((int)_controller.PlayerIndex).childCount < currentPlayer.NbPoints / 150)
        //{
        if (pileOfPancake.childCount == 1)
        {
            GameObject plate = pileOfPancake.GetChild(0).gameObject;
            plate.transform.SetParent(platesLocation.GetChild((int)_controller.PlayerIndex));
            plate.transform.localPosition = platesLocation.GetChild((int)_controller.PlayerIndex).childCount * Vector3.up * 0.2f;

            Destroy(pileOfPancake.gameObject);
            CreateAPileOfPancakes((int)_controller.PlayerIndex);
        }

        FoodMeterHandler.FoodMeterIncrease((int)_controller.PlayerIndex);

    }

    //public void BadInput(PlayerControllerFood _controller)
    //{
    //     score --
    //    Player currentPlayer = GameManager.Instance.PlayerStart.PlayersReference[(int)_controller.PlayerIndex].GetComponent<Player>();
    //    currentPlayer.UpdateCollectableValue(CollectableType.Points, -scoreStep);

    //    int platesCount = platesLocation.GetChild((int)_controller.PlayerIndex).childCount;
    //    if (platesCount > currentPlayer.NbPoints / 150)
    //        Destroy(platesLocation.GetChild((int)_controller.PlayerIndex).GetChild(platesCount - 1).gameObject);

    //}

    public override void EndMinigame()
    {
        minigameUI.SetActive(false);
    }
}
