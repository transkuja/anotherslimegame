﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUtils : MonoBehaviour {

    public GameObject spawnableSpriteUI;

    [Header("Victory panel")]
    [SerializeField]
    public Sprite victoryNumber1;
    [SerializeField]
    public Sprite victoryNumber2;
    [SerializeField]
    public Sprite victoryNumber3;
    [SerializeField]
    public Sprite victoryNumber4;

    [Header("Texture Panneau sur les iles")]
    [SerializeField]
    public Texture Hammer;
    [SerializeField]
    public Texture Ghost;
    [SerializeField]
    public Texture Agile;
    [SerializeField]
    public Texture Platformist;

    [Header("Controls sprites")]
    public Sprite movementSprite;
    public Sprite jumpSprite;
    public Sprite actionSprite;
    public Sprite specialActionSprite;
    public Sprite rightTriggerSprite;
    public Sprite leftTriggerSprite;
    public Sprite interactionSprite;

    [Header("GP related")]
    public Sprite badOneSprite;

    public Sprite GetControlSprite(ControlType _type)
    {
        Sprite result = null;
        switch(_type)
        {
            case ControlType.Interaction:
                result = interactionSprite;
                break;
            case ControlType.Action:
                result = actionSprite;
                break;
            case ControlType.SpecialAction:
                result = specialActionSprite;
                break;
            case ControlType.Movement:
                result = movementSprite;
                break;
            case ControlType.Jump:
                result = jumpSprite;
                break;
            case ControlType.RightTrigger:
                result = rightTriggerSprite;
                break;
            case ControlType.LeftTrigger:
                result = leftTriggerSprite;
                break;
            case ControlType.Steering:
                result = movementSprite;
                break;
            default:
                return null;
        }

        if (result == null)
            Debug.LogWarning("No specified sprite for control type " + _type);

        return result;
    }

    public Sprite GetSpriteFromInput(PossibleInputs _input)
    {
        switch (_input)
        {
            case PossibleInputs.A:
                return jumpSprite;
            case PossibleInputs.X:
                return actionSprite;
            case PossibleInputs.Y:
                return specialActionSprite;
            case PossibleInputs.B:
                return interactionSprite;
            case PossibleInputs.BadOne:
                return badOneSprite;
            default:
                return null;
        }
    }
}
