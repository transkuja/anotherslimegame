﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RunnerUI : APlayerUI {

    // Je dois avoir plusieurs ecran de mort selonles joueurs. 
    // un ecran de mort pour le moment c'est : 
    // un fond + un text ( = prefab. 
    // Le Sprite de fond doit avoir une taille variable + position variable. ( pareil pour le texte.)

    // Je dois les isntancier losq'un joueur meurt. 
    // je dois prévoir le reload. 
    [SerializeField]GameObject deathScreenModel;

    public override void Init()
    {
        base.Init();
        foreach (GameObject p in GameManager.Instance.PlayerStart.PlayersReference)
            p.GetComponent<PlayerControllerHub>().OnDeathEvent += OnPlayerDeath;
    }

    public void OnPlayerDeath(int playerId)
    {
        GameObject deathScreen = Instantiate(deathScreenModel, Vector3.zero, Quaternion.identity, transform);
        int maxPlayerNb = GameManager.Instance.PlayerStart.PlayersReference.Count;
            // if 4 player invert player screen places
        if (maxPlayerNb>2)
            playerId = (playerId + 2) % 4;

        // delinéarisation : ID player [0] à [4] --> offset [0,0]  à [1,1]
        // [0,0], [1,0] 
        // [0,1], [1,1]
        Vector2 playerIDOffset = new Vector2(playerId % 2, playerId / 2);

            // place anchor for player
        RectTransform rectTr = deathScreen.GetComponent<RectTransform>();
        Vector2 anchorMin = playerIDOffset * 0.5f;
        rectTr.anchoredPosition = Vector3.zero;
        rectTr.anchorMin = new Vector2(anchorMin.x, anchorMin.y);
        Vector2 anchorMax = playerIDOffset * 0.5f;
        if (maxPlayerNb == 1)
            anchorMax = anchorMin + Vector2.one;
        else if (maxPlayerNb == 2)
            anchorMax = anchorMin + new Vector2(0.5f, 1);
        else
            anchorMax = anchorMin + Vector2.one * 0.5f;
        rectTr.anchorMax = new Vector2(anchorMax.x, anchorMax.y);

        // Set nb points on death screen
        GameObject curPlayer = GameManager.Instance.PlayerStart.PlayersReference[playerId];
        deathScreen.transform.GetChild(1).GetComponentInChildren<Text>().text = curPlayer.GetComponent<Player>().NbPoints + " pts";

    }

}
