using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugPanel : MonoBehaviour {
    enum DebugPanelChildren { EvolutionsState, CollectablesState, PlayerInfo, Infos };

    public Transform evolutions;
    public Transform collectables;
    public Transform playerInfo;
    public Transform helpPanel;
    Text evolutionsText;
    Text collectablesText;
    Text playerInfoText;
    public Text infoText;

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

    public void ResetInfoText()
    {
        infoText.text = "";
    }

    public void AddToDebugPanelInfos(string firstKey, string secondKey, string description)
    {
        infoText.text += " " + firstKey 
            + ((secondKey != "") ? " + " + secondKey : " ") 
            + ((firstKey != "" && secondKey != "") ? ": " : "") 
            + description 
            + "\n";
    }

    void Update () {
        UpdateEvolutionText();
        UpdateCollectableText();
        UpdatePlayerInfoText();
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
        Player player = DebugTools.DebugPlayerSelected;
        collectablesText.text = "";
        for (int i = 0; i < player.Collectables.Length; i++)
            collectablesText.text += player.Collectables[i] + " " + ((CollectableType)i).ToString() + " collectable\n";
    }

    void UpdatePlayerInfoText()
    {
        PlayerControllerHub playerController = DebugTools.DebugPlayerSelected.GetComponent<PlayerControllerHub>();
        playerInfoText.text = "";
        playerInfoText.text += "Player index: " + (int)playerController.PlayerIndex + "\n";
        playerInfoText.text += "Use a controller: " + playerController.IsUsingAController + "\n";
        playerInfoText.text += "Is grounded: " + playerController.IsGrounded + "\n";
        playerInfoText.text += "Current state: " + playerController.PlayerState + "\n";
        playerInfoText.text += "Has been teleported: " + DebugTools.DebugPlayerSelected.hasBeenTeleported + "\n";
        playerInfoText.text += "NbJumpMade: " + DebugTools.DebugPlayerSelected.PlayerController.jumpState.nbJumpMade + "\n";

        if (playerController.GetComponent<EvolutionPlatformist>())
        {
            playerInfoText.text += "Charges: " + playerController.GetComponent<EvolutionPlatformist>().Charges + "\n";
            playerInfoText.text += "Pattern index: " + playerController.GetComponent<EvolutionPlatformist>().IndexPattern + "\n";
        }
        if(playerController.GetComponent<EvolutionGhost>())
        {
            EvolutionGhost ghost = playerController.GetComponent<EvolutionGhost>();
            playerInfoText.text += "Charge: " + (ghost.CurrentEmissionTimeLeft/ ghost.MaxEmissionTime * 100.0f).ToString("0") + "%\n";
            playerInfoText.text += "Usable: " + !ghost.HitZero + "\n";
        }
    }

}
