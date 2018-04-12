using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakEventPot : BreakEvent {

    public override void OnBreakEvent()
    {     
        SpawnChief();
    }

    void SpawnChief()
    {
        GetComponentInChildren<SneakyChiefPot>().sneakyChiefReference.SetActive(true);
    }
}
