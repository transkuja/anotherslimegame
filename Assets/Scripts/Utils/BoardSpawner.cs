using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class BoardSpawner
{
    public enum Pattern {
        CenteredSquare4x4,
        Corners2x2,
        Corners3x3,
        Corners3x3WithCenteredSquare4x4,
        Corners2x2WithCenter2x2,
        Borders1,
        Borders2,
        Losange,
        LosangeWithCross,
        X,
        Plus,
        HappyFace,
        SquareEmptySquare1,
        SquareEmptySquare2,
        Corners,
        Center4x4,
        Center2x2,
        SmallH,
        BigH,
        RotatedSmallH,
        RotatedBigH,
        Size
    }
    public static int[] AscendingDiagonal(int _startingLine = -1, int _startingColumn = -1)
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

    public static int[] DescendingDiagonal(int _startingLine = -1, int _startingColumn = -1)
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

    public static int[] LineSpawn(int _lineIndex = -1, int _lineStartingPoint = 0, int _lineEndPoint = 7)
    {
        int[] result = new int[_lineEndPoint - _lineStartingPoint + 1];
        if (_lineIndex == -1)
        {
            _lineIndex = Random.Range(0, 7);
        }

        for (int i = 0; i < result.Length; i++)
            result[i] = (_lineIndex * 8) + i + _lineStartingPoint;

        return result;
    }

    public static int[] ColumnSpawn(int _columnIndex = -1, int _columnStartingPoint = 0, int _columnEndPoint = 7)
    {
        int[] result = new int[_columnEndPoint - _columnStartingPoint + 1];
        if (_columnIndex == -1)
        {
            _columnIndex = Random.Range(0, 7);
        }

        for (int i = 0; i < result.Length; i++)
            result[i] = _columnIndex + 8 * (i + _columnStartingPoint);

        return result;
    }

    public static int[] GetPattern(Pattern _pattern)
    {
        switch (_pattern)
        {
            case Pattern.Borders1:
                return Borders1();
            case Pattern.Borders2:
                return Borders2();
            case Pattern.Center2x2:
                return Square(3,3,2);
            case Pattern.Center4x4:
                return Center4x4();
            case Pattern.Corners2x2WithCenter2x2:
                return Corners2x2WithCenter2x2();
            case Pattern.Corners:
                return Corners();
            case Pattern.Corners2x2:
                return Corners2x2();
            case Pattern.Corners3x3:
                return Corners3x3();
            case Pattern.Corners3x3WithCenteredSquare4x4:
                return Corners3x3WithCenteredSquare4x4();
            case Pattern.HappyFace:
                return HappyFace();
            case Pattern.Losange:
                return Losange();
            case Pattern.LosangeWithCross:
                return LosangeWithCross();
            case Pattern.Plus:
                return Plus();
            case Pattern.SquareEmptySquare1:
                return SquareEmptySquare1();
            case Pattern.SquareEmptySquare2:
                return SquareEmptySquare2();
            case Pattern.X:
                return X();
            case Pattern.SmallH:
                return SmallH();
            case Pattern.RotatedSmallH:
                return RotatedSmallH();
            case Pattern.BigH:
                return BigH();
            case Pattern.RotatedBigH:
                return RotatedBigH();
            default:
                return ColumnSpawn();
        }
        
    }

    static int[] Square(int _startLine, int _startColumn, int _sideLength)
    {
        List<int> result = new List<int>();
        result.AddRange(LineSpawn(_startLine, _startColumn, _startColumn + _sideLength - 1));
        result.AddRange(LineSpawn(_startLine + _sideLength - 1, _startColumn, _startColumn + _sideLength - 1));
        if (_sideLength > 2)
        {
            result.AddRange(ColumnSpawn(_startColumn, _startLine + 1, _startLine + _sideLength - 2));
            result.AddRange(ColumnSpawn(_startColumn + _sideLength - 1, _startLine + 1, _startLine + _sideLength - 2));
        }

        return result.ToArray();
    }

    static int[] Borders1()
    {
        List<int> result = new List<int>();
        result.AddRange(Square(0,0,8));

        return result.ToArray();      
    }

    static int[] Borders2()
    {
        List<int> result = new List<int>();
        result.AddRange(Square(0, 0, 8));
        result.AddRange(Square(1, 1, 6));

        return result.ToArray();
    }

    static int[] Center4x4()
    {
        List<int> result = new List<int>();
        result.AddRange(Square(3, 3, 2));
        result.AddRange(Square(2, 2, 4));

        return result.ToArray();
    }

    static int[] Corners2x2WithCenter2x2()
    {
        List<int> result = new List<int>();
        result.AddRange(Square(3, 3, 2));
        result.AddRange(Corners2x2());

        return result.ToArray();
    }

    static int[] Corners2x2()
    {
        List<int> result = new List<int>();
        result.AddRange(Square(0, 0, 2));
        result.AddRange(Square(6, 0, 2));
        result.AddRange(Square(0, 6, 2));
        result.AddRange(Square(6, 6, 2));

        return result.ToArray();
    }

    static int[] Corners()
    {
        return new int[12] {0, 1, 8,
                            6, 7, 15,
                            48, 56, 57,
                            55, 62, 63 };
    }

    static int[] Corners3x3()
    {
        List<int> result = new List<int>();
        result.AddRange(Square(0, 0, 3));
        result.AddRange(Square(5, 0, 3));
        result.AddRange(Square(0, 5, 3));
        result.AddRange(Square(5, 5, 3));

        return result.ToArray();
    }

    static int[] Corners3x3WithCenteredSquare4x4()
    {
        List<int> result = new List<int>();
        result.AddRange(Corners3x3());
        result.AddRange(LineSpawn(2, 3, 4));
        result.AddRange(LineSpawn(5, 3, 4));
        result.AddRange(ColumnSpawn(2, 3, 4));
        result.AddRange(ColumnSpawn(5, 3, 4));

        return result.ToArray();
    }

    static int[] SquareEmptySquare1()
    {
        List<int> result = new List<int>();
        result.AddRange(Square(3, 3, 2));
        result.AddRange(Square(1, 1, 6));

        return result.ToArray();
    }

    static int[] SquareEmptySquare2()
    {
        List<int> result = new List<int>();
        result.AddRange(Borders1());
        result.AddRange(Square(2, 2, 4));

        return result.ToArray();
    }

    static int[] Plus()
    {
        List<int> result = new List<int>();
        result.AddRange(ColumnSpawn(3));
        result.AddRange(ColumnSpawn(4));
        result.AddRange(LineSpawn(3, 0, 2));
        result.AddRange(LineSpawn(4, 0, 2));
        result.AddRange(LineSpawn(3, 5, 7));
        result.AddRange(LineSpawn(4, 5, 7));

        return result.ToArray();
    }

    static int[] X()
    {
        List<int> result = new List<int>();
        result.AddRange(DescendingDiagonal(0, 0));
        result.AddRange(AscendingDiagonal(7, 0));

        return result.ToArray();
    }

    static int[] HappyFace()
    {
        List<int> result = new List<int>();
        result.AddRange(ColumnSpawn(2, 1, 2));
        result.AddRange(ColumnSpawn(5, 1, 2));
        result.AddRange(LineSpawn(4, 1, 6));
        result.AddRange(LineSpawn(6, 2, 5));
        result.Add(41);
        result.Add(46);

        return result.ToArray();
    }

    static int[] Losange()
    {
        List<int> result = new List<int>();
        result.AddRange(AscendingDiagonal(3, 0));
        result.AddRange(AscendingDiagonal(7, 4));
        result.AddRange(DescendingDiagonal(0, 4));
        result.AddRange(DescendingDiagonal(4, 0));

        return result.ToArray();
    }

    static int[] LosangeWithCross()
    {
        List<int> result = new List<int>();
        result.AddRange(Losange());
        result.AddRange(X());
        
        return result.ToArray();
    }

    static int[] SmallH()
    {
        List<int> result = new List<int>();
        result.AddRange(Corners());
        result.AddRange(ColumnSpawn(2, 2, 5));
        result.AddRange(ColumnSpawn(5, 2, 5));
        result.AddRange(Square(3, 3, 2));

        return result.ToArray();
    }

    static int[] RotatedSmallH()
    {
        List<int> result = new List<int>();
        result.AddRange(Corners());
        result.AddRange(LineSpawn(2, 2, 5));
        result.AddRange(LineSpawn(5, 2, 5));
        result.AddRange(Square(3, 3, 2));

        return result.ToArray();
    }

    static int[] BigH()
    {
        List<int> result = new List<int>();
        result.AddRange(SmallH());
        result.AddRange(ColumnSpawn(1, 1, 6));
        result.AddRange(ColumnSpawn(6, 1, 6));

        return result.ToArray();
    }

    static int[] RotatedBigH()
    {
        List<int> result = new List<int>();
        result.AddRange(RotatedSmallH());
        result.AddRange(LineSpawn(1, 1, 6));
        result.AddRange(LineSpawn(6, 1, 6));

        return result.ToArray();
    }
}