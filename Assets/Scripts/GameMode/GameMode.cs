﻿using System;
using System.Collections.Generic;
using UnityEngine;


public enum MiniGame { None, KickThemAll, Size }

abstract public class GameMode : MonoBehaviour
{
    [SerializeField] protected int nbPlayersMin;
    [SerializeField] protected int nbPlayersMax;
    // Use to remove damage on points based on gamemode when players collide. Players will still be expulsed
    [SerializeField] private bool takesDamageFromPlayer = true;
    // Use to remove damage on points based on gamemode when players collide with a trap. Players will still be expulsed
    [SerializeField] private bool takesDamageFromTraps = true;

    public bool TakesDamageFromPlayer
    {
        get
        {
            return takesDamageFromPlayer;
        }
    }

    public bool TakesDamageFromTraps
    {
        get
        {
            return takesDamageFromTraps;
        }
    }

    public void Awake()
    {
        GameManager.Instance.CurrentGameMode = this;
    }
    public virtual bool IsMiniGame()
    {
        return this is HubMode;
    }

    public virtual void StartGame(List<GameObject> playerReferences)
    {
        
    }
    public virtual void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        if (cameraReferences.Length == 0)
        {
            Debug.LogError("No camera assigned in playerStart");
            return;
        }
        // By default, cameraP2 is set for 2-Player mode, so we only update cameraP1
        if (activePlayersAtStart == 2)
        {
            cameraReferences[0].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1.0f);
        }
        // By default, cameraP3 and cameraP4 are set for 4-Player mode, so we only update cameraP1 and cameraP2
        else if (activePlayersAtStart > 2)
        {
            cameraReferences[0].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            cameraReferences[1].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
    public virtual  void PlayerHasFinished(Player player)
    {
        throw new NotImplementedException();
    }
    public static string GetSceneNameFromMinigame(MiniGame _miniGame)
    {
        if (_miniGame == MiniGame.KickThemAll)
            return "SceneMinigamePush";
        return "";
    }
}



/*
 * Handles gamemodes with an internal database in code
 */
//public class GameModeManager
//{
//    //GameMode escapeMode = new GameMode(GameModeType.Escape, EvolutionMode.GrabEvolution, 1, 4);

//    //GameMode arenaMode1 = new GameMode(GameModeType.Arena, EvolutionMode.GrabCollectableAndAutoEvolve, 1, 4);
//    //GameMode arenaMode2 = new GameMode(GameModeType.Arena, EvolutionMode.GrabCollectableAndActivate, 1, 4);


//    //public GameMode GetGameModeByName(GameModeType _name, EvolutionMode _evolutionMode = EvolutionMode.GrabEvolution)
//    //{
//    //    //switch (_name)
//    //    //{
//    //    //    case GameModeType.Escape:
//    //    //        return escapeMode;
//    //    //    case GameModeType.Arena:
//    //    //        if (_evolutionMode == EvolutionMode.GrabCollectableAndAutoEvolve)
//    //    //            return arenaMode1;
//    //    //        else
//    //    //            return arenaMode2;
//    //    //    default:
//    //    //        Debug.LogWarning("The gamemode name specified is unknown:" + _name);
//    //    //        return null;
//    //    //}
//    //}
//}