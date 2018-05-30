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
    Vector3 offsetPlates = new Vector3(-3.5f, 0.92f, -4.39f);
    Vector3 offsetPancakes = new Vector3(0.0f, 0.92f, -3.39f);
    GameObject platesLocation;
    GameObject pileOfPancakesLocation;

    [SerializeField]
    GameObject platePrefab;
    [SerializeField]
    GameObject pancakeTopPrefab;
    [SerializeField]
    GameObject pancakePrefab;

    [SerializeField]
    int numberOfPancakesPerPile;
    bool[] eatPancakeOnNextInput = new bool[4];

    [SerializeField]
    RuntimeAnimatorController specificAnimator;

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
        float[] possibleTopRotations = { 45.0f, 66.0f, 0.0f, 133.0f };
        pile.transform.SetParent(pileOfPancakesLocation.transform.GetChild(_playerIndex));
        pile.transform.localPosition = Vector3.zero;

        GameObject plate = Instantiate(platePrefab, pile.transform);
        plate.transform.localPosition = Vector3.zero;

        for (int i = 0; i < numberOfPancakesPerPile - 1; i++)
        {
            GameObject pancake = Instantiate(pancakePrefab, pile.transform);
            pancake.transform.localPosition = (i + 1) * Vector3.up * 0.1f + Random.Range(-0.1f, 0.1f) * Vector3.right + Random.Range(-0.1f, 0.1f) * Vector3.forward;
        }
        GameObject topPancake = Instantiate(pancakeTopPrefab, pile.transform);
        topPancake.transform.localPosition = numberOfPancakesPerPile * Vector3.up * 0.1f;
        topPancake.transform.localEulerAngles = Vector3.up * possibleTopRotations[Random.Range(0, possibleTopRotations.Length)];
        if (!_init)
            ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, pile.transform.position, Quaternion.identity, true, true);
    }

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this, minigameVersion);
        checkRuneObjective = CheckRuneObjectiveForFood;

        for (int i = 0; i < playerReferences.Count; i++)
        {
            Destroy(playerReferences[i].GetComponent<PlayerControllerHub>());
            PlayerController pc = playerReferences[i].AddComponent<PlayerController>();
            pc.playerIndex = (PlayerIndex)i;
            pc.PlayerIndexSet = true;

            playerReferences[i].GetComponent<Rigidbody>().useGravity = false;
            playerReferences[i].GetComponent<Rigidbody>().isKinematic = true;
            playerReferences[i].GetComponent<Player>().NbPoints = 0;

            playerReferences[i].GetComponentInChildren<Animator>().runtimeAnimatorController = specificAnimator;
            foreach (GameObject fork in playerReferences[i].GetComponentInChildren<Fork>().forkReferences)
                fork.SetActive(true);
        }


        LaunchTimer();

        if (GameManager.Instance.PlayerStart.DEBUG_SkipMinigamesRuleScreen)
        {
            minigameUI.SetActive(true);
            // Food meter init
            FoodMeterHandler = foodMeterUI.GetComponent<FoodMeterHandler>();

            inputTracksHandler.StartGame();
        }


        platesLocation = new GameObject("Empty Plates");
        pileOfPancakesLocation = new GameObject("Piles of Pancakes");
        for (int i = 0; i < playerReferences.Count; i++)
        {
            GameObject plates = new GameObject("Plates" + i);
            plates.transform.SetParent(platesLocation.transform);
            plates.transform.position = startingPositions[i] + offsetPlates;

            GameObject pile = new GameObject("Pile" + i);
            pile.transform.SetParent(pileOfPancakesLocation.transform);
            pile.transform.position = startingPositions[i] + offsetPancakes;
            CreateAPileOfPancakes(i, true);
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
            miniTail = 1.2f;
            maxTail = 2.0f;
            inputSpeed = 50.0f;
            _minigameVersion -= 1;
        }
        else
        {
            miniTail = 2.0f;
            maxTail = 3.0f;
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
        Transform pileOfPancake = pileOfPancakesLocation.transform.GetChild((int)_controller.PlayerIndex).GetChild(0);
        currentPlayer.GetComponentInChildren<Animator>().SetTrigger("eat");

        //if (eatPancakeOnNextInput[(int)_controller.PlayerIndex])
        //{
        DestroyImmediate(pileOfPancake.GetChild(pileOfPancake.childCount - 1).gameObject);
        //}
        //eatPancakeOnNextInput[(int)_controller.PlayerIndex] = !eatPancakeOnNextInput[(int)_controller.PlayerIndex];

        //if (platesLocation.GetChild((int)_controller.PlayerIndex).childCount < currentPlayer.NbPoints / 150)
        //{
        if (pileOfPancake.childCount == 1)
        {
            GameObject plate = pileOfPancake.GetChild(0).gameObject;
            plate.transform.SetParent(platesLocation.transform.GetChild((int)_controller.PlayerIndex));
            plate.transform.localPosition = platesLocation.transform.GetChild((int)_controller.PlayerIndex).childCount * Vector3.up * 0.1f
                + Random.Range(-1.0f, 1.0f) * Vector3.right + Random.Range(-1.0f, 1.0f) * Vector3.forward;
            plate.transform.localEulerAngles = Vector3.up * Random.Range(0, 360);

            plate.AddComponent<BoxCollider>().size *= 0.75f;
            plate.AddComponent<Rigidbody>().useGravity = true;

            // Remove food on plate
            for (int i = 0; i < plate.transform.childCount; i++)
                Destroy(plate.transform.GetChild(i).gameObject);

            Destroy(pileOfPancake.gameObject);
            CreateAPileOfPancakes((int)_controller.PlayerIndex);
        }

        FoodMeterHandler.FoodMeterIncrease((int)_controller.PlayerIndex);

    }

    public void BadInput(PlayerControllerFood _controller)
    {
        //score--
        Player currentPlayer = GameManager.Instance.PlayerStart.PlayersReference[(int)_controller.PlayerIndex].GetComponent<Player>();
        currentPlayer.UpdateCollectableValue(CollectableType.Points, -100);
    }

    public override void EndMinigame()
    {
        minigameUI.SetActive(false);
        base.EndMinigame();
    }

    bool CheckRuneObjectiveForFood()
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
