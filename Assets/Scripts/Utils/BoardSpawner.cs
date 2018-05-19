using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class BoardSpawner
{

    public static int[] AscendingDiagonalBadPickup(int _startingLine = -1, int _startingColumn = -1)
    {
        if (_startingLine == -1 && _startingColumn == -1)
        {
            int randStart = Random.Range(0, 15);
            _startingLine = Mathf.Min(randStart, 7);
            _startingColumn = Mathf.Max(0, randStart - 7);
        }
        else
        {
            _startingLine = Mathf.Clamp(_startingLine, 0, 7);
            _startingColumn = Mathf.Clamp(_startingColumn, 0, 7);
        }
        int[] result = new int[_startingLine + 1 - _startingColumn];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (_startingLine - i) * 8 + i + _startingColumn;
        }

        return result;
    }

    public static int[] DescendingDiagonalBadPickup(int _startingLine = -1, int _startingColumn = -1)
    {
        if (_startingLine == -1 && _startingColumn == -1)
        {
            int randStart = Random.Range(0, 15);
            _startingLine = (randStart > 7) ? 0 : randStart;
            _startingColumn = Mathf.Max(0, randStart - 7);
        }
        else
        {
            _startingLine = Mathf.Clamp(_startingLine, 0, 7);
            _startingColumn = Mathf.Clamp(_startingColumn, 0, 7);
        }
        int[] result = new int[8 - _startingLine - _startingColumn];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (_startingLine + i) * 8 + i + _startingColumn;
        }

        return result;
    }

    public static int[] LineSpawnBadPickup(int _lineIndex = -1)
    {
        int[] result = new int[8];
        if (_lineIndex == -1)
        {
            _lineIndex = Random.Range(0, 7);
        }

        for (int i = 0; i < 8; i++)
            result[i] = (_lineIndex * 8) + i;

        return result;
    }

    public static int[] ColumnSpawnBadPickup(int _columnIndex = -1)
    {
        int[] result = new int[8];
        if (_columnIndex == -1)
        {
            _columnIndex = Random.Range(0, 7);
        }

        for (int i = 0; i < 8; i++)
            result[i] = _columnIndex + 8 * i;

        return result;
    }
}