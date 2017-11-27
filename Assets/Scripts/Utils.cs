using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class Utils {

    static int[] mapTypeMaxValue =
        {
            50,            // CollectableType Evolution3 (Strength)
            50,            // CollectableType Evolution3 (Platformist)
            50,            // CollectableType Evolution4 (Agile)
            50,            // CollectableType Evolution4 (Ghost)
            9999,          // Max points
            3              // Max Keys
        };

    static int[] defaultValueCollectable =
    {
        30,                // CollectableType Evolution3 (Strength)
        50,                // CollectableType Evolution4 (Platformist)
        50,                // CollectableType Evolution4 (Agile)
        50,                // CollectableType Evolution4 (Ghost)
        30,                // points
        1                  // Key
    };

    /*
     * Returns the maximum value for a collectableType
     */
    public static int GetMaxValueForCollectable(CollectableType collectableType)
    {
        return mapTypeMaxValue[(int)collectableType];
    }

    /*
     * Returns true if collectableType is linked to an evolution
     */
    public static bool IsAnEvolutionCollectable(CollectableType collectableType)
    {
        return collectableType == CollectableType.StrengthEvolution1
            || collectableType == CollectableType.PlatformistEvolution1
            || collectableType == CollectableType.AgileEvolution1
            || collectableType == CollectableType.GhostEvolution1;
    }

    public static int GetDefaultCollectableValue(int collectableType)
    {
        return defaultValueCollectable[collectableType];
    }

    public static bool CheckEvolutionAndCollectableTypeCompatibility(CollectableType _collectableType, EvolutionComponent _currentComponent)
    {
        return (_collectableType == CollectableType.AgileEvolution1 && _currentComponent is EvolutionAgile)
            || (_collectableType == CollectableType.PlatformistEvolution1 && _currentComponent is EvolutionPlatformist)
            || (_collectableType == CollectableType.GhostEvolution1 && _currentComponent is EvolutionGhost)
            || (_collectableType == CollectableType.StrengthEvolution1 && _currentComponent is EvolutionStrength);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        //RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        //int n = list.Count;
        //while (n > 1)
        //{
        //    byte[] box = new byte[1];
        //    do provider.GetBytes(box);
        //    while (!(box[0] < n * (Byte.MaxValue / n)));
        //    int k = (box[0] % n);
        //    n--;
        //    T value = list[k];
        //    list[k] = list[n];
        //    list[n] = value;
        //}
    }
}
