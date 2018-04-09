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

                    // Init ears //
                    InitCustomizable(CustomizableType.Ears, GameManager.Instance.DataContainer.earsSelected[i], customizableParent);

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
    }

    void InitCustomizable(CustomizableType _type, string _value, Transform _customizableParent)
    {
        if (_value == "None" || _value == "")
            return;
     
        GameObject customizable = Instantiate(Resources.Load(_value), _customizableParent.GetChild((int)_type - 2).transform) as GameObject;
        customizable.GetComponent<ICustomizable>().Init(_customizableParent.GetComponentInParent<Rigidbody>());

        // Hide ears if the hat is supposed to hide them
        if (_type == CustomizableType.Hat)
        {
            DatabaseClass.HatData hat = (DatabaseClass.HatData)DatabaseManager.Db.GetDataFromModel<DatabaseClass.HatData>(_value);
            // BUG : _value = Hats/CowboyHat au lieu de Cowboy
            if (hat != null && hat.shouldHideEars)
            {
                _customizableParent.GetChild((int)CustomizableType.Ears - 2).gameObject.SetActive(false);
            }
        }
        
    }

    void InitializeScorePanel()
    {
        if(GameManager.Instance.ScoreScreenReference != null)
            GameManager.Instance.ScoreScreenReference.Init();
    }
}
