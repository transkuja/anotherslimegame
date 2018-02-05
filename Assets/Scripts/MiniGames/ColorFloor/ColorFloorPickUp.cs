using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorFloorPickUpType { Score, ColorArrow, ColorAround, SpeedUp, Bomb, Missile }
public class ColorFloorPickUp : MonoBehaviour {

    public ColorFloorPickUpType pickupType;
	
    public void Collect(int _playerIndex)
    {
        switch (pickupType)
        {
            // Action on pickup
            case ColorFloorPickUpType.Score:
                ColorFloorHandler.ScorePoints(_playerIndex);
                break;
            case ColorFloorPickUpType.ColorArrow:
            case ColorFloorPickUpType.ColorAround:
                ColorFloorHandler.ColorFloorWithPickup(this, _playerIndex);
                break;
            // Stock item on pickup then have to press a button to use it
            case ColorFloorPickUpType.Bomb:
            case ColorFloorPickUpType.Missile:
                break;
            // Get buff on pickup
            case ColorFloorPickUpType.SpeedUp:
                GameManager.EvolutionManager.AddEvolutionComponent(
                    GameManager.Instance.PlayerStart.PlayersReference[_playerIndex], GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Agile)
                );
                break;
        }
    }


}
