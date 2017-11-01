using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour {

    private static bool isDebugModeActive = false;

    [SerializeField]
    Transform debugPanel;

    private static Player debugPlayerSelected;

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
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (DebugPlayerSelected.GetComponent<DoubleJump>() == null)
                        GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.DoubleJump));
                    Debug.Log("Added Double Jump on player " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    if (DebugPlayerSelected.GetComponent<Hover>() == null)
                        GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Hover));
                    Debug.Log("Added Hover on player " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    if (DebugPlayerSelected.GetComponent<EvolutionStrength>() == null)
                        GameManager.EvolutionManager.AddEvolutionComponent(DebugPlayerSelected.gameObject, GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Strength));
                    Debug.Log("Added Strength on player " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                if (Input.GetKeyUp(KeyCode.Alpha1))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.WingsEvolution1);
                    Debug.Log("Pop some " + CollectableType.WingsEvolution1 + " on the ground!");
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.StrengthEvolution1);
                    Debug.Log("Pop some " + CollectableType.StrengthEvolution1 + " on the ground!");
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.Points);
                    Debug.Log("Pop some " + CollectableType.Points + " on the ground!");
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                        DebugPlayerSelected.transform.position + DebugPlayerSelected.transform.forward * 4.0f, Quaternion.identity, null, CollectableType.Key);
                    Debug.Log("Pop some " + CollectableType.Key + " on the ground!");
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    DebugPlayerSelected.Collectables = new int[(int)CollectableType.Size];
                    if (DebugPlayerSelected.GetComponent<DoubleJump>()) Destroy(DebugPlayerSelected.GetComponent<DoubleJump>());
                    if (DebugPlayerSelected.GetComponent<Hover>()) Destroy(DebugPlayerSelected.GetComponent<Hover>());
                    if (DebugPlayerSelected.GetComponent<EvolutionStrength>()) Destroy(DebugPlayerSelected.GetComponent<EvolutionStrength>());
                    DebugPlayerSelected.Rb.velocity = Vector3.zero;
                    Debug.Log("Reset current player! " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }

                if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    Respawner.RespawnProcess(DebugPlayerSelected);
                    DebugPlayerSelected.Rb.velocity = Vector3.zero;
                    Debug.Log("Reset current player to last respawn point! " + DebugPlayerSelected.GetComponent<PlayerController>().PlayerIndex);
                }
            }
        }
        
    }
}
