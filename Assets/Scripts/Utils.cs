using System.Collections;
using System.Collections.Generic;

public class Utils {

    static int[] mapTypeValue =
        {
            10,           // CollectableType Evolution1
            50             // CollectableType Evolution2
        };

    public static int GetMaxValueForCollectable(CollectableType collectableType)
    {
        return mapTypeValue[(int)collectableType];
    }
}
