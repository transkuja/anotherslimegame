using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorFloorHandler {

    static List<OnColoredFloorTrigger>[] currentlyColoredByPlayer;
    static List<int> currentlyColoredByPlayerToInt;
    static List<OnColoredFloorTrigger> pendingUnregistration = new List<OnColoredFloorTrigger>();
    static OnColoredFloorTrigger[][] board = new OnColoredFloorTrigger[8][];

    static bool isInitialized = false;

    public static void Init(uint _nbPlayers, GameObject _board)
    {
        currentlyColoredByPlayer = new List<OnColoredFloorTrigger>[_nbPlayers];
        for (int i = 0; i < currentlyColoredByPlayer.Length; i++)
            currentlyColoredByPlayer[i] = new List<OnColoredFloorTrigger>();

        for (int i = 0; i < 8; i++)
            board[i] = new OnColoredFloorTrigger[8];

        Transform boardTr = _board.transform;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i][j] = boardTr.GetChild(i).GetChild(j).GetComponent<OnColoredFloorTrigger>();
            }
        }

        isInitialized = true;
    }

    // Called when a player steps on a floor
    public static void RegisterFloor(int _playerIndex, OnColoredFloorTrigger _toRegister, bool bypassSquareDetection = false, MinigamePickUp _fromAPickup = null)
    {
        if (!isInitialized)
            return;

        if (_toRegister.IsLocked())
            return;

        if (!currentlyColoredByPlayer[_playerIndex].Contains(_toRegister))
        {
            UnregisterFloor(_toRegister);
            if (!bypassSquareDetection)
                _toRegister.GetComponent<Animator>().SetBool("animate", true);

            currentlyColoredByPlayer[_playerIndex].Add(_toRegister);
            _toRegister.GetComponentInChildren<MeshRenderer>().material.EnableKeyword("_EMISSION");
            _toRegister.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", GameManager.Instance.PlayerStart.colorPlayer[_playerIndex]);
            _toRegister.currentOwner = _playerIndex;


            if (((ColorFloorGameMode)GameManager.Instance.CurrentGameMode).squareToScoreMode && !bypassSquareDetection)
            {
                if (SquareDetection(_toRegister.GetComponent<OnColoredFloorTrigger>(), _playerIndex))
                {
                    ScorePoints(_playerIndex);
                    if (_fromAPickup != null)
                        RegisterFloor(_playerIndex, _fromAPickup.GetComponentInParent<OnColoredFloorTrigger>(), true);
                    else
                        RegisterFloor(_playerIndex, _toRegister, true);
                }
            }
        }

    }

    public static void RegisterAll(int _playerIndex, List<OnColoredFloorTrigger> _toRegister, OnColoredFloorTrigger _pickupOrigin, bool bypassSquareDetection = false)
    {
        if (!isInitialized)
            return;

        foreach (OnColoredFloorTrigger c in _toRegister)
        {
            if (!currentlyColoredByPlayer[_playerIndex].Contains(c))
            {
                UnregisterFloor(c);
                c.GetComponent<Animator>().SetBool("animate", true);

                currentlyColoredByPlayer[_playerIndex].Add(c);
                c.GetComponentInChildren<MeshRenderer>().material.EnableKeyword("_EMISSION");
                c.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", GameManager.Instance.PlayerStart.colorPlayer[_playerIndex]);
                c.currentOwner = _playerIndex;
            }
        }

        if (((ColorFloorGameMode)GameManager.Instance.CurrentGameMode).squareToScoreMode && !bypassSquareDetection)
        {
            int nbrOfSquares = 0;
            foreach (OnColoredFloorTrigger c in _toRegister)
            {
               if (SquareDetection(c, _playerIndex))
               {
                    nbrOfSquares++;
                }
            }

            if (nbrOfSquares > 0)
            {
                ScorePoints(_playerIndex);
                RegisterFloor(_playerIndex, _pickupOrigin, true);
            }
        }
    }

    public static void RegisterFloor(int _playerIndex, int _toRegister)
    {
        RegisterFloor(_playerIndex, board[_toRegister / 8][_toRegister % 8], true);
    }

    public static bool SquareDetection(OnColoredFloorTrigger _lastStepTrigger, int _playerIndex)
    {
        // No loop possible from this point if not at least 2 neighbors of the same color
        if (_lastStepTrigger.SameColorNeighbors() < 2 || _lastStepTrigger.SameColorNeighbors() == 4)
            return false;

        // No loop possible without at least 8 floors colored
        if (currentlyColoredByPlayer[_playerIndex].Count < 8)
            return false;

        List<List<OnColoredFloorTrigger>> paths = new List<List<OnColoredFloorTrigger>>();

        for (int i = 0; i < 4; ++i)
        {
            if (_lastStepTrigger.Neighbors[i] == null)
                continue;

            if (_lastStepTrigger.Neighbors[i].currentOwner == _playerIndex)
            {
                List<OnColoredFloorTrigger> newPath = new List<OnColoredFloorTrigger>();
                newPath.Add(_lastStepTrigger);
                newPath.Add(_lastStepTrigger.Neighbors[i]);                
                paths.Add(newPath);
            }
        }

        int nbIteration = 0;
        bool squared = false;

        while (paths.Count > 0 && paths[0].Count < currentlyColoredByPlayer[_playerIndex].Count * 2 && !squared) // /!\
        {
            OnColoredFloorTrigger _currentTrigger;
            List<List<OnColoredFloorTrigger>> newPaths = new List<List<OnColoredFloorTrigger>>();
            
            foreach (List<OnColoredFloorTrigger> path in paths)
            {
                _currentTrigger = path[path.Count - 1];
                if (_currentTrigger.SameColorNeighbors() < 2)
                {
                    continue;
                }

                List<OnColoredFloorTrigger> check = new List<OnColoredFloorTrigger>();
                check.AddRange(path);
                check.Remove(_currentTrigger);

                if (check.Contains(_currentTrigger))
                {
                    if (_currentTrigger == _lastStepTrigger)
                    {
                        if (path.Count >= 8)
                        {
                            // Check path validity
                            if (!CheckPathValidity(path, _playerIndex))
                                continue;

                            squared = true;
                            newPaths.Clear();
                            newPaths.Add(path);
                            break;
                        }
                        else
                            continue;
                    }
                    else
                        continue;
                }

                for (int i = 0; i < 4; ++i)
                {
                    if (_currentTrigger.Neighbors[i] == null || _currentTrigger.Neighbors[i] == path[path.Count - 2])
                        continue;

                    if (_currentTrigger.Neighbors[i].currentOwner == _playerIndex)
                    {
                        List<OnColoredFloorTrigger> newPath = new List<OnColoredFloorTrigger>();
                        newPath.AddRange(path);
                        newPath.Add(_currentTrigger.Neighbors[i]);
                        newPaths.Add(newPath);
                    }
                }
            }


            paths.Clear();
            paths.AddRange(newPaths);
            nbIteration++;
        }

        return squared;
    }

    private static bool CheckPathValidity(List<OnColoredFloorTrigger> path, int _playerIndex)
    {
        int nbEdges = 0;
        List<int>[] intPath = new List<int>[8];
        bool hasAHole = false;

        currentlyColoredByPlayerToInt = new List<int>();
        foreach (OnColoredFloorTrigger c in currentlyColoredByPlayer[_playerIndex])
            currentlyColoredByPlayerToInt.Add(c.GetFloorIndex());

        foreach (OnColoredFloorTrigger f in path)
        {
            int floorIndex = f.GetFloorIndex();
            int line = (floorIndex / 8);
            if (intPath[line] == null)
                intPath[line] = new List<int>();

            if (!intPath[line].Contains(floorIndex))
                intPath[line].Add(floorIndex);
        }

        for (int i = 0; i < 8; i++)
        {
            if (intPath[i] != null)
            {
                intPath[i].Sort();
                   
                if (intPath[i].Count == intPath[i][intPath[i].Count - 1] - intPath[i][0] + 1)
                {
                    ++nbEdges;
                }
                else
                {
                    for (int k = intPath[i][0]; k < intPath[i][intPath[i].Count - 1]; ++k)
                    {
                        if (!currentlyColoredByPlayerToInt.Contains(k))
                        {
                            hasAHole = true;
                            RegisterFloor(_playerIndex, k);
                        }
                    }
                }
            }
        }

        if (nbEdges < 2)
            return false;
        if (hasAHole)
            return true;
        return false;
    }

    // Unregister a floor from any player
    static void UnregisterFloor(OnColoredFloorTrigger _toUnregister)
    {
        for (int i = 0; i < currentlyColoredByPlayer.Length; i++)
        {
            if (currentlyColoredByPlayer[i].Contains(_toUnregister))
            {
                _toUnregister.currentOwner = -1;
                currentlyColoredByPlayer[i].Remove(_toUnregister);
            }
        }
    }

    // Score points event, should be called when conditions to score points are met
    public static void ScorePoints(int _playerIndex)
    {
        if (!isInitialized)
            return;

        int scoredPoints = currentlyColoredByPlayer[_playerIndex].Count;
        GameManager.Instance.PlayerStart.PlayersReference[_playerIndex].GetComponent<Player>().UpdateCollectableValue(CollectableType.Points, scoredPoints);

        // Standard case
        foreach (OnColoredFloorTrigger col in currentlyColoredByPlayer[_playerIndex])
        {
            //col.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
            col.ScoreFromThisFloor();
        }

        currentlyColoredByPlayer[_playerIndex].Clear();
    }


    public static void ColorFloorWithPickup(MinigamePickUp _pickupComponent, int _playerIndex)
    {
        Transform floorPosition = _pickupComponent.transform.parent;
        Transform lineTransform = floorPosition.parent;
        int floorIndex = floorPosition.GetSiblingIndex();
        int lineIndex = lineTransform.GetSiblingIndex();
        List<OnColoredFloorTrigger> pendingRegistration = new List<OnColoredFloorTrigger>();
        pendingRegistration.Add(_pickupComponent.GetComponentInParent<OnColoredFloorTrigger>());

        if (_pickupComponent.pickupType == PickUpType.ColorAround)
        {
            // Register the 2 sides
            if (floorIndex > 0)
                pendingRegistration.Add(lineTransform.GetChild(floorIndex - 1).GetComponent<OnColoredFloorTrigger>());
            if (floorIndex < floorPosition.parent.childCount - 1)
                pendingRegistration.Add(lineTransform.GetChild(floorIndex + 1).GetComponent<OnColoredFloorTrigger>());

            // Register 3 above
            if (lineIndex > 0)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (floorIndex + i >= 0 && (floorIndex + i) <= (lineTransform.parent.GetChild(lineIndex - 1).childCount - 1))
                        pendingRegistration.Add(lineTransform.parent.GetChild(lineIndex - 1).GetChild(floorIndex + i).GetComponent<OnColoredFloorTrigger>());
                }
            }
            // Register 3 under
            if (lineIndex < floorPosition.parent.parent.childCount - 1)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (floorIndex + i >= 0 && (floorIndex + i) <= (lineTransform.parent.GetChild(lineIndex + 1).childCount - 1))
                        pendingRegistration.Add(lineTransform.parent.GetChild(lineIndex + 1).GetChild(floorIndex + i).GetComponent<OnColoredFloorTrigger>());
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
                    pendingRegistration.Add(lineTransform.GetChild(nextIndex).GetComponent<OnColoredFloorTrigger>());
                    nextIndex += unit;
                }            
            }
            else
            {
                unit = ((pickupForward.z > 0.0f) ? -1 : 1);
                nextIndex = lineIndex + unit;
                
                while (nextIndex >= 0 && nextIndex < lineTransform.parent.childCount)
                {
                    pendingRegistration.Add(lineTransform.parent.GetChild(nextIndex).GetChild(floorIndex).GetComponent<OnColoredFloorTrigger>());
                    nextIndex += unit;
                }
            }
        }
        else
        {
            Debug.LogWarning(_pickupComponent.pickupType + " is not a proper type for the function ColorFloorWithPickup.");
            return;
        }

        if (pendingRegistration.Count > 0)
        {
            RegisterAll(_playerIndex, pendingRegistration, _pickupComponent.GetComponentInParent<OnColoredFloorTrigger>());
        }
    }

}
