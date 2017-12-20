using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



abstract public class GameMode {

    readonly protected int nbPlayersMin;
    readonly protected int nbPlayersMax;

    public GameMode(int _nbPlayerMin= 2,int nbPlayerMax = 4)
    {
        nbPlayersMin = _nbPlayerMin;
    }
    public virtual void OnBegin()
    {

    }
    public abstract void AttributeCamera();
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