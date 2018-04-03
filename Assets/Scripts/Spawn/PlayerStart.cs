using UWPAndXInput;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerStart : MonoBehaviour {

    Transform[] playerStart;
    public GameObject playerPrefab;
    public GameObject playerRabbitPrefab;

    [SerializeField] GameObject PlayerUI;
    [SerializeField] GameMode gameMode;

    public GameObject[] cameraPlayerReferences;
    uint activePlayersAtStart = 0;
    public bool DEBUG_playXPlayers = true;
    [Range(1,4)]
    public uint DEBUG_NbPlayers = 1;
    public bool DEBUG_SkipMinigamesRuleScreen = false;

    List<GameObject> playersReference = new List<GameObject>();
    [SerializeField]
    public Color[] colorPlayer;



    public List<GameObject> PlayersReference
    {
        get
        {
            return playersReference;
        }
    }

    public uint ActivePlayersAtStart
    {
        get
        {
            return activePlayersAtStart;
        }
    }

    void Start()
    {
        playerStart = new Transform[4];
        for (int i = 0; i < transform.childCount; i++)
            playerStart[i] = transform.GetChild(i);
        GameManager.Instance.RegisterPlayerStart(this);
        SpawnPlayers();
        Debug.Assert(gameMode != null, "Missing gameMode");
        gameMode.AttributeCamera(activePlayersAtStart, cameraPlayerReferences, playersReference);

        // Inits
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            InitializeScorePanel();

        }

        if( !GameManager.Instance.IsInHub() && GameManager.Instance.SpecificPlayerUI != null)
            GameManager.Instance.SpecificPlayerUI.Init();
        gameMode.StartGame(playersReference);
        gameMode.OpenRuleScreen();

        // Disable debug behaviour in build
#if UNITY_EDITOR
        if (DEBUG_SkipMinigamesRuleScreen)
        {
            GameManager.UiReference.RuleScreen.GetComponent<RuleScreenHandler>().CleanUpAndStart();
            GameManager.ChangeState(GameState.Normal);
        }
    #endif

        // Pour chaque joueur
        //for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        //{
        //    // Pour chaque canvas
        //    for (int j = 0; j < GameManager.Instance.PlayerStart.PlayersReference.Count; j++)
        //    {
        //        if (j != i)
        //        {
        //            GameManager.Instance.PlayerStart.PlayersReference[i].transform.GetChild((int)PlayerChildren.Canvas).GetChild(j).gameObject.GetComponentInChildren<Text>().text = GameManager.Instance.PlayerStart.PlayersReference[i].name;
        //            GameManager.Instance.PlayerStart.PlayersReference[i].transform.GetChild((int)PlayerChildren.Canvas).GetChild(j).gameObject.SetActive(true);
        //        }
        //    }
        //}

#if UNITY_EDITOR
        ResourceUtils.Instance.debugTools.ActivateDebugMode(true);
#endif

        if (AudioManager.Instance != null)
            AudioManager.Instance.Init();
    }

    public Transform GetPlayerStart(uint playerIndex)
    {
        return playerStart[playerIndex];
    }

    public void SpawnPlayers()
    {
        if (GameManager.Instance.DataContainer == null)
        {
            GameManager.Instance.RegisterDataContainer(FindObjectOfType<SlimeDataContainer>());
        }

        if (GameManager.Instance.DataContainer != null && GameManager.Instance.DataContainer.launchedFromMinigameScreen)
            DEBUG_playXPlayers = false;

        if (DEBUG_playXPlayers)
            activePlayersAtStart = DEBUG_NbPlayers;
        else
        {
            if (GameManager.Instance.DataContainer != null)
                activePlayersAtStart = (uint)Mathf.Max(1, GameManager.Instance.DataContainer.nbPlayers);
            else
                activePlayersAtStart = 1;
        }

        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go;
            if (GameManager.Instance.DataContainer != null && GameManager.Instance.DataContainer.rabbitSelected[i])
            {
                go = Instantiate(playerRabbitPrefab);
            }
            else
                go = Instantiate(playerPrefab);

            if (GameManager.Instance.IsInHub())
            {
                if (GameManager.Instance.savedPositionInHub != Vector3.zero)
                {
                    go.transform.position = GameManager.Instance.savedPositionInHub + Vector3.right * i;
                    go.transform.rotation = GameManager.Instance.savedRotationInHub;
                    Player currentPlayer = go.GetComponent<Player>();
                    currentPlayer.respawnPoint = playerStart[i];
                }

                go.AddComponent<WindBlowingSound>();
            }
            else
            {
                Transform playerSpawn = playerStart[i];
                go.transform.position = playerSpawn.position;
                go.transform.rotation = playerSpawn.rotation;
                Player currentPlayer = go.GetComponent<Player>();
                currentPlayer.respawnPoint = playerSpawn;
            }
            PlayerController playerController = go.GetComponent<PlayerController>();
            
            playerController.PlayerIndex = (PlayerIndex)i;
            playerController.PlayerIndexSet = true;

            if (GameManager.Instance.DataContainer != null)
            {
                if (go.transform.GetComponentInChildren<PlayerCosmetics>() != null)
                {
                    if (GameManager.Instance.DataContainer.colorFadeSelected[i])
                        go.transform.GetComponentInChildren<PlayerCosmetics>().UseColorFade = true;
                    else
                        go.transform.GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(GameManager.Instance.DataContainer.selectedColors[i]);
                    go.transform.GetComponentInChildren<PlayerCosmetics>().FaceType = (FaceType)GameManager.Instance.DataContainer.selectedFaces[i];
                }

                if (go.transform.GetComponentInChildren<CustomizableSockets>() != null)
                {
                    Transform customizableParent = go.transform.GetComponentInChildren<CustomizableSockets>().transform;

                    // Init mustaches //
                    InitCustomizable(CustomizableType.Mustache, GameManager.Instance.DataContainer.mustachesSelected[i], customizableParent);

                    // Init hats //
                    InitCustomizable(CustomizableType.Hat, GameManager.Instance.DataContainer.hatsSelected[i], customizableParent);

                }
            }
            else
            {
                if (i > 0)
                {
                    go.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", colorPlayer[i - 1]);
                    go.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", colorPlayer[i - 1]);
                    go.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", colorPlayer[i - 1]);

                    go.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", colorPlayer[i - 1]);
                    go.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", colorPlayer[i - 1]);
                }
            }

            PlayersReference.Add(go);
           
        }

        // Ugly shit, should be handled in gamemode @Anthony
        if (SceneManager.GetActiveScene().name == "MinigameAntho")
            ColorFloorHandler.Init(activePlayersAtStart);


    }

    void InitCustomizable(CustomizableType _type, string _value, Transform _customizableParent)
    {
        if (_value == "None")
            return;
     
        GameObject customizable = Instantiate(Resources.Load(_value), _customizableParent.GetChild((int)_type - 2).transform) as GameObject;
        customizable.GetComponent<ICustomizable>().Init(_customizableParent.GetComponentInParent<Rigidbody>());
        
    }

    public void AttributeCamera()
    {
        if (cameraPlayerReferences.Length == 0)
        {
            Debug.LogError("No camera assigned in playerStart");
            return;
        }

        // By default, cameraP2 is set for 2-Player mode, so we only update cameraP1
        if (activePlayersAtStart == 2)
        {
            cameraPlayerReferences[0].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1.0f);
        }
        // By default, cameraP3 and cameraP4 are set for 4-Player mode, so we only update cameraP1 and cameraP2
        else if (activePlayersAtStart > 2)
        {
            cameraPlayerReferences[0].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            cameraPlayerReferences[1].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        }

        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = PlayersReference[i];
            if (SceneManager.GetActiveScene().name == "MinigamePush")
            {
                //cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = go.transform;
            }
            else
            {
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform.GetChild((int)PlayerChildren.CameraTarget);
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().playerIndex = (PlayerIndex)i;
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().associatedPlayerController = go.GetComponent<PlayerControllerHub>();
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().associatedPlayerCharacter = go.GetComponent<PlayerCharacterHub>();
            }

            go.GetComponent<Player>().cameraReference = cameraPlayerReferences[i];
            cameraPlayerReferences[i].SetActive(true);
        }
    }

    void InitializeScorePanel()
    {
        if(GameManager.Instance.ScoreScreenReference != null)
            GameManager.Instance.ScoreScreenReference.Init();
    }
}
