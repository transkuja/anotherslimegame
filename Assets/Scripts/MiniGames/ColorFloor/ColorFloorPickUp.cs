using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFloorPickUp : MinigamePickUp {

    [SerializeField]
    float speedBoostDuration = 10.0f;

    [SerializeField]
    float colorArrowRotationDelay = 2.0f;

    IEnumerator Start()
    {
        InitializeDelegates(pickupType);
        if (pickupType == PickUpType.ColorArrow)
        {
            while (true)
            {
                yield return new WaitForSeconds(colorArrowRotationDelay);
                transform.localEulerAngles += Vector3.up * 90;
            }
        }
    }

    void ScorePoints(int _playerIndex)
    {
        ColorFloorHandler.ScorePoints(_playerIndex);
    }

    void ColorFloorWithPickup(int _playerIndex)
    {
        ColorFloorHandler.ColorFloorWithPickup(this, _playerIndex);
    }

    void UseBomb(int _playerIndex)
    {

    }

    void UseMissile(int _playerIndex)
    {

    }

    void UseSpeedUp(int _playerIndex)
    {
        GameManager.EvolutionManager.AddEvolutionComponent(
            GameManager.Instance.PlayerStart.PlayersReference[_playerIndex],
            GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Agile),
            true,
            speedBoostDuration
        );
    }

    protected override void InitializeDelegates(PickUpType _pickupType)
    {
        switch (_pickupType)
        {
            case PickUpType.Score:
            case PickUpType.ColorArrow:
            case PickUpType.ColorAround:
            case PickUpType.SpeedUp:
                collectPickup = InstantUse;
                break;

            default:
                collectPickup = StoreAndUseLater;
                break;
        }

        switch (_pickupType)
        {
            case PickUpType.Score:
                usePickup = ScorePoints;
                break;
            case PickUpType.ColorArrow:
            case PickUpType.ColorAround:
                usePickup = ColorFloorWithPickup;
                break;
            case PickUpType.SpeedUp:
                usePickup = UseSpeedUp;
                break;
            case PickUpType.Missile:
                usePickup = UseMissile;
                break;
            case PickUpType.Bomb:
                usePickup = UseBomb;
                break;
        }

        if (usePickup == null)
            Debug.LogWarning("No use method defined for pickup " + _pickupType);
    }
}
