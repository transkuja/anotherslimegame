using UWPAndXInput;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerStart : MonoBehaviour {

    Transform[] playerStart;
    public GameObject playerPrefab;
    [SerializeField] GameObject PlayerUI;
    [SerializeField] GameMode gameMode;

    public GameObject[] cameraPlayerReferences;
    uint activePlayersAtStart = 0;
    public bool DEBUG_playXPlayers = true;
    public uint DEBUG_NbPlayers = 1;

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
            InitializePlayersUI();
        }
        gameMode.StartGame(playersReference);

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
    }

    public Transform GetPlayerStart(uint playerIndex)
    {
        return playerStart[playerIndex];
    }


    void CheckNumberOfActivePlayers()
    {
#if UNITY_EDITOR
        if (DEBUG_playXPlayers)
        {
            activePlayersAtStart = DEBUG_NbPlayers;
            return;
        }
#endif
        for (int i = 0; i < 4; ++i)
        {
            PlayerIndex testPlayerIndex = (PlayerIndex)i;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected)
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
#endif
                activePlayersAtStart++;
            }
        }

        // There should always be one player
        if (activePlayersAtStart == 0) activePlayersAtStart++;
    }

    public void SpawnPlayers()
    {
        CheckNumberOfActivePlayers();

        if (GameManager.Instance.DataContainer == null)
        {
            GameManager.Instance.RegisterDataContainer(FindObjectOfType<SlimeDataContainer>());
        }

        if (GameManager.Instance.DataContainer != null)
        {
            activePlayersAtStart = (uint)GameManager.Instance.DataContainer.nbPlayers;
        }

        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = Instantiate(playerPrefab);
            Transform playerSpawn = playerStart[i];
            go.transform.position = playerSpawn.position;
            go.transform.rotation = playerSpawn.rotation;
            Player currentPlayer = go.GetComponent<Player>();
            currentPlayer.respawnPoint = playerSpawn;

            PlayerControllerHub playerController = go.GetComponent<PlayerControllerHub>();
            
            playerController.PlayerIndex = (PlayerIndex)i;
            playerController.IsUsingAController = true;
            playerController.PlayerIndexSet = true;

            
            if (GameManager.Instance.DataContainer != null)
            {
                if (GameManager.Instance.DataContainer.colorFadeSelected[i])
                    go.transform.GetComponentInChildren<PlayerCosmetics>().gameObject.AddComponent<ColorFade>();
                else
                    go.transform.GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(GameManager.Instance.DataContainer.selectedColors[i]);
                go.transform.GetComponentInChildren<PlayerCosmetics>().FaceType = (FaceType)GameManager.Instance.DataContainer.selectedFaces[i];
            }
            else
            {
                if (i > 0)
                {
                    go.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i - 1]);
                    go.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i - 1]);
                    go.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i - 1]);

                    go.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i - 1]);
                    go.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i - 1]);
                }
            }

            PlayersReference.Add(go);
           
        }

       
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
            if (SceneManager.GetActiveScene().name == "SceneMinigamePush")
            {
                //cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = go.transform;
            }
            else
            {
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform.GetChild((int)PlayerChildren.CameraTarget);
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().playerIndex = (PlayerIndex)i;
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().associatedPlayerController = go.GetComponent<PlayerControllerHub>();
            }

            go.GetComponent<Player>().cameraReference = cameraPlayerReferences[i];
            cameraPlayerReferences[i].SetActive(true);
        }
    }

    void InitializeScorePanel()
    {
        GameManager.Instance.ScoreScreenReference.Init();
    }

    void InitializePlayersUI()
    {
        GameManager.Instance.PlayerUI.Init();
    }
}
