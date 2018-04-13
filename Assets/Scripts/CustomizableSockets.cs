using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizableSockets : MonoBehaviour {
    public Transform[] sockets = new Transform[(int)CustomizableType.Size-2];
    
    public Transform GetSocket(CustomizableType type)
    {
        return sockets[(int)type - 2];
    }
}
