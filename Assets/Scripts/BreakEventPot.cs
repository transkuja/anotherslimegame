﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakEventPot : CollectEvent {

    public override void OnCollectEvent(Player playerTarget)
    {     
    }

    public void SpawnChief()
    {
        GetComponentInChildren<SneakyChiefPot>().sneakyChiefReference.SetActive(true);
    }
}
