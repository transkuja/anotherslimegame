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
        {
            currentlyColoredByPlayer[_playerIndex].Add(_toRegister);
            _toRegister.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            _toRegister.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", GameManager.Instance.PlayerStart.colorPlayer[_playerIndex]);
        }
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

    public static void ColorFloorWithPickup(MinigamePickUp _pickupComponent, int _playerIndex)
    {
        Transform floorPosition = _pickupComponent.transform.parent;
        Transform lineTransform = floorPosition.parent;
        int floorIndex = floorPosition.GetSiblingIndex();
        int lineIndex = lineTransform.GetSiblingIndex();

        if (_pickupComponent.pickupType == PickUpType.ColorAround)
        {
            // Register the 2 sides
            if (floorIndex > 0)
                RegisterFloor(_playerIndex, lineTransform.GetChild(floorIndex - 1).GetComponent<Collider>());
            if (floorIndex < floorPosition.parent.childCount - 1)
                RegisterFloor(_playerIndex, lineTransform.GetChild(floorIndex + 1).GetComponent<Collider>());

            // Register 3 above
            if (lineIndex > 0)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (floorIndex + i >= 0 && (floorIndex + i) <= (lineTransform.parent.GetChild(lineIndex - 1).childCount - 1))
                        RegisterFloor(_playerIndex, lineTransform.parent.GetChild(lineIndex - 1).GetChild(floorIndex + i).GetComponent<Collider>());
                }
            }
            // Register 3 under
            if (lineIndex < floorPosition.parent.parent.childCount - 1)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (floorIndex + i >= 0 && (floorIndex + i) <= (lineTransform.parent.GetChild(lineIndex + 1).childCount - 1))
                        RegisterFloor(_playerIndex, lineTransform.parent.GetChild(lineIndex + 1).GetChild(floorIndex + i).GetComponent<Collider>());
                }
            }

        }
        else if (_pickupComponent.pickupType == PickUpType.ColorArrow)
        {
            Vector3 pickupForward = _pickupComponent.transform.forward;
            bool colorOnX = Utils.Abs(pickupForward.x) > Utils.Abs(pickupForward.z);
            int unit, nextIndex;

            if (colorOnX)
            {
                unit = ((pickupForward.x > 0.0f) ? 1 : -1);
                nextIndex = floorIndex + unit;
                while (nextIndex >= 0 && nextIndex < lineTransform.childCount)
                {
                    RegisterFloor(_playerIndex, lineTransform.GetChild(nextIndex).GetComponent<Collider>());
                    nextIndex += unit;
                }            
            }
            else
            {
                unit = ((pickupForward.z > 0.0f) ? -1 : 1);
                nextIndex = lineIndex + unit;
                
                while (nextIndex >= 0 && nextIndex < lineTransform.parent.childCount)
                {
                    RegisterFloor(_playerIndex, lineTransform.parent.GetChild(nextIndex).GetChild(floorIndex).GetComponent<Collider>());
                    nextIndex += unit;
                }
            }
        }
        else
        {
            Debug.LogWarning(_pickupComponent.pickupType + " is not a proper type for the function ColorFloorWithPickup.");
            return;
        }
    }

}
