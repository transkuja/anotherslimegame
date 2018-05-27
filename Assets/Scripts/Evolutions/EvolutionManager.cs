using System;
using System.Collections;
using System.Collections.Generic;
using UWPAndXInput;
using UnityEngine;

public class Evolution
{
    int id;
    public float duration;
    CollectableType associatedCollectable;
    int cost;
    BodyPart bodyPart;
    Powers power;

        // GamePlay for each evolution
    
    public int Cost
    {
        get
        {
            return cost;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }
    }

    public CollectableType AssociatedCollectable
    {
        get
        {
            return associatedCollectable;
        }
    }

    public BodyPart BodyPart
    {
        get
        {
            return bodyPart;
        }
    }

    public Evolution(Powers _powerName, float _duration, CollectableType _associatedCollectable, int _cost, BodyPart _bodyPart)
    {
        id = (int)_powerName;
        duration = _duration;
        associatedCollectable = _associatedCollectable;
        cost = _cost;
        bodyPart = _bodyPart;
    }
}

public enum Powers { Strength, Platformist, Agile, Ghost, Size }

public class EvolutionManager {

    // Evolution database handled in code
    Evolution strengthEvolution = new Evolution(Powers.Strength, -1, CollectableType.StrengthEvolution1, 30, BodyPart.Hammer);
    Evolution agileEvolution = new Evolution(Powers.Agile, -1, CollectableType.AgileEvolution1, 30, BodyPart.Wings);
    Evolution platformistEvolution = new Evolution(Powers.Platformist, -1, CollectableType.PlatformistEvolution1, 30, BodyPart.Staff);
    Evolution ghostEvolution = new Evolution(Powers.Ghost, -1, CollectableType.GhostEvolution1, 30, BodyPart.None);

    public Evolution GetEvolutionByPowerName(Powers _powerName, bool overrideEvolutionDuration = false, float evolutionDuration = 0.0f)
    {
        Evolution tmpEvolution;

        switch (_powerName)
        {
            case Powers.Strength:
                tmpEvolution = strengthEvolution;
                break;
            case Powers.Agile:
                tmpEvolution = agileEvolution;
                break;
            case Powers.Platformist:
                tmpEvolution = platformistEvolution;
                break;
            case Powers.Ghost:
                tmpEvolution = ghostEvolution;
                break;
            default:
                Debug.Log("Unknown power, something went wrong");
                return null;
        }

        if (overrideEvolutionDuration)
            tmpEvolution.duration = evolutionDuration;
        return tmpEvolution;
    }

    public void AddEvolutionComponent(GameObject gameObject, Evolution evolution, bool overrideEvolutionDuration = false, float evolutionDuration = 0.0f)
    {
        Powers power = (Powers)evolution.Id;
        float duration = (overrideEvolutionDuration) ? evolutionDuration : evolution.duration;
        EvolutionComponent newEvolution;

        switch (power)
        {
            case Powers.Strength:
                newEvolution = gameObject.AddComponent<EvolutionStrength>();
                break;
            case Powers.Agile:
                newEvolution = gameObject.AddComponent<EvolutionAgile>();
                break;
            case Powers.Platformist:
                newEvolution = gameObject.AddComponent<EvolutionPlatformist>();
                break;
            case Powers.Ghost:
                newEvolution = gameObject.AddComponent<EvolutionGhost>();
                break;
            default:
                newEvolution = null;
                Debug.Log("Unknown power, something went wrong");
                break;

        }
        newEvolution.Timer = duration;
    }

    /*
     * Return the evolution linked to the collectable type or null if the collectable is not linked to any evolution (ex: Points)
     */
    public Evolution GetEvolutionByCollectableType(CollectableType _type)
    {
        switch (_type)
        {
            case CollectableType.StrengthEvolution1:
                return strengthEvolution;
            case CollectableType.PlatformistEvolution1:
                return platformistEvolution;
            case CollectableType.AgileEvolution1:
                return agileEvolution;
            case CollectableType.GhostEvolution1:
                return ghostEvolution;
            default:
                Debug.LogError("Unhandle Evolution");
                return null;
        }
    }
}
