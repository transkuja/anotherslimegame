using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableTrigger : MonoBehaviour {

    private PlayerCharacterHub _playerCharacterHub;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCharacterHub>() && other.GetComponent<PlayerControllerHub>())
        {
            _playerCharacterHub = other.GetComponent<PlayerCharacterHub>();
            //OnBreakEvent
            if (GetComponent<CollectEvent>() != null)
            {
                GetComponent<CollectEvent>().OnCollectEvent(_playerCharacterHub.GetComponent<Player>());
            }
        }
    }
}
