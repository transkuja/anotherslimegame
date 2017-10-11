using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution
{
    int id;
    string name; 
    float duration;
    CollectableType associatedCollectable;
    int cost;
    BodyPart bodyPart;
    Powers power;

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

    public float Duration
    {
        get
        {
            return duration;
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
public enum Powers { DoubleJump, Hover, Size }

public class EvolutionManager {
    List<Evolution> evolutions = new List<Evolution>();

    // Evolution database handled in code
    Evolution doubleJumpEvolution = new Evolution(Powers.DoubleJump, 5, CollectableType.Evolution1, 20, BodyPart.Head);
    Evolution hoverEvolution = new Evolution(Powers.Hover, 3, CollectableType.Evolution2, 30, BodyPart.Wings);

    private void Start()
    {
        evolutions.Add(doubleJumpEvolution);
        evolutions.Add(hoverEvolution);
    }

    public Evolution GetEvolutionByPowerName(Powers _powerName)
    {
        switch (_powerName)
        {
            case Powers.DoubleJump:
                return doubleJumpEvolution;
            case Powers.Hover:
                return hoverEvolution;
            default:
                Debug.Log("Unknown power, something went wrong");
                return null;
        }
    }

    public void AddEvolutionComponent(GameObject gameObject, Evolution evolution)
    {
        Powers power = (Powers)evolution.Id;
        switch (power)
        {
            case Powers.DoubleJump:
                if (gameObject.GetComponent<DoubleJump>() != null) gameObject.GetComponent<DoubleJump>().Timer = evolution.Duration;
                else
                    gameObject.AddComponent<DoubleJump>();
                break;
            case Powers.Hover:
                if (gameObject.GetComponent<Hover>() != null) gameObject.GetComponent<Hover>().Timer = evolution.Duration;
                else
                    gameObject.AddComponent<Hover>(); break;
            default:
                Debug.Log("Unknown power, something went wrong");
                break;
        }
    }

    // Only for gameplay 1
    public Evolution GetEvolutionByCollectableType(CollectableType _type)
    {
        switch (_type)
        {
            case CollectableType.Evolution1:
                return doubleJumpEvolution;
            case CollectableType.Evolution2:
                return hoverEvolution;
            default:
                Debug.Log("Unknown power, something went wrong");
                return null;
        }
    }
}
