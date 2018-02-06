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
        switch (pickupType)
        {
            // Action on pickup
            case PickUpType.Score:
                ColorFloorHandler.ScorePoints(_playerIndex);
                break;
            case PickUpType.ColorArrow:
            case PickUpType.ColorAround:
                ColorFloorHandler.ColorFloorWithPickup(this, _playerIndex);
                break;
            // Stock item on pickup then have to press a button to use it
            case PickUpType.Bomb:
            case PickUpType.Missile:
                break;
            // Get buff on pickup
            case PickUpType.SpeedUp:
                GameManager.EvolutionManager.AddEvolutionComponent(
                    GameManager.Instance.PlayerStart.PlayersReference[_playerIndex], GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Agile), speedBoostDuration
                );
                break;
        }
    }

    public void InstantUse(int _playerIndex)
    {

    }

    public void SetCollectBehaviourForPickup(PickUpType _pickupType)
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

    }
}
