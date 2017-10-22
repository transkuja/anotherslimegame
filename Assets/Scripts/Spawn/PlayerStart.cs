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
        InitializeScorePanel();
        InitializePlayersUI();
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
        Debug.Log("Active players at start: " + activePlayersAtStart);
    }

    public void SpawnPlayers()
    {
        CheckNumberOfActivePlayers();
        // ===================================================
        // Debug 
        if (activePlayersAtStart == 0)
        {
            GameObject go = Instantiate(playerPrefab);
            Transform playerSpawn = playerStart[0];
            go.transform.position = playerSpawn.position;
            go.transform.rotation = playerSpawn.rotation;
            Player currentPlayer = go.GetComponent<Player>();
            currentPlayer.respawnPoint = playerSpawn;
            PlayerController playerController = go.GetComponent<PlayerController>();

            playerController.PlayerIndex = 0;
            playerController.IsUsingAController = false;
            playerController.PlayerIndexSet = true;

            PlayersReference.Add(go);
        }
        // ==========================================================

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

        if (activePlayersAtStart == 0)
        {
            GameObject go = PlayersReference[0];
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            {
                cameraPlayerReferences[0].GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = go.transform;
            } else
            {
                cameraPlayerReferences[0].GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform;
                cameraPlayerReferences[0].GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
            } 
            go.GetComponent<Player>().cameraReference = cameraPlayerReferences[0];
            cameraPlayerReferences[0].SetActive(true);
        }

        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = PlayersReference[i];
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            {
                cameraPlayerReferences[i].GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = go.transform;
            }
            else
            {
                cameraPlayerReferences[i].GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform;
                cameraPlayerReferences[i].GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
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
