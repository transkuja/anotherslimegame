using XInputDotNetPure;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

public class PlayerStart : MonoBehaviour {

    Transform[] playerStart;
    public GameObject playerPrefab;
    public GameObject[] cameraPlayerReferences;
    uint activePlayersAtStart = 0;

    List<GameObject> playersReference = new List<GameObject>();

    public bool isPlayerReady = false;

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

    void Start () {
        playerStart = new Transform[4];
        for (int i = 0; i < transform.childCount; i++)
            playerStart[i] = transform.GetChild(i);
        GameManager.Instance.RegisterPlayerStart(this);
        SpawnPlayers();
        AttributeCamera();

        // Inits
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            InitializeScorePanel();
            InitializePlayersUI();
        }

        isPlayerReady = true;

}

public Transform GetPlayerStart(uint playerIndex)
    {
        return playerStart[playerIndex];
    }


    void CheckNumberOfActivePlayers()
    {
        for (int i = 0; i < 4; ++i)
        {
            PlayerIndex testPlayerIndex = (PlayerIndex)i;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected)
            {
                Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                activePlayersAtStart++;
            }
        }

        // There should always be one player
        if (activePlayersAtStart == 0) activePlayersAtStart++;
    }

    public void SpawnPlayers()
    {
        CheckNumberOfActivePlayers();
        
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = Instantiate(playerPrefab);
            Transform playerSpawn = playerStart[i];
            go.transform.position = playerSpawn.position;
            go.transform.rotation = playerSpawn.rotation;
            Player currentPlayer = go.GetComponent<Player>();
            currentPlayer.respawnPoint = playerSpawn;

            PlayerController playerController = go.GetComponent<PlayerController>();

            playerController.PlayerIndex = (PlayerIndex)i;
            playerController.IsUsingAController = true;
            playerController.PlayerIndexSet = true;

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
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            {
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = go.transform;
            }
            else
            {
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform;
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
                cameraPlayerReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().playerIndex = (PlayerIndex)i;
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
