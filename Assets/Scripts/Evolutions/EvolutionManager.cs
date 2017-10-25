using System;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

public class Evolution
{
    int id;
    string name; 
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
        name = _powerName.ToString();
        duration = _duration;
        associatedCollectable = _associatedCollectable;
        cost = _cost;
        bodyPart = _bodyPart;
    }


  
}

public enum BodyPart { Head, Wings, Size }
public enum Powers { DoubleJump, Hover, Size, Strengh }

public class EvolutionManager {

    // Evolution database handled in code
    Evolution doubleJumpEvolution = new Evolution(Powers.DoubleJump, 5, CollectableType.Evolution1, 20, BodyPart.Head);
    Evolution hoverEvolution = new Evolution(Powers.Hover, 3, CollectableType.Evolution2, 30, BodyPart.Wings);
    Evolution strenghEvolution = new Evolution(Powers.Strengh, 3, CollectableType.Evolution2, 30, BodyPart.Wings);

    public Evolution GetEvolutionByPowerName(Powers _powerName, bool isPermanent = false)
    {
        Evolution tmpEvolution;

        switch (_powerName)
        {
            case Powers.DoubleJump:
                tmpEvolution = doubleJumpEvolution;
                break;
            case Powers.Hover:
                tmpEvolution = hoverEvolution;
                break;
            case Powers.Strengh:
                tmpEvolution = strenghEvolution;
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
            case Powers.DoubleJump:
                if (gameObject.GetComponent<DoubleJump>() != null) gameObject.GetComponent<DoubleJump>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<DoubleJump>();
                break;
            case Powers.Hover:
                if (gameObject.GetComponent<Hover>() != null) gameObject.GetComponent<Hover>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<Hover>(); break;
            case Powers.Strengh:
                if (gameObject.GetComponent<EvolutionStrengh>() != null) gameObject.GetComponent<EvolutionStrengh>().Timer = (isPermanent) ? 0.0f : evolution.duration;
                else
                    gameObject.AddComponent<EvolutionStrengh>(); break;
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
            case CollectableType.Evolution1:
                return doubleJumpEvolution;
            case CollectableType.Evolution2:
                return hoverEvolution;
            default:
                return null;
        }
    }
}
