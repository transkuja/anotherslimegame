using System;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
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

public enum BodyPart { Body, Wings, Hammer, Staff, Size }
public enum Powers { Strength, Platformist, Agile, Ghost, Size }

public class EvolutionManager {

    // Evolution database handled in code
    Evolution strengthEvolution = new Evolution(Powers.Strength, 3, CollectableType.StrengthEvolution1, 30, BodyPart.Hammer);
    Evolution agileEvolution = new Evolution(Powers.Agile, 3, CollectableType.AgileEvolution1, 30, BodyPart.Wings);
    // TODO: assign a new bodypart for the platformist evolution
    Evolution platformistEvolution = new Evolution(Powers.Platformist, 3, CollectableType.PlatformistEvolution1, 30, BodyPart.Hammer);
    // TODO: assign a new bodypart for the platformist evolution
    Evolution ghostEvolution = new Evolution(Powers.Ghost, 3, CollectableType.GhostEvolution1, 30, BodyPart.Hammer);

    public Evolution GetEvolutionByPowerName(Powers _powerName, bool isPermanent = false)
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

        if (isPermanent) tmpEvolution.duration = 0.0f;
        return tmpEvolution;
    }

    public void AddEvolutionComponent(GameObject gameObject, Evolution evolution, bool isPermanent = false)
    {
        Powers power = (Powers)evolution.Id;
        switch (power)
        {
            case Powers.Strength:
                if (gameObject.GetComponent<EvolutionStrength>() != null) gameObject.GetComponent<EvolutionStrength>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<EvolutionStrength>(); break;
            case Powers.Agile:
                if (gameObject.GetComponent<EvolutionAgile>() != null) gameObject.GetComponent<EvolutionAgile>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<EvolutionAgile>(); break;
            case Powers.Platformist:
                if (gameObject.GetComponent<EvolutionPlatformist>() != null) gameObject.GetComponent<EvolutionPlatformist>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<EvolutionPlatformist>(); break;
            case Powers.Ghost:
                if (gameObject.GetComponent<EvolutionGhost>() != null) gameObject.GetComponent<EvolutionGhost>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<EvolutionGhost>(); break;
            default:
                Debug.Log("Unknown power, something went wrong");
                break;
        }
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
