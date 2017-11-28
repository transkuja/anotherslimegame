﻿using UWPAndXInput;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerStart : MonoBehaviour {

    Transform[] playerStart;
    public GameObject playerPrefab;
    public GameObject[] cameraPlayerReferences;
    uint activePlayersAtStart = 0;

    List<GameObject> playersReference = new List<GameObject>();
    Color[] colorPlayer;
    public float timeSinceStageIsSet = 0.0f;

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
        colorPlayer = new Color[4];
        colorPlayer[0] = Color.red;
        colorPlayer[1] = Color.blue;
        colorPlayer[2] = Color.green;
        colorPlayer[3] = Color.cyan;
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

            timeSinceStageIsSet = 0.0f;
        }
        int j = 0;
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {

            if( j == i)
            {
                GameManager.Instance.PlayerStart.PlayersReference[i].transform.GetChild(3).GetChild(j).gameObject.SetActive(false);
                j++;

            } else
            {
                GameManager.Instance.PlayerStart.PlayersReference[i].transform.GetChild(3).GetChild(i).gameObject.GetComponentInChildren<Text>().text = GameManager.Instance.PlayerStart.PlayersReference[i].name;
            }

        }
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


            //go.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i]);
            //go.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i]);
            //go.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i]);

            //go.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i]);
            //go.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", colorPlayer[i]);

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

    public void FixedUpdate()
    {
        timeSinceStageIsSet += Time.fixedDeltaTime;
        // TODO rethink this.
        if (timeSinceStageIsSet > 1000000) timeSinceStageIsSet = 0.0f;
    }
}
