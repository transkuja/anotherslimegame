using System.Collections;
using System.Collections.Generic;

public class Utils {

    static int[] mapTypeMaxValue =
        {
            100,           // CollectableType Evolution1
            50,            // CollectableType Evolution2
            9999           // Max points
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
        return collectableType == CollectableType.Evolution1
            || collectableType == CollectableType.Evolution2;
    }
}
