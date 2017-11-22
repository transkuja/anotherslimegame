using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class DebugTools : MonoBehaviour {

    private static bool isDebugModeActive = false;

    [SerializeField]
    Transform debugPanel;

    private static Player debugPlayerSelected;

    private Player debugSpawnedPlayer;
    bool possessASpawnedPlayer = false;

    public static Player DebugPlayerSelected
    {
        get
        {
            // TODO: very ugly handling, should be refactored when multiplayer are handled (references in GameManager?)
            if (debugPlayerSelected == null)
            {
                debugPlayerSelected = GameObject.FindObjectOfType<Player>();
            }
            return debugPlayerSelected;
        }
    }

    private void Start()
    {
        if (debugPanel == null)
        {
            Debug.LogWarning("DebugPanel is not linked on DebugTools, autolink with Find ...");
            debugPanel = GameObject.Find("DebugPanel").transform;
        }

        if (debugPlayerSelected == null)
        {
            debugPlayerSelected = GameObject.FindObjectOfType<Player>();
        }
    }

    void Update () {
        // Activation/Deactivation debug mode
        if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKeyDown(KeyCode.RightShift))
        {
            isDebugModeActive = !isDebugModeActive;
            debugPanel.gameObject.SetActive(isDebugModeActive);
            if (isDebugModeActive)
                Debug.Log("DEBUG MODE ACTIVATED!");
            else
                Debug.Log("DEBUG MODE DEACTIVATED!");
        }

        if (isDebugModeActive)
        {
            // TODO Antho: Handle evolution mode switch
            // Add evolution to current player
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (DebugPlayerSelected.GetComponent<EvolutionStrength>() == null)
                        GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Strength));
                    Debug.Log("Added Strength on player " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    if (DebugPlayerSelected.GetComponent<EvolutionAgile>() == null)
                        GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Agile));
                    Debug.Log("Added Agile on player " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    if (DebugPlayerSelected.GetComponent<EvolutionPlatformist>() == null)
                        GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Platformist));
                    Debug.Log("Added Platformist on player " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    // Should be ghost
                    if (DebugPlayerSelected.GetComponent<EvolutionPlatformist>() == null)
                        GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Ghost));
                    Debug.Log("Added Ghost on player (dev needed here) " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }
                // Debug platformist
                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    if (DebugPlayerSelected.GetComponent<EvolutionPlatformist>() == null)
                        GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Platformist));

                    EvolutionPlatformist evolution = DebugPlayerSelected.GetComponent<EvolutionPlatformist>();
                    evolution.CooldownCharge = 1.0f;
                    evolution.PlatformLifetime = 300.0f;

                    Debug.Log("Added Platformist on player " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }
            }

            // Spawn a collectable
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.Points).GetComponent<Collectable>().Init(0);
                    Debug.Log("Pop some " + CollectableType.StrengthEvolution1 + " on the ground!");
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.Key).GetComponent<Collectable>().Init(0);
                    Debug.Log("Pop some " + CollectableType.Points + " on the ground!");
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.StrengthEvolution1).GetComponent<Collectable>().Init(0);
                    Debug.Log("Pop some " + CollectableType.Key + " on the ground!");
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.PlatformistEvolution1).GetComponent<Collectable>().Init(0);
                    Debug.Log("Pop some " + CollectableType.PlatformistEvolution1 + " on the ground!");
                }
                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.AgileEvolution1).GetComponent<Collectable>().Init(0);
                    Debug.Log("Pop some " + CollectableType.AgileEvolution1 + " on the ground!");
                }
                if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.GhostEvolution1).GetComponent<Collectable>().Init(0);
                    Debug.Log("Pop some " + CollectableType.GhostEvolution1 + " on the ground!");
                }

            }
            else
            {
                // Reset player
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    DebugPlayerSelected.Collectables = new int[(int)CollectableType.Size];
                    if (DebugPlayerSelected.GetComponent<EvolutionStrength>()) Destroy(DebugPlayerSelected.GetComponent<EvolutionStrength>());
                    if (DebugPlayerSelected.GetComponent<EvolutionAgile>()) Destroy(DebugPlayerSelected.GetComponent<EvolutionAgile>());
                    if (DebugPlayerSelected.GetComponent<EvolutionPlatformist>()) Destroy(DebugPlayerSelected.GetComponent<EvolutionPlatformist>());
                       // TODO: add ghost evolution
                    DebugPlayerSelected.Rb.velocity = Vector3.zero;
                    Debug.Log("Reset current player! " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }

                // Respawn player
                if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    Respawner.RespawnProcess(DebugPlayerSelected);
                    DebugPlayerSelected.Rb.velocity = Vector3.zero;
                    Debug.Log("Reset current player to last respawn point! " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }

                // Pop player
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GameObject go = Instantiate(GameManager.Instance.PlayerStart.playerPrefab);
                    go.transform.position = DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f;
                    go.transform.rotation = Quaternion.identity;
                    Player currentPlayer = go.GetComponent<Player>();
                    currentPlayer.respawnPoint = DebugPlayerSelected.respawnPoint;

                    PlayerController playerController = go.GetComponent<PlayerController>();
                    playerController.DEBUG_hasBeenSpawnedFromTool = true;
                    playerController.PlayerIndex = (PlayerIndex)5;
                    playerController.IsUsingAController = true;
                    playerController.PlayerIndexSet = true;

                    GameManager.Instance.PlayerStart.PlayersReference.Add(go);
                    debugSpawnedPlayer = go.GetComponent<Player>();

                    Debug.Log("Player spawned! " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    if (debugSpawnedPlayer == null)
                    {
                        Debug.Log("No spawned player to possess");
                        return;
                    }

                    if (!possessASpawnedPlayer)
                    {
                        PlayerController playerControllerOne = DebugPlayerSelected.GetComponent<PlayerController>();
                        playerControllerOne.DEBUG_hasBeenSpawnedFromTool = true;
                        playerControllerOne.PlayerIndex = (PlayerIndex)5;

                        GameObject tmpCamRef = DebugPlayerSelected.cameraReference;
                        DebugPlayerSelected.cameraReference = debugSpawnedPlayer.cameraReference;
                        debugSpawnedPlayer.cameraReference = tmpCamRef;

                        if (DebugPlayerSelected.cameraReference != null)
                        {
                            DebugPlayerSelected.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = DebugPlayerSelected.transform;
                            DebugPlayerSelected.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = DebugPlayerSelected.transform;
                        }
                        if (debugSpawnedPlayer.cameraReference != null)
                        {
                            debugSpawnedPlayer.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = debugSpawnedPlayer.transform;
                            debugSpawnedPlayer.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = debugSpawnedPlayer.transform;
                        }

                        PlayerController playerControllerSpwn = debugSpawnedPlayer.GetComponent<PlayerController>();
                        playerControllerSpwn.DEBUG_hasBeenSpawnedFromTool = false;
                        playerControllerSpwn.PlayerIndex = (PlayerIndex)0;

                        Debug.Log("Possess player one! ");
                    }
                    else
                    {
                        PlayerController playerControllerSpwn = debugSpawnedPlayer.GetComponent<PlayerController>();
                        playerControllerSpwn.DEBUG_hasBeenSpawnedFromTool = true; 
                        playerControllerSpwn.PlayerIndex = (PlayerIndex)5;

                        GameObject tmpCamRef = DebugPlayerSelected.cameraReference;
                        DebugPlayerSelected.cameraReference = debugSpawnedPlayer.cameraReference;
                        debugSpawnedPlayer.cameraReference = tmpCamRef;

                        if (DebugPlayerSelected.cameraReference != null)
                        {
                            DebugPlayerSelected.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = DebugPlayerSelected.transform;
                            DebugPlayerSelected.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = DebugPlayerSelected.transform;
                        }
                        if (debugSpawnedPlayer.cameraReference != null)
                        {
                            debugSpawnedPlayer.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = debugSpawnedPlayer.transform;
                            debugSpawnedPlayer.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = debugSpawnedPlayer.transform;
                        }

                        PlayerController playerControllerOne = DebugPlayerSelected.GetComponent<PlayerController>();
                        playerControllerOne.DEBUG_hasBeenSpawnedFromTool = false;
                        playerControllerOne.PlayerIndex = (PlayerIndex)0;

                        Debug.Log("Possess a spawned player! ");
                    }

                    possessASpawnedPlayer = !possessASpawnedPlayer;

                }

                // Reload all powers
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    if (DebugPlayerSelected.GetComponent<EvolutionPlatformist>())
                        DebugPlayerSelected.GetComponent<EvolutionPlatformist>().Charges = 3;

                    Debug.Log("Powers reloaded for player " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }

            }
        }
        
    }
}
