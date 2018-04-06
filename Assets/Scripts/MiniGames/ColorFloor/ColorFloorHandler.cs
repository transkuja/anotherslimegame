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

        if (!currentlyColoredByPlayer[_playerIndex].Contains(_toRegister))
        {
            ColorFloorHandler.UnregisterFloor(_toRegister);
            _toRegister.GetComponent<Animator>().SetBool("animate", true);

            currentlyColoredByPlayer[_playerIndex].Add(_toRegister);
            _toRegister.GetComponentInChildren<MeshRenderer>().material.EnableKeyword("_EMISSION");
            _toRegister.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", GameManager.Instance.PlayerStart.colorPlayer[_playerIndex]);
            _toRegister.GetComponent<OnColoredFloorTrigger>().currentOwner = _playerIndex;
        }

        if (((ColorFloorGameMode)GameManager.Instance.CurrentGameMode).squareToScoreMode)
            SquareDetection(_toRegister.GetComponent<OnColoredFloorTrigger>(), _playerIndex);
    }

    public static void SquareDetection(OnColoredFloorTrigger _lastStepTrigger, int _playerIndex)
    {
        int[][] boardState = new int[8][];
        for (int i = 0; i < 8; i++)
            boardState[i] = new int[8];

        // No loop possible from this point if not at least 2 neighbors of the same color
        if (_lastStepTrigger.SameColorNeighbors() < 2)
            return;

        // No loop possible without at least 8 floors colored
        if (currentlyColoredByPlayer[_playerIndex].Count < 8)
            return;

        List<List<OnColoredFloorTrigger>> paths = new List<List<OnColoredFloorTrigger>>();

        for (int i = 0; i < 4; ++i)
        {
            if (_lastStepTrigger.Neighbors[i] == null)
                continue;

            if (_lastStepTrigger.Neighbors[i].currentOwner == _playerIndex)
            {
                Debug.Log("!");
                List<OnColoredFloorTrigger> newPath = new List<OnColoredFloorTrigger>();
                newPath.Add(_lastStepTrigger);
                newPath.Add(_lastStepTrigger.Neighbors[i]);                
                paths.Add(newPath);
            }
        }

        int nbIteration = 0;
        bool squared = false;

        while (paths.Count > 0 && paths[0].Count < currentlyColoredByPlayer[_playerIndex].Count * 2 && nbIteration < 1000 && !squared) // /!\
        {
            OnColoredFloorTrigger _currentTrigger;
            List<List<OnColoredFloorTrigger>> newPaths = new List<List<OnColoredFloorTrigger>>();
            
            foreach (List<OnColoredFloorTrigger> path in paths)
            {
                _currentTrigger = path[path.Count - 1];
                Debug.Log("current:" + _currentTrigger.GetFloorIndex());

                if (_currentTrigger.SameColorNeighbors() < 2)
                {
                    Debug.Log("moins de 2 voisins");
                    continue;
                }

                //if (path.Contains(_currentTrigger))
                //{
                //    continue;
                //}

                for (int i = 0; i < 4; ++i)
                {
                    if (_currentTrigger.Neighbors[i] == null || _currentTrigger.Neighbors[i] == path[path.Count - 2])
                        continue;

                    if (_currentTrigger == _lastStepTrigger)
                    {
                        squared = true;
                        newPaths.Clear();
                        newPaths.Add(path);
                        break;
                    }

                    if (_currentTrigger.Neighbors[i].currentOwner == _playerIndex)
                    {
                        List<OnColoredFloorTrigger> newPath = new List<OnColoredFloorTrigger>();
                        newPath.AddRange(path);
                        newPath.Add(_currentTrigger.Neighbors[i]);
                        newPaths.Add(newPath);
                    }
                }
            }

            foreach (List<OnColoredFloorTrigger> p in newPaths)
            {
                string debugstr = "";
                foreach (OnColoredFloorTrigger f in p)
                    debugstr += f.GetFloorIndex() + ",";
                Debug.Log(debugstr);
            }

            paths.Clear();
            paths.AddRange(newPaths);
            nbIteration++;
        }

        // Here, paths[0] should contain the squared path
        if (squared)
            ScorePoints(_playerIndex);           

    }

    // Unregister a floor from any player
    static void UnregisterFloor(Collider _toUnregister)
    {
        for (int i = 0; i < currentlyColoredByPlayer.Length; i++)
        {
            if (currentlyColoredByPlayer[i].Contains(_toUnregister))
                currentlyColoredByPlayer[i].Remove(_toUnregister);
        }

        _toUnregister.GetComponent<OnColoredFloorTrigger>().currentOwner = -1;
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
            col.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
            col.GetComponent<OnColoredFloorTrigger>().currentOwner = -1;
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
                RegisterFloor(_playerIndex, lineTransform.GetChild(floorIndex - 1).GetComponentInChildren<Collider>());
            if (floorIndex < floorPosition.parent.childCount - 1)
                RegisterFloor(_playerIndex, lineTransform.GetChild(floorIndex + 1).GetComponentInChildren<Collider>());

            // Register 3 above
            if (lineIndex > 0)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (floorIndex + i >= 0 && (floorIndex + i) <= (lineTransform.parent.GetChild(lineIndex - 1).childCount - 1))
                        RegisterFloor(_playerIndex, lineTransform.parent.GetChild(lineIndex - 1).GetChild(floorIndex + i).GetComponentInChildren<Collider>());
                }
            }
            // Register 3 under
            if (lineIndex < floorPosition.parent.parent.childCount - 1)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (floorIndex + i >= 0 && (floorIndex + i) <= (lineTransform.parent.GetChild(lineIndex + 1).childCount - 1))
                        RegisterFloor(_playerIndex, lineTransform.parent.GetChild(lineIndex + 1).GetChild(floorIndex + i).GetComponentInChildren<Collider>());
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
                    RegisterFloor(_playerIndex, lineTransform.GetChild(nextIndex).GetComponentInChildren<Collider>());
                    nextIndex += unit;
                }            
            }
            else
            {
                unit = ((pickupForward.z > 0.0f) ? -1 : 1);
                nextIndex = lineIndex + unit;
                
                while (nextIndex >= 0 && nextIndex < lineTransform.parent.childCount)
                {
                    RegisterFloor(_playerIndex, lineTransform.parent.GetChild(nextIndex).GetChild(floorIndex).GetComponentInChildren<Collider>());
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
