using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameModeType {
    Escape, // Main Gamemode, 1st phase run and grab evolution, 2nd phase escape with X keys
    Arena // Fight and collect points  
};

public enum EvolutionMode {
    GrabEvolution, // Evolutions are collectables, they are permanent once the player grab it
    GrabCollectableAndActivate, // Evolutions have a collectable cost, the player can then choose which one he wants to activate
    GrabCollectableAndAutoEvolve // There are several types of collectables, one for each evolution. Once the player reach the required amount, he gets the evolution
};

public class GameMode {
    public GameModeType gameModeType;
    public EvolutionMode evolutionMode;
    int nbPlayersMin;
    int nbPlayersMax;

    public GameMode(GameModeType _gameModeType, EvolutionMode _evolutionMode, int _nbPlayersMin, int _nbPlayersMax)
    {
        gameModeType = _gameModeType;
        evolutionMode = _evolutionMode;
        nbPlayersMin = _nbPlayersMin;
        nbPlayersMax = _nbPlayersMax;
    }
}

/*
 * Handles gamemodes with an internal database in code
 */
public class GameModeManager
{
    GameMode escapeMode = new GameMode(GameModeType.Escape, EvolutionMode.GrabEvolution, 1, 4);

    GameMode arenaMode1 = new GameMode(GameModeType.Arena, EvolutionMode.GrabCollectableAndAutoEvolve, 1, 4);
    GameMode arenaMode2 = new GameMode(GameModeType.Arena, EvolutionMode.GrabCollectableAndActivate, 1, 4);


    public GameMode GetGameModeByName(GameModeType _name, EvolutionMode _evolutionMode = EvolutionMode.GrabEvolution)
    {
        switch (_name)
        {
            case GameModeType.Escape:
                return escapeMode;
            case GameModeType.Arena:
                if (_evolutionMode == EvolutionMode.GrabCollectableAndAutoEvolve)
                    return arenaMode1;
                else
                    return arenaMode2;
            default:
                Debug.LogWarning("The gamemode name specified is unknown:" + _name);
                return null;
        }
    }
}