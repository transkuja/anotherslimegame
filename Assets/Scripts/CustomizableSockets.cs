using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizableSockets : MonoBehaviour {
    public Transform[] sockets;

    public Transform GetSocket(CustomizableType type)
    {
        Transform toReturn = null;
        try
        {
            toReturn = sockets[(int)type - 2];
        }
        catch (System.Exception e)
        {
            toReturn =  null;
        }

        return toReturn;
    }
}
