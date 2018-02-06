using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUtils : MonoBehaviour {

    [Header("Victory panel")]
    [SerializeField]
    Sprite victoryPanel_1;
    [SerializeField]
    Sprite victoryPanel_2;
    [SerializeField]
    Sprite victoryPanel_3;
    [SerializeField]
    Sprite victoryPanel_4;
    [SerializeField]
    Sprite victoryNumber1;
    [SerializeField]
    Sprite victoryNumber2;
    [SerializeField]
    Sprite victoryNumber3;
    [SerializeField]
    Sprite victoryNumber4;

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
    public Sprite rightTriggerSprite;

    public Sprite GetControlSprite(ControlType _type)
    {
        Sprite result = null;
        switch(_type)
        {
            case ControlType.Action:
                result = actionSprite;
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
            default:
                return null;
        }

        if (result == null)
            Debug.LogWarning("No specified sprite for control type " + _type);

        return result;
    }

    
}
