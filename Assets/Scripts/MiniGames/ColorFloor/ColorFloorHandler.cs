using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorFloorHandler {

    static List<Collider>[] currentlyColoredByPlayer;

    public static void RegisterFloor(int _playerIndex, Collider _toRegister)
    {
        UnregisterFloor(_toRegister);
        if (!currentlyColoredByPlayer[_playerIndex].Contains(_toRegister))
            currentlyColoredByPlayer[_playerIndex].Add(_toRegister);
    }

    static void UnregisterFloor(Collider _toUnregister)
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.ActivePlayersAtStart; i++)
        {
            if (currentlyColoredByPlayer[i].Contains(_toUnregister))
                currentlyColoredByPlayer[i].Remove(_toUnregister);
        }
    }

    public static void ScorePoints(int _playerIndex)
    {
        int scoredPoints = currentlyColoredByPlayer[_playerIndex].Count;
        GameManager.Instance.PlayerStart.PlayersReference[_playerIndex].GetComponent<Player>().UpdateCollectableValue(CollectableType.Points, scoredPoints);

        foreach (Collider col in currentlyColoredByPlayer[_playerIndex])
        {
            col.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
        }

        currentlyColoredByPlayer[_playerIndex].Clear();
    }
}
