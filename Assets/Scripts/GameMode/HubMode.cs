using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubMode : GameMode {

    public HubMode(int _nbPlayerMin, int nbPlayerMax) : base(_nbPlayerMin, nbPlayerMax)
    {

    }
    public override void AttributeCamera()
    {
        throw new NotImplementedException();
    }
}
