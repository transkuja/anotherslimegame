using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorFloorHandler {

    static List<Collider>[] currentlyColoredByPlayer;
    static bool isInitialized = false;

    public static void Init(uint _nbPlayers)
    {
        currentlyColoredByPlayer = new List<Collider>[_nbPlayers];
        for (int i = 0; i < currentlyColoredByPlayer.Length; i++)
            currentlyColoredByPlayer[i] = new List<Collider>();

        isInitialized = true;
    }

    // Called when a player steps on a floor
    public static void RegisterFloor(int _playerIndex, Collider _toRegister)
    {
        if (!isInitialized)
            return;

        ColorFloorHandler.UnregisterFloor(_toRegister);
        if (!currentlyColoredByPlayer[_playerIndex].Contains(_toRegister))
            currentlyColoredByPlayer[_playerIndex].Add(_toRegister);
    }

    // Unregister a floor from any player
    static void UnregisterFloor(Collider _toUnregister)
    {
        for (int i = 0; i < currentlyColoredByPlayer.Length; i++)
        {
            if (currentlyColoredByPlayer[i].Contains(_toUnregister))
                currentlyColoredByPlayer[i].Remove(_toUnregister);
        }
    }

    // Score points event, should be called when conditions to score points are met
    public static void ScorePoints(int _playerIndex)
    {
        if (!isInitialized)
            return;

        int scoredPoints = currentlyColoredByPlayer[_playerIndex].Count;
        GameManager.Instance.PlayerStart.PlayersReference[_playerIndex].GetComponent<Player>().UpdateCollectableValue(CollectableType.Points, scoredPoints);

        foreach (Collider col in currentlyColoredByPlayer[_playerIndex])
        {
            col.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
        }

        currentlyColoredByPlayer[_playerIndex].Clear();
    }

    public static void ColorFloorWithPickup(ColorFloorPickUp _pickupComponent)
    {
        if (_pickupComponent.pickupType == ColorFloorPickUpType.ColorAround)
        {

        }
        else if (_pickupComponent.pickupType == ColorFloorPickUpType.ColorArrow)
        {

        }
        else
        {
            Debug.LogWarning(_pickupComponent.pickupType + " is not a proper type for the function ColorFloorWithPickup.");
            return;
        }
    }
}
