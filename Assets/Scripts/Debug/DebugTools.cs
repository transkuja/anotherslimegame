using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UWPAndXInput;
using System;

/*
 * DebugTools V3.0 
 *  - DebugTools now has a state machine for inputs. 
 *  - Press F1, F2 or F3 to switch controls.
 *  - Some controls can be used regardless of the current state.
 *  - Current state is shown in the right corner. Defaults are ChangeGameplayData for minigames and AddEvolution for Hub.
 *  
 * New ideas:
 *  - ReadySetGo skip?
 */
public class DebugTools : MonoBehaviour {
    public static bool isDebugModeActive = false;
    enum DebugState { AddEvolution, SpawnCollectable, GameplayData, Unlockables, Size }
    enum DebugUIState { None, FPS, Full, Size }

    public enum DebugPlayerInfos { GameState, Index, IsGrounded, GravityEnabled, CurrentState, HasBeenTp, NbJumpMade, CameraState, Size };
    public enum DebugEvolutionInfos { Charge, PatternIndex, Usable, Size };

    public static string[] debugPlayerInfos = new string[(int)DebugPlayerInfos.Size];
    public static string[] debugEvolutionInfos = new string[(int)DebugEvolutionInfos.Size];


    [SerializeField]
    Transform debugPanelReference;

    [SerializeField]
    GameObject debugPanelPrefab;

    [SerializeField]
    List<Transform> teleportPlayerPositionsList;
    int currentTeleportIndex = 0;

    [SerializeField]
    List<GameObject> torchLights;

    private static Player debugPlayerSelected;

    private Player debugSpawnedPlayer;
    bool possessASpawnedPlayer = false;
    bool hasUpdatedDebugPanel = false;
    bool lightsEnabled = false;

    DebugState currentState;
    DebugUIState currentUIState;

    List<float> lastFramesTime = new List<float>();
    public static float computedFPS = 0.0f;

    string[] helpPanels = new string[(int)DebugState.Size + 1];

    private void Start()
    {
        debugPlayerInfos[(int)DebugPlayerInfos.GravityEnabled] = "True";
        debugPlayerInfos[(int)DebugPlayerInfos.HasBeenTp] = "False";
        debugPlayerInfos[(int)DebugPlayerInfos.IsGrounded] = "True";
        debugPlayerInfos[(int)DebugPlayerInfos.NbJumpMade] = "0";
    }

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

    public Transform DebugPanelReference
    {
        get
        {
            if (debugPanelReference == null)
            {
                Debug.Log("No debug panel, instantiate debug UI ...");

                // UGLY
                Transform canvasParent;
                if (GameManager.UiReference == null)
                    canvasParent = GameObject.FindObjectOfType<UI>().transform;
                else
                    canvasParent = GameManager.UiReference.transform;

                debugPanelReference = GameObject.Instantiate(debugPanelPrefab, canvasParent).transform;
            }
            return debugPanelReference;
        }

        set
        {
            debugPanelReference = value;
        }
    }

    private DebugState CurrentState
    {
        set
        {
            currentState = value;
            DebugPanelReference.GetComponent<DebugPanel>().activationText.text = "Debug\n" + currentState.ToString();

            DebugPanelReference.GetComponent<DebugPanel>().UpdateDebugPanelInfos(helpPanels[(int)DebugState.Size], helpPanels[(int)currentState]);
        }
    }

    private DebugUIState CurrentUIState
    {
        get
        {
            return currentUIState;
        }

        set
        {
            currentUIState = value;
            DebugPanelReference.GetComponent<DebugPanel>().ChangeState((int)currentUIState);

        }
    }

    void UpdateDebugPanel()
    {
        DebugPanel debugPanelComponent = DebugPanelReference.GetComponent<DebugPanel>();

        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("", "", " Common controls\n");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("Ctrl", "RShift", "Activation/Deactivation");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("U", "I", "Show debug UI");

        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("F1", "", "AddEvolution mode");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("F2", "", "SpawnCollectable mode");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("F3", "", "ChangeData mode");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("F4", "", "Unlockables mode");

        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("0", "", "Reset player");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("9", "", "Respawn player");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("Space", "", "Spawn a player");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("P", "", "Possess a spawned player");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("LeftAlt", "", "Reload all powers");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("N", "", "Switch to next player debug info");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("T", "", "Teleport the player (transform must be specified)");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("L", "", "Enable torchlights on player");
        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("F", "", "Finish current minigame");

        helpPanels[(int)DebugState.Size] += debugPanelComponent.AddToDebugPanelInfos("", "", "\n  State-specific controls\n");

        helpPanels[(int)DebugState.AddEvolution] += debugPanelComponent.AddToDebugPanelInfos("1", "", "Strength");
        helpPanels[(int)DebugState.AddEvolution] += debugPanelComponent.AddToDebugPanelInfos("2", "", "Agile");
        helpPanels[(int)DebugState.AddEvolution] += debugPanelComponent.AddToDebugPanelInfos("3", "", "Platformist");
        helpPanels[(int)DebugState.AddEvolution] += debugPanelComponent.AddToDebugPanelInfos("4", "", "Ghost");
        helpPanels[(int)DebugState.AddEvolution] += debugPanelComponent.AddToDebugPanelInfos("5", "", "Platformist debug");

        helpPanels[(int)DebugState.SpawnCollectable] += debugPanelComponent.AddToDebugPanelInfos("1", "", CollectableType.Points.ToString());
        helpPanels[(int)DebugState.SpawnCollectable] += debugPanelComponent.AddToDebugPanelInfos("2", "", CollectableType.Rune.ToString());
        helpPanels[(int)DebugState.SpawnCollectable] += debugPanelComponent.AddToDebugPanelInfos("3", "", CollectableType.StrengthEvolution1.ToString());
        helpPanels[(int)DebugState.SpawnCollectable] += debugPanelComponent.AddToDebugPanelInfos("4", "", CollectableType.PlatformistEvolution1.ToString());
        helpPanels[(int)DebugState.SpawnCollectable] += debugPanelComponent.AddToDebugPanelInfos("5", "", CollectableType.AgileEvolution1.ToString());
        helpPanels[(int)DebugState.SpawnCollectable] += debugPanelComponent.AddToDebugPanelInfos("6", "", CollectableType.GhostEvolution1.ToString());
        helpPanels[(int)DebugState.SpawnCollectable] += debugPanelComponent.AddToDebugPanelInfos("7", "", CollectableType.Money.ToString());

        helpPanels[(int)DebugState.GameplayData] += debugPanelComponent.AddToDebugPanelInfos("LeftShift", "", "Change unit to 10");
        helpPanels[(int)DebugState.GameplayData] += debugPanelComponent.AddToDebugPanelInfos("1", "", "Add points");
        helpPanels[(int)DebugState.GameplayData] += debugPanelComponent.AddToDebugPanelInfos("2", "", "Remove points");
        helpPanels[(int)DebugState.GameplayData] += debugPanelComponent.AddToDebugPanelInfos("3", "", "Add life");
        helpPanels[(int)DebugState.GameplayData] += debugPanelComponent.AddToDebugPanelInfos("4", "", "Remove life");
        helpPanels[(int)DebugState.GameplayData] += debugPanelComponent.AddToDebugPanelInfos("5", "", "Increase timer");

        helpPanels[(int)DebugState.Unlockables] += debugPanelComponent.AddToDebugPanelInfos("A", "M", "Unlock/Lock All minigames");
        helpPanels[(int)DebugState.Unlockables] += debugPanelComponent.AddToDebugPanelInfos("A", "R", "Unlock/Lock All runes");
        helpPanels[(int)DebugState.Unlockables] += debugPanelComponent.AddToDebugPanelInfos("1", "", "Unlock next minigame");
        helpPanels[(int)DebugState.Unlockables] += debugPanelComponent.AddToDebugPanelInfos("2", "", "Lock last minigame unlocked");
        helpPanels[(int)DebugState.Unlockables] += debugPanelComponent.AddToDebugPanelInfos("3", "", "Unlock next rune");
        helpPanels[(int)DebugState.Unlockables] += debugPanelComponent.AddToDebugPanelInfos("4", "", "Lock last rune unlocked");

        debugPanelComponent.UpdateDebugPanelInfos(helpPanels[(int)DebugState.Size], helpPanels[(int)currentState]);
        hasUpdatedDebugPanel = true;
    }

    void AddEvolutionControls()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (DebugPlayerSelected.GetComponent<EvolutionStrength>() == null)
                GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Strength));
            Debug.Log("Added Strength on player " + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (DebugPlayerSelected.GetComponent<EvolutionAgile>() == null)
                GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Agile));
            Debug.Log("Added Agile on player " + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (DebugPlayerSelected.GetComponent<EvolutionPlatformist>() == null)
                GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Platformist));
            Debug.Log("Added Platformist on player " + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (DebugPlayerSelected.GetComponent<EvolutionGhost>() == null)
                GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Ghost));
            Debug.Log("Added Ghost on player" + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }
        // Debug platformist
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (DebugPlayerSelected.GetComponent<EvolutionPlatformist>() == null)
                GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Platformist));

            EvolutionPlatformist evolution = DebugPlayerSelected.GetComponent<EvolutionPlatformist>();
            evolution.CooldownCharge = 1.0f;
            evolution.PlatformLifetime = 300.0f;

            Debug.Log("Added Platformist on player " + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }
    }

    void SpawnCollectableControls()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DebugPlayerSelected.UpdateCollectableValue(CollectableType.Points, 10);
            Debug.Log("Adding " + CollectableType.Points + " to the player");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DebugPlayerSelected.UpdateCollectableValue(CollectableType.Rune, 1);
            Debug.Log("Adding " + CollectableType.Rune + " to the player");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.StrengthEvolution1).GetComponent<Collectable>().Init();
            Debug.Log("Pop some " + CollectableType.StrengthEvolution1 + " on the ground!");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.PlatformistEvolution1).GetComponent<Collectable>().Init();
            Debug.Log("Pop some " + CollectableType.PlatformistEvolution1 + " on the ground!");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.AgileEvolution1).GetComponent<Collectable>().Init();
            Debug.Log("Pop some " + CollectableType.AgileEvolution1 + " on the ground!");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.GhostEvolution1).GetComponent<Collectable>().Init();
            Debug.Log("Pop some " + CollectableType.GhostEvolution1 + " on the ground!");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            DebugPlayerSelected.UpdateCollectableValue(CollectableType.Money, 9999);
            Debug.Log("Adding " + CollectableType.Money + " to the player");
        }
    }

    void ChangeGameplayDataControls()
    {
        int increaseStep = Input.GetKey(KeyCode.LeftShift) ? 10 : 1;
        if (Input.GetKey(KeyCode.Alpha1))
            debugPlayerSelected.NbPoints += increaseStep;

        if (Input.GetKey(KeyCode.Alpha2))
            debugPlayerSelected.NbPoints -= increaseStep;

        if (Input.GetKey(KeyCode.Alpha3))
            debugPlayerSelected.NbLife += increaseStep;

        if (Input.GetKey(KeyCode.Alpha4))
            debugPlayerSelected.NbLife -= increaseStep;

        if (Input.GetKey(KeyCode.Alpha5))
            GameManager.Instance.DEBUG_IncreaseFinalCountdown(increaseStep);
    }

    void UnlockablesControls()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (DatabaseManager.Db.IsUnlock<DatabaseClass.MinigameData>(DatabaseManager.Db.minigames[DatabaseManager.Db.minigames.Count - 1].Id))
                {
                    foreach (DatabaseClass.MinigameData minigame in DatabaseManager.Db.minigames)
                        DatabaseManager.Db.SetUnlock<DatabaseClass.MinigameData>(minigame.Id, false);

                    Debug.Log("DEBUG: All minigames locked.");
                }
                else
                {
                    foreach (DatabaseClass.MinigameData minigame in DatabaseManager.Db.minigames)
                        DatabaseManager.Db.SetUnlock<DatabaseClass.MinigameData>(minigame.Id, true);

                    Debug.Log("DEBUG: All minigames unlocked.");
                }
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (DatabaseManager.Db.IsUnlock<DatabaseClass.RuneData>(DatabaseManager.Db.runes[DatabaseManager.Db.runes.Count - 1].Id))
                {
                    foreach (DatabaseClass.RuneData rune in DatabaseManager.Db.runes)
                        DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(rune.Id, false);

                    Debug.Log("DEBUG: All runes locked.");
                }
                else
                {
                    foreach (DatabaseClass.RuneData rune in DatabaseManager.Db.runes)
                        DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(rune.Id, true);

                    Debug.Log("DEBUG: All runes unlocked.");
                }
            }
        }

        // Unlock next minigame locked
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < DatabaseManager.Db.minigames.Count; ++i)
            {
                if (!DatabaseManager.Db.IsUnlock<DatabaseClass.MinigameData>(DatabaseManager.Db.minigames[i].Id))
                {
                    DatabaseManager.Db.SetUnlock<DatabaseClass.MinigameData>(DatabaseManager.Db.minigames[i].Id, true);
                    Debug.Log("DEBUG: Minigame " + DatabaseManager.Db.minigames[i].Id + " unlocked.");
                    break;
                }
            }
        }

        // Lock last minigame unlocked
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (int i = DatabaseManager.Db.minigames.Count - 1; i >= 0; --i)
            {
                if (DatabaseManager.Db.IsUnlock<DatabaseClass.MinigameData>(DatabaseManager.Db.minigames[i].Id))
                {
                    DatabaseManager.Db.SetUnlock<DatabaseClass.MinigameData>(DatabaseManager.Db.minigames[i].Id, false);
                    Debug.Log("DEBUG: Minigame " + DatabaseManager.Db.minigames[i].Id + " locked.");
                    break;
                }
            }
        }

        // Unlock next rune locked
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            for (int i = 0; i < DatabaseManager.Db.runes.Count; ++i)
            {
                if (!DatabaseManager.Db.IsUnlock<DatabaseClass.RuneData>(DatabaseManager.Db.runes[i].Id))
                {
                    DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(DatabaseManager.Db.runes[i].Id, true);
                    Debug.Log("DEBUG: Rune " + DatabaseManager.Db.runes[i].Id + " unlocked.");
                    break;
                }
            }
        }

        // Lock last rune unlocked
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            for (int i = DatabaseManager.Db.runes.Count - 1; i >= 0; --i)
            {
                if (DatabaseManager.Db.IsUnlock<DatabaseClass.RuneData>(DatabaseManager.Db.runes[i].Id))
                {
                    DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(DatabaseManager.Db.runes[i].Id, false);
                    Debug.Log("DEBUG: Rune " + DatabaseManager.Db.runes[i].Id + " locked.");
                    break;
                }
            }
        }

    }

    void CommonControls()
    {
        // Reset player
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DebugPlayerSelected.NbPoints = 0;
            DebugPlayerSelected.NbLife = 0;
            if (DebugPlayerSelected.GetComponent<EvolutionStrength>()) Destroy(DebugPlayerSelected.GetComponent<EvolutionStrength>());
            if (DebugPlayerSelected.GetComponent<EvolutionAgile>()) Destroy(DebugPlayerSelected.GetComponent<EvolutionAgile>());
            if (DebugPlayerSelected.GetComponent<EvolutionPlatformist>()) Destroy(DebugPlayerSelected.GetComponent<EvolutionPlatformist>());
            if (DebugPlayerSelected.GetComponent<EvolutionGhost>()) Destroy(DebugPlayerSelected.GetComponent<EvolutionGhost>());
            DebugPlayerSelected.PlayerCharacter.Rb.velocity = Vector3.zero;
            Debug.Log("Reset current player! " + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }

        // Respawn player
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Respawner.RespawnProcess(DebugPlayerSelected);
            DebugPlayerSelected.PlayerCharacter.Rb.velocity = Vector3.zero;
            Debug.Log("Reset current player to last respawn point! " + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }

        // Pop player
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject go = Instantiate(GameManager.Instance.PlayerStart.playerPrefab);
            go.transform.position = DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f;
            go.transform.rotation = Quaternion.identity;
            Player currentPlayer = go.GetComponent<Player>();
            currentPlayer.respawnPoint = DebugPlayerSelected.respawnPoint;

            PlayerControllerHub playerController = go.GetComponent<PlayerControllerHub>();
            playerController.DEBUG_hasBeenSpawnedFromTool = true;
            playerController.PlayerIndex = (PlayerIndex)5;
            playerController.IsUsingAController = true;
            playerController.PlayerIndexSet = true;

            GameManager.Instance.PlayerStart.PlayersReference.Add(go);
            debugSpawnedPlayer = go.GetComponent<Player>();

            Debug.Log("Player spawned! " + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }

        // Possess a player
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (debugSpawnedPlayer == null)
            {
                Debug.Log("No spawned player to possess");
                return;
            }

            if (!possessASpawnedPlayer)
            {
                PlayerControllerHub playerControllerOne = DebugPlayerSelected.GetComponent<PlayerControllerHub>();
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

                PlayerControllerHub playerControllerSpwn = debugSpawnedPlayer.GetComponent<PlayerControllerHub>();
                playerControllerSpwn.DEBUG_hasBeenSpawnedFromTool = false;
                playerControllerSpwn.PlayerIndex = (PlayerIndex)0;

                Debug.Log("Possess player one! ");
            }
            else
            {
                PlayerControllerHub playerControllerSpwn = debugSpawnedPlayer.GetComponent<PlayerControllerHub>();
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

                PlayerControllerHub playerControllerOne = DebugPlayerSelected.GetComponent<PlayerControllerHub>();
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

            if (DebugPlayerSelected.GetComponent<PlayerControllerHub>() != null)
                Debug.Log("Powers reloaded for player " + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }

        // Change debug player selected
        if (Input.GetKeyDown(KeyCode.N))
        {
            SwitchPlayer();

            if (DebugPlayerSelected.GetComponent<PlayerControllerHub>() != null)
                Debug.Log("Switch to player index: " + DebugPlayerSelected.GetComponent<PlayerControllerHub>().PlayerIndex);
        }

        // Teleport a player on a specified transform (in inspector)
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (teleportPlayerPositionsList == null || teleportPlayerPositionsList.Count == 0)
            {
                Debug.LogWarning("No transform specified for teleportation.");
                return;
            }
            DebugPlayerSelected.transform.position = teleportPlayerPositionsList[currentTeleportIndex].position;
            DebugPlayerSelected.transform.rotation = teleportPlayerPositionsList[currentTeleportIndex].rotation;
            currentTeleportIndex = (currentTeleportIndex + 1) % teleportPlayerPositionsList.Count;
        }

        // Turn on the lights on player
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (torchLights == null || torchLights.Count == 0)
            {
                Debug.LogWarning("No torchlight prefabs specified");
                return;
            }

            if (!lightsEnabled)
            {
                foreach (GameObject torch in torchLights)
                {
                    GameObject go = Instantiate(torch, DebugPlayerSelected.transform);
                    go.transform.localPosition = torch.transform.localPosition;
                    go.transform.localRotation = torch.transform.localRotation;
                }
            }
            else
            {
                Light[] lights = DebugPlayerSelected.GetComponentsInChildren<Light>();
                for (int i = 0; i < lights.Length; i++)
                    Destroy(lights[i].gameObject);
            }

            lightsEnabled = !lightsEnabled;
        }

        // Show debug UI
        if (Input.GetKey(KeyCode.U)
            && Input.GetKeyDown(KeyCode.I))
        {
            CurrentUIState = (DebugUIState)(((int)CurrentUIState + 1)% (int)DebugUIState.Size);
        }

        // Show debug help
        if (Input.GetKeyDown(KeyCode.H))
        {
            DebugPanelReference.GetComponent<DebugPanel>().helpPanel.gameObject.SetActive(!DebugPanelReference.GetComponent<DebugPanel>().helpPanel.gameObject.activeInHierarchy);
        }

        // Finish current mini game
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.Instance.CurrentGameMode.IsMiniGame())
            {
                try
                {
                    foreach (GameObject go in GameManager.Instance.PlayerStart.PlayersReference)
                        GameManager.Instance.CurrentGameMode.PlayerHasFinished(go.GetComponent<Player>());
                }
                catch
                {
                }
                finally
                {
                    // Stop timer
                    GameManager.Instance.DEBUG_EndFinalCountdown();
                    // Hide rule screen
                    GameManager.UiReference.RuleScreen.gameObject.SetActive(false);
                    // Destroy '3 2 1 Go' timer
                    ReadySetGo readySetGoComp = FindObjectOfType<ReadySetGo>();
                    if (readySetGoComp != null)
                        Destroy(readySetGoComp.gameObject);

                    // Call end game and make sure we can replay right away
                    ScoreScreen screenRef = GameManager.Instance.ScoreScreenReference;
                    screenRef.RankPlayersByPoints();
                    screenRef.DEBUG_SetMinigameReplayable();
                }
            }
        }
    }

    void Update () {
        // Activation/Deactivation debug mode
        if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKeyDown(KeyCode.RightShift))
        {
            ActivateDebugMode();
        }

        if (isDebugModeActive)
        {
            ComputeFPS();

            switch (currentState)
            {
                case DebugState.AddEvolution:
                    AddEvolutionControls();
                    break;
                case DebugState.SpawnCollectable:
                    SpawnCollectableControls();
                    break;
                case DebugState.GameplayData:
                    ChangeGameplayDataControls();
                    break;
                case DebugState.Unlockables:
                    UnlockablesControls();
                    break;
                default:
                    break;
            }

            if (Input.GetKeyDown(KeyCode.F1))
                CurrentState = DebugState.AddEvolution;

            else if (Input.GetKeyDown(KeyCode.F2))
                CurrentState = DebugState.SpawnCollectable;

            else if (Input.GetKeyDown(KeyCode.F3))
                CurrentState = DebugState.GameplayData;

            else if (Input.GetKeyDown(KeyCode.F4))
                CurrentState = DebugState.Unlockables;

            CommonControls();
        }
        
    }

    void ComputeFPS()
    {
        lastFramesTime.Add(Time.deltaTime);

        computedFPS = 0;
        for (int i = 0; i < lastFramesTime.Count; i++)
        {
            computedFPS += lastFramesTime[i];
        }

        if (computedFPS > 1.0f)
        {
            DebugPanelReference.GetComponent<DebugPanel>().UpdateFPS(lastFramesTime.Count / computedFPS);
            lastFramesTime.Clear();
        }
    }

    public void ActivateDebugMode(bool _forceActivation = false)
    {
        // No debug mode in menu
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            isDebugModeActive = false;
            return;
        }

        isDebugModeActive = (_forceActivation) ? true : !isDebugModeActive;
        DebugPanelReference.gameObject.SetActive(isDebugModeActive);
        CurrentState = ((GameManager.Instance.CurrentGameMode.IsMiniGame()) ? DebugState.GameplayData : DebugState.AddEvolution);

        if (!hasUpdatedDebugPanel)
            UpdateDebugPanel();

        SwitchPlayer(true);

        if (isDebugModeActive)
            Debug.Log("DEBUG MODE ACTIVATED!");
        else
            Debug.Log("DEBUG MODE DEACTIVATED!");
    }

    private static void SwitchPlayer(bool forcedToFirst = false)
    {
        //Index, IsGrounded, GravityEnabled, CurrentState, HasBeenTp, NbJumpMade, CameraState
        List<GameObject> playersReference = GameManager.Instance.PlayerStart.PlayersReference;

        int currentIndex = 0;

        if (forcedToFirst)
        {
            currentIndex = -1;
        }
        else
        {
            for (int i = 0; i < playersReference.Count; i++)
            {
                if (DebugPlayerSelected.gameObject == playersReference[i])
                {
                    currentIndex = i;
                    break;
                }
            }
        }
        debugPlayerSelected = playersReference[(currentIndex + 1) % playersReference.Count].GetComponent<Player>();

        debugPlayerInfos[(int)DebugPlayerInfos.Index] = ((int)debugPlayerSelected.PlayerController.playerIndex).ToString();
        debugPlayerInfos[(int)DebugPlayerInfos.IsGrounded] = (debugPlayerSelected.PlayerCharacter is PlayerCharacterHub) ? ((PlayerCharacterHub)debugPlayerSelected.PlayerCharacter).IsGrounded.ToString() : "--";
        debugPlayerInfos[(int)DebugPlayerInfos.GravityEnabled] = (debugPlayerSelected.PlayerCharacter is PlayerCharacterHub) ? ((PlayerCharacterHub)debugPlayerSelected.PlayerCharacter).IsGravityEnabled.ToString() : "--";

        debugPlayerInfos[(int)DebugPlayerInfos.CurrentState] = (debugPlayerSelected.PlayerCharacter is PlayerCharacterHub) ? ((PlayerCharacterHub)debugPlayerSelected.PlayerCharacter).PlayerState.ToString() : "--";
        debugPlayerInfos[(int)DebugPlayerInfos.HasBeenTp] = debugPlayerSelected.HasBeenTeleported.ToString();
        debugPlayerInfos[(int)DebugPlayerInfos.NbJumpMade] = (debugPlayerSelected.PlayerCharacter is PlayerCharacterHub) ? ((PlayerCharacterHub)debugPlayerSelected.PlayerCharacter).jumpState.NbJumpMade.ToString() : "--"; ;

        debugPlayerInfos[(int)DebugPlayerInfos.CameraState] = (debugPlayerSelected.PlayerCharacter is PlayerCharacterHub) ? debugPlayerSelected.cameraReference.GetComponentInChildren<DynamicJoystickCameraController>().CurrentState.ToString() : "--" ;

        DebugPanel debugPanelRef = FindObjectOfType<DebugPanel>();
        if (debugPanelRef != null)
            FindObjectOfType<DebugPanel>().UpdatePlayerInfoText();
    }


    public static void UpdatePlayerInfos(DebugPlayerInfos _type, string _value, int _playerIndex)
    {
        if (debugPlayerSelected != null && _playerIndex != debugPlayerSelected.ID)
            return;

        debugPlayerInfos[(int)_type] = _value;
        DebugPanel debugPanelRef = FindObjectOfType<DebugPanel>();
        if (debugPanelRef != null)
            FindObjectOfType<DebugPanel>().UpdatePlayerInfoText();
    }

    public static void UpdateEvolutionInfos(DebugEvolutionInfos _type, string _value)
    {
        debugEvolutionInfos[(int)_type] = _value;
        DebugPanel debugPanelRef = FindObjectOfType<DebugPanel>();
        if (debugPanelRef != null)
            FindObjectOfType<DebugPanel>().UpdatePlayerInfoText();
    }

}
