using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakEventPot : BreakEvent {

    public override void OnBreakEvent(Player playerTarget)
    {     
        SpawnChief();
    }

    void SpawnChief()
    {
        GetComponentInChildren<SneakyChiefPot>().sneakyChiefReference.SetActive(true);
    }
}
