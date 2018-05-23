using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakableForPotInHub : Breakable {
    [SerializeField]
    GameObject rabbit;

    public override bool OverrideDestruction(PlayerCharacterHub _playerCharacterHub)
    {
        // Est ce que les lapins peuvent peter des pots
        if (_playerCharacterHub.GetComponent<EnnemyController>())
            return true;
        return false;
    }

    public override void PostCollisionProcess(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub)
    {
        int rand = Random.Range(0, 100);
        if( rand < 10)
            Invoke("DelayedSummonRabbit", 0.3f);
    }

    void DelayedSummonRabbit()
    {
        GameObject aaa = Instantiate(rabbit, transform.position + Vector3.up, Quaternion.identity);
        aaa.GetComponent<EnnemyController>().isSpawned = true;
    }
}
