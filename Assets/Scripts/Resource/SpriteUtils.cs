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
    public Sprite Strength;
    [SerializeField]
    public Sprite Ghost;
    [SerializeField]
    public Sprite Agile;
    [SerializeField]
    public Sprite Platformist;

    [Header("Controls sprites")]
    public Sprite leftThumbstickSprite;
    public Sprite AButtonSprite;
    public Sprite BButtonSprite;
    public Sprite XButtonSprite;
    public Sprite YButtonSprite;
    public Sprite rightTriggerSprite;
    public Sprite leftTriggerSprite;
    public Sprite rightShoulderSprite;

    [Header("Keyboard sprites")]
    public Sprite keyboardZQSD;
    public Sprite keyboardQD;

    public Sprite keyboardZ;
    public Sprite keyboardQ;
    public Sprite keyboardS;
    public Sprite keyboardD;

    public Sprite keyboardE;
    public Sprite keyboardA;
    public Sprite keyboardR;
    public Sprite keyboardT;
    public Sprite keyboardSpaceBar;

    public Sprite leftMouse;
    public Sprite rightMouse;
    public Sprite middleMouse;


    [Header("GP related")]
    public Sprite badOneSprite;

    public Sprite GetKeyboardControlSprite(KeyboardControlType _key)
    {
        Sprite result = null;
        switch (_key)
        {
            case KeyboardControlType.Move:
                result = keyboardZQSD;
                break;
            case KeyboardControlType.Jump:
                result = keyboardSpaceBar;
                break;
            case KeyboardControlType.Dash:
                result = leftMouse;
                break;
            case KeyboardControlType.SpawnPl:
            case KeyboardControlType.Stomp:
                result = rightMouse;
                break;
            case KeyboardControlType.Accelerate:
                result = keyboardZ;
                break;
            case KeyboardControlType.ChangePatternPl:
                result = middleMouse;
                break;
            case KeyboardControlType.Reverse:
                result = keyboardS;
                break;
            case KeyboardControlType.TurnKart:
                result = keyboardQD;
                break;
            case KeyboardControlType.NextDialog:
                result = keyboardE;
                break;
            default:
                return null;
        }

        if (result == null)
            Debug.LogWarning("No specified sprite for control type " + _key);

        return result;
    }

    public Sprite GetControlSprite(ControlType _type)
    {
        Sprite result = null;
        switch(_type)
        {
            case ControlType.A:
                result = AButtonSprite;
                break;
            case ControlType.B:
                result = BButtonSprite;
                break;
            case ControlType.X:
                result = XButtonSprite;
                break;
            case ControlType.Y:
                result = YButtonSprite;
                break;
            case ControlType.LeftThumbstick:
                result = leftThumbstickSprite;
                break;
            case ControlType.RightTrigger:
                result = rightTriggerSprite;
                break;
            case ControlType.LeftTrigger:
                result = leftTriggerSprite;
                break;
            case ControlType.RightShoulder:
                result = rightShoulderSprite;
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
                return AButtonSprite;
            case PossibleInputs.B:
                return BButtonSprite;
            case PossibleInputs.X:
                return XButtonSprite;
            case PossibleInputs.Y:
                return YButtonSprite;
            case PossibleInputs.BadOne:
                return badOneSprite;
            default:
                return null;
        }
    }
}
