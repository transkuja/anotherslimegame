using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugPanel : MonoBehaviour {
    enum DebugPanelChildren { EvolutionsState, CollectablesState, PlayerInfo };

    Transform evolutions;
    Transform collectables;
    Transform playerInfo;
    Text evolutionsText;
    Text collectablesText;
    Text playerInfoText;

    void Start () {
        evolutions = transform.GetChild((int)DebugPanelChildren.EvolutionsState);
        collectables = transform.GetChild((int)DebugPanelChildren.CollectablesState);
        playerInfo = transform.GetChild((int)DebugPanelChildren.PlayerInfo);
        evolutionsText = evolutions.GetComponent<Text>();
        collectablesText = collectables.GetComponent<Text>();
        playerInfoText = playerInfo.GetComponent<Text>();
        evolutionsText.text = collectablesText.text = playerInfoText.text = "";
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
        evolutionsText.text += Powers.DoubleJump + ": " + ((player.GetComponent<DoubleJump>() != null) ? player.GetComponent<DoubleJump>().Timer.ToString("0.0") + "s" : "Inactive") + "\n";
        evolutionsText.text += Powers.Hover + ": " + ((player.GetComponent<Hover>() != null) ? player.GetComponent<Hover>().Timer.ToString("0.0") + "s" : "Inactive") + "\n";
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
        PlayerController playerController = DebugTools.DebugPlayerSelected.GetComponent<PlayerController>();
        playerInfoText.text = "";
        playerInfoText.text += "Player index: " + (int)playerController.PlayerIndex + "\n";
        playerInfoText.text += "Use a controller: " + playerController.IsUsingAController + "\n";
        playerInfoText.text += "Is grounded: " + playerController.IsGrounded + "\n";
    }
}
