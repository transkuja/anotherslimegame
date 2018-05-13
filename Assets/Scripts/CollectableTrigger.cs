using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableTrigger : MonoBehaviour {

    private PlayerCharacterHub _playerCharacterHub;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCharacterHub>())
        {
            _playerCharacterHub = other.GetComponent<PlayerCharacterHub>();
            //OnBreakEvent
            if (GetComponent<CollectEvent>() != null)
            {
                if (GetComponent<CollectEvent>().GetType() == typeof(CollectEvent))
                {
                    switch (GetComponent<CollectEvent>().type)
                    {
                        case CollectableType.AgileEvolution1:
                            if (_playerCharacterHub.GetComponent<EvolutionAgile>() != null)
                                return;
                            break;
                        case CollectableType.PlatformistEvolution1:
                            if (_playerCharacterHub.GetComponent<EvolutionPlatformist>() != null)
                                return;
                            break;
                        case CollectableType.StrengthEvolution1:
                            if (_playerCharacterHub.GetComponent<EvolutionStrength>() != null)
                                return;
                            break;
                        case CollectableType.GhostEvolution1:
                            if (_playerCharacterHub.GetComponent<EvolutionGhost>() != null)
                                return;
                            break;
                        default:
                            break;
                    }
                }
                GetComponent<CollectEvent>().OnCollectEvent(_playerCharacterHub.GetComponent<Player>());
            }
        }
    }
}
