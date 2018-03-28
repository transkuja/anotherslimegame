using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugPanel : MonoBehaviour {
    enum DebugPanelChildren { EvolutionsState, CollectablesState, PlayerInfo, Infos, ActivationText };
    enum DebugUIState { None, FPS, Full, Size};

    public Transform evolutions;
    public Transform collectables;
    public Transform playerInfo;
    public Transform helpPanel;
    public Text activationText;

    Text evolutionsText;
    Text collectablesText;
    Text playerInfoText;
    public Text infoText;
    DebugUIState currentState;

    public void ChangeState(int _newState)
    {
        currentState = (DebugUIState)_newState;
        evolutionsText.text = "";
        collectablesText.text = "";

        switch (currentState)
        {
            case DebugUIState.FPS:
                playerInfoText.text = "FPS: 0.0";
                evolutions.gameObject.SetActive(false);
                collectables.gameObject.SetActive(false);
                playerInfo.gameObject.SetActive(true);
                break;
            case DebugUIState.Full:
                evolutions.gameObject.SetActive(true);
                collectables.gameObject.SetActive(true);
                playerInfo.gameObject.SetActive(true);
                break;
            default:
                evolutions.gameObject.SetActive(false);
                collectables.gameObject.SetActive(false);
                playerInfo.gameObject.SetActive(false);
                break;
        }
    }

    void Start () {
        evolutions = transform.GetChild((int)DebugPanelChildren.EvolutionsState);
        collectables = transform.GetChild((int)DebugPanelChildren.CollectablesState);
        playerInfo = transform.GetChild((int)DebugPanelChildren.PlayerInfo);
        helpPanel = transform.GetChild((int)DebugPanelChildren.Infos);

        evolutionsText = evolutions.GetComponent<Text>();
        collectablesText = collectables.GetComponent<Text>();
        playerInfoText = playerInfo.GetComponent<Text>();
        evolutionsText.text = collectablesText.text = playerInfoText.text = "";
    }

    public string AddToDebugPanelInfos(string firstKey, string secondKey, string description)
    {
        return " " + firstKey 
            + ((secondKey != "") ? " + " + secondKey : " ") 
            + ((firstKey != "" && secondKey != "") ? ": " : "") 
            + description 
            + "\n";
    }

    public void UpdateDebugPanelInfos(string _commonControls, string _stateControls)
    {
        infoText.text = _commonControls + _stateControls;
    }

    void Update () {
        if (currentState == DebugUIState.Full)
        {
            UpdateEvolutionText();
            UpdateCollectableText();
            UpdatePlayerInfoText();
        }

    }

    void UpdateEvolutionText()
    {
        Player player = DebugTools.DebugPlayerSelected;
        evolutionsText.text = "";
        evolutionsText.text += Powers.Platformist + ": " + ((player.GetComponent<EvolutionPlatformist>() != null) ? ((player.GetComponent<EvolutionPlatformist>().Timer == 0.0f) ? "Active" : player.GetComponent<EvolutionPlatformist>().Timer.ToString("0.0") + "s") : "Inactive") + "\n";
        evolutionsText.text += Powers.Strength + ": " + ((player.GetComponent<EvolutionStrength>() != null) ? ((player.GetComponent<EvolutionStrength>().Timer == 0.0f) ? "Active" : player.GetComponent<EvolutionStrength>().Timer.ToString("0.0") + "s") : "Inactive") + "\n";
        evolutionsText.text += Powers.Agile + ": " + ((player.GetComponent<EvolutionAgile>() != null) ? ((player.GetComponent<EvolutionAgile>().Timer == 0.0f) ? "Active" : player.GetComponent<EvolutionAgile>().Timer.ToString("0.0") + "s") : "Inactive") + "\n";
        evolutionsText.text += Powers.Ghost + ": " + ((player.GetComponent<EvolutionGhost>() != null) ? ((player.GetComponent<EvolutionGhost>().Timer == 0.0f) ? "Active" : player.GetComponent<EvolutionGhost>().Timer.ToString("0.0") + "s") : "Inactive") + "\n";
    }

    void UpdateCollectableText()
    {
        collectablesText.text = "";
        collectablesText.text += GameManager.Instance.Runes + " " + CollectableType.Rune.ToString() + "\n";
        collectablesText.text += GameManager.Instance.GlobalMoney + " " + CollectableType.Money.ToString() + "\n";

        Player player = DebugTools.DebugPlayerSelected;
        collectablesText.text += player.NbLife + " " + PlayerUIStat.Life.ToString() + "\n";
        collectablesText.text += player.NbPoints + " " + PlayerUIStat.Points.ToString() + "\n";
    }

    void UpdatePlayerInfoText()
    {
        PlayerControllerHub playerController = DebugTools.DebugPlayerSelected.GetComponent<PlayerControllerHub>();
        if (playerController == null)
            return;

        playerInfoText.text = "Game state: " + GameManager.CurrentState + "\n";
        playerInfoText.text += "Player index: " + (int)playerController.PlayerIndex + "\n";
        playerInfoText.text += "Use a controller: " + playerController.IsUsingAController + "\n";
        playerInfoText.text += "Is grounded: " + playerController.IsGrounded + "\n";
        playerInfoText.text += "Gravity enabled: " + playerController.isGravityEnabled + "\n";
        playerInfoText.text += "Current state: " + playerController.PlayerState + "\n";
        playerInfoText.text += "Has been teleported: " + DebugTools.DebugPlayerSelected.hasBeenTeleported + "\n";
        playerInfoText.text += "NbJumpMade: " + ((PlayerControllerHub)DebugTools.DebugPlayerSelected.PlayerController).jumpState.nbJumpMade + "\n";
        playerInfoText.text += "Camera State: " + DebugTools.DebugPlayerSelected.cameraReference.GetComponentInChildren<DynamicJoystickCameraController>().currentState + "\n";

        if (playerController.GetComponent<EvolutionPlatformist>())
        {
            playerInfoText.text += "Charges: " + playerController.GetComponent<EvolutionPlatformist>().Charges + "\n";
            playerInfoText.text += "Pattern index: " + playerController.GetComponent<EvolutionPlatformist>().IndexPattern + "\n";
        }
        if (playerController.GetComponent<EvolutionGhost>())
        {
            EvolutionGhost ghost = playerController.GetComponent<EvolutionGhost>();
            playerInfoText.text += "Charge: " + (ghost.CurrentEmissionTimeLeft / ghost.MaxEmissionTime * 100.0f).ToString("0") + "%\n";
            playerInfoText.text += "Usable: " + !ghost.HitZero + "\n";
        }
    }

    public void UpdateFPS(float _fps)
    {
        if (currentState == DebugUIState.FPS)
            playerInfoText.text = "FPS: " + _fps.ToString("0.0");
    }

}
