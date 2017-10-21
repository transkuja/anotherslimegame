using XInputDotNetPure;

using UnityEngine;

public class PlayerStart : MonoBehaviour {

    Transform[] playerStart;
    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    uint activePlayersAtStart = 0;

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

        //cameraSceneReference.GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform;
        //cameraSceneReference.GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
        //currentPlayer.cameraReference = cameraSceneReference;
        //cameraSceneReference.SetActive(true);

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
            cameraPrefab.GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform;
            cameraPrefab.GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
            currentPlayer.cameraReference = cameraPrefab;
            cameraPrefab.SetActive(true);
            PlayerController playerController = go.GetComponent<PlayerController>();

            playerController.PlayerIndex = 0;
            playerController.IsUsingAController = false;
            playerController.PlayerIndexSet = true;

            GameManager.Instance.PlayersReference.Add(go);
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
            cameraPrefab.GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform;
            cameraPrefab.GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
            currentPlayer.cameraReference = cameraPrefab;
            cameraPrefab.SetActive(true);
            PlayerController playerController = go.GetComponent<PlayerController>();

            playerController.PlayerIndex = (PlayerIndex)i;
            playerController.IsUsingAController = true;
            playerController.PlayerIndexSet = true;

            GameManager.Instance.PlayersReference.Add(go);

        }
    }

}
