using UWPAndXInput;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerStart : MonoBehaviour {

    Transform[] playerStart;
    public GameObject playerPrefab;

    [SerializeField] GameMode gameMode;

    public GameObject[] cameraPlayerReferences;
    uint activePlayersAtStart = 0;
    public bool DEBUG_playXPlayers = true;
    [Range(1,4)]
    public uint DEBUG_NbPlayers = 1;
    public bool DEBUG_SkipMinigamesRuleScreen = false;

    List<GameObject> playersReference = new List<GameObject>();
    [SerializeField]
    private Color[] colorPlayer;



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

    public Color[] ColorPlayer
    {
        get
        {
            if (colorPlayer == null)
            {
                if (SlimeDataContainer.instance != null)
                {
                    colorPlayer = SlimeDataContainer.instance.playerColorsUI;
                }
                else
                {
                    colorPlayer = new Color[4];
                    colorPlayer[0] = Color.red;
                    colorPlayer[1] = Color.blue;
                    colorPlayer[2] = Color.green;
                    colorPlayer[3] = Color.yellow;

                }
            }
                
            return colorPlayer;
        }

        set
        {
            colorPlayer = value;
        }
    }

    void Start()
    {
        colorPlayer = null;
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

        if (!GameManager.Instance.IsInHub())
        {
            if (GameManager.Instance.SpecificPlayerUI != null)
                GameManager.Instance.SpecificPlayerUI.Init();
        }
        else
        {
            DatabaseManager.Db.ResetBreakablesState();
            DatabaseManager.Db.ResetCollectablesState();
            DatabaseManager.Db.ResetLockState();
        }

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

        Transform feedbacksCooldown = ResourceUtils.Instance.feedbackCooldown.transform;

        if (GameManager.Instance.IsInHub() || GameManager.Instance.CurrentGameMode is Runner3DGameMode)
        {
            for (int i = 0; i < 4; i++)
            {
                feedbacksCooldown.GetChild(i).gameObject.SetActive(i == SlimeDataContainer.instance.nbPlayers - 1);
            }
            for (int i = 0; i < feedbacksCooldown.GetChild(SlimeDataContainer.instance.nbPlayers - 1).childCount; i++)
            {
                if (GameManager.Instance.IsInHub())
                {
                    feedbacksCooldown.GetChild(SlimeDataContainer.instance.nbPlayers - 1).GetChild(i).GetChild(0).gameObject.SetActive(false); // deactivate alpha
                    Image feedbackImg = feedbacksCooldown.GetChild(SlimeDataContainer.instance.nbPlayers - 1).GetChild(i).GetChild(1).GetComponent<Image>();
                    feedbackImg.fillMethod = Image.FillMethod.Radial360;
                    feedbackImg.fillOrigin = 2;
                }
                else if (GameManager.Instance.CurrentGameMode is Runner3DGameMode && GameManager.Instance.CurrentGameMode.minigameVersion == 1)
                {
                    feedbacksCooldown.GetChild(SlimeDataContainer.instance.nbPlayers - 1).GetChild(i).GetChild(0).gameObject.SetActive(true); // activate alpha
                    Image feedbackImg = feedbacksCooldown.GetChild(SlimeDataContainer.instance.nbPlayers - 1).GetChild(i).GetChild(1).GetComponent<Image>();
                    feedbackImg.fillMethod = Image.FillMethod.Vertical;
                    feedbackImg.fillOrigin = 0;
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
                feedbacksCooldown.GetChild(i).gameObject.SetActive(false);
        }
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
            GameObject go = Instantiate(playerPrefab);

            if (GameManager.Instance.IsInHub())
            {
                if (GameManager.Instance.savedPositionInHub != Vector3.zero)
                {
                    go.transform.position = GameManager.Instance.savedPositionInHub + Vector3.right * i;
                    go.transform.rotation = GameManager.Instance.savedRotationInHub;
                    Player currentPlayer = go.GetComponent<Player>();
                    currentPlayer.respawnPoint = playerStart[i];
                }
                else
                {
                    Transform playerSpawn = playerStart[i];
                    if ( DatabaseManager.Db.NbRunes > 1)
                    {
                        if (i == 0)
                            playerSpawn = playerStart[2];
                        else if( i == 1 )
                            playerSpawn = playerStart[3];
                    } 
                    go.transform.position = playerSpawn.position;
                    go.transform.rotation = playerSpawn.rotation;
                    Player currentPlayer = go.GetComponent<Player>();
                    currentPlayer.respawnPoint = playerSpawn;
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
            PlayerCosmetics playerCosmetics = go.GetComponentInChildren<PlayerCosmetics>();
            if (!playerCosmetics)
            {
                Debug.LogError("There is no Player Cosmetics component on this object or in children");
            }
            else
            {
                if (GameManager.Instance.DataContainer != null)
                {
                    if (GameManager.Instance.DataContainer.colorFadeSelected[i])
                        playerCosmetics.ColorFadeType = ColorFadeType.Basic;
                    else
                        playerCosmetics.SetUniqueColor(GameManager.Instance.DataContainer.selectedColors[i]);
                    playerCosmetics.FaceType = GameManager.Instance.DataContainer.selectedFaces[i];

                    if (go.transform.GetComponentInChildren<CustomizableSockets>() != null)
                    {
                        Transform customizableParent = go.transform.GetComponentInChildren<CustomizableSockets>().transform;

                        // Init mustaches //
                        playerCosmetics.Mustache = GameManager.Instance.DataContainer.mustachesSelected[i];

                        // Init ears //
                        playerCosmetics.Ears = GameManager.Instance.DataContainer.earsSelected[i];

                        // Init hats //
                        playerCosmetics.Hat = GameManager.Instance.DataContainer.hatsSelected[i];

                        // Init forehead //
                        playerCosmetics.Forehead = GameManager.Instance.DataContainer.foreheadsSelected[i];

                        // Init skin //
                        playerCosmetics.Skin = GameManager.Instance.DataContainer.skinsSelected[i];                        
                        
                        // Init chin //
                        playerCosmetics.Chin = GameManager.Instance.DataContainer.chinsSelected[i];

                        // Init accessory //
                        playerCosmetics.Accessory = GameManager.Instance.DataContainer.accessoriesSelected[i];
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        foreach (Renderer r in go.transform.GetChild(0).GetComponentsInChildren<Renderer>())
                            r.material.SetColor("_Color", ColorPlayer[i - 1]);
                    }
                }
            }
            PlayersReference.Add(go);
           
        }
    }

    void InitializeScorePanel()
    {
        if(GameManager.Instance.ScoreScreenReference != null)
            GameManager.Instance.ScoreScreenReference.Init();
    }
}
