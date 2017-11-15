using System.Collections;
using System.Collections.Generic;

public class Utils {

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

}
