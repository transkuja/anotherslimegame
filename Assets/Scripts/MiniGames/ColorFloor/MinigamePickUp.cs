using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType { Score, ColorArrow, ColorAround, SpeedUp, Bomb, Missile }
public class MinigamePickUp : MonoBehaviour {

    [SerializeField]
    float speedBoostDuration = 10.0f;

    [SerializeField]
    float colorArrowRotationDelay = 2.0f;

    public PickUpType pickupType;
    public delegate void Collect(int _playerIndex);
    public delegate void Use(int _playerIndex);

    public Collect collectPickup;
    public Use usePickup;

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

    public void StoreAndUseLater(int _playerIndex)
    {
        GameManager.Instance.PlayerStart.PlayersReference[_playerIndex].GetComponent<Player>().currentStoredPickup = this;
    }

    public void InstantUse(int _playerIndex)
    {
        usePickup(_playerIndex);
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

    void InitializeDelegates(PickUpType _pickupType)
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
