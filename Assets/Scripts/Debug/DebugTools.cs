using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour {

    private static bool isDebugModeActive = false;

    [SerializeField]
    Transform debugPanel;

    public static Player debugPlayerSelected;

    private void Start()
    {
        if (debugPanel == null)
        {
            Debug.LogWarning("DebugPanel is not linked on DebugTools, autolink with Find ...");
            debugPanel = GameObject.Find("DebugPanel").transform;
        }

        // TODO: very ugly handling, should be refactored when multiplayer are handled (references in GameManager?)
        if (debugPlayerSelected == null)
        {
            debugPlayerSelected = GameObject.Find("Player").GetComponent<Player>();
        }
    }

    void Update () {
        if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKeyDown(KeyCode.CapsLock))
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
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                debugPlayerSelected.Collectables = new int[(int)CollectableType.Size];
                if (debugPlayerSelected.GetComponent<DoubleJump>()) Destroy(debugPlayerSelected.GetComponent<DoubleJump>());
                if (debugPlayerSelected.GetComponent<Hover>()) Destroy(debugPlayerSelected.GetComponent<Hover>());
            }
            
        }
        
    }
}
