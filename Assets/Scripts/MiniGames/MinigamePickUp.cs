using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType { Score, ColorArrow, ColorAround, SpeedUp, Bomb, Missile, DashBoost, Changer, Aspirator, BadOne, GiantFruit, BallPickup }
/*
 * Each minigame using pickups should have a MyMinigamePickUp inherited from this class with InitializeDelegates overriden.
 */
public class MinigamePickUp : MonoBehaviour {

    public PickUpType pickupType;
    public delegate void Collect(int _playerIndex);
    public delegate void Use(int _playerIndex);

    public Collect collectPickup;
    public Use usePickup;

    public void StoreAndUseLater(int _playerIndex)
    {
        GameManager.Instance.PlayerStart.PlayersReference[_playerIndex].GetComponent<Player>().currentStoredPickup = usePickup;
    }

    public void InstantUse(int _playerIndex)
    {
        usePickup(_playerIndex);
    }

    protected MinigamePickUp() { }

    MinigamePickUp(MinigamePickUp _toCopy)
    {
        pickupType = _toCopy.pickupType;
        collectPickup = _toCopy.collectPickup;
        usePickup = _toCopy.usePickup;
    }

    // Should be overriden in child class then called in child class' Start method
    protected virtual void InitializeDelegates(PickUpType _pickupType) { }
}
