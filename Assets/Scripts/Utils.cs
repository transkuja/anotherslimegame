using System.Collections;
using System.Collections.Generic;

public class Utils {

    static int[] mapTypeMaxValue =
        {
            100,           // CollectableType Evolution1 (Wings 1)
            50,            // CollectableType Evolution2 (Wings 2)
            50,            // CollectableType Evolution3 (Strength)
            50,            // CollectableType Evolution3 (Platformist)
            50,            // CollectableType Evolution4 (Agile)
            9999,          // Max points
            3              // Max Keys
        };

    static int[] defaultValueCollectable =
    {
        30,                // CollectableType Evolution1 (Wings 1)
        30,                // CollectableType Evolution2 (Wings 2)
        30,                // CollectableType Evolution3 (Strength)
        50,                // CollectableType Evolution4 (Platformist)
        50,                // CollectableType Evolution4 (Agile)
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
        return collectableType == CollectableType.WingsEvolution1
            || collectableType == CollectableType.WingsEvolution2
            || collectableType == CollectableType.StrengthEvolution1
            || collectableType == CollectableType.PlatformistEvolution1
            || collectableType == CollectableType.AgileEvolution1;
    }

    public static int GetDefaultCollectableValue(int collectableType)
    {
        return defaultValueCollectable[collectableType];
    }

}
