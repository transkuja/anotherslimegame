using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: should be a static class
public class EndingTrigger : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            HasFinishedProcess(other.GetComponent<Player>());            
        }
    }

    public void HasFinishedProcess(Player _player)
    {
        _player.PlayerController.enabled = false;

        // Making the player to stop in the air 
        _player.Rb.Sleep(); // Quelque part là, il y a un sleep

        // TODO: REACTIVATE INSTEAD OF INSTANTIATE (keys must not be destroyed too)
        for (int i = 0; i < Utils.GetMaxValueForCollectable(CollectableType.Key); i++)
        {
            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                _player.KeysInitialPosition[i],
                _player.KeysInitialRotation[i],
                null,
                CollectableType.Key)
            .GetComponent<Collectable>().Init();
        }

        GameManager.Instance.ScoreScreenReference.rank++;
        GameManager.Instance.ScoreScreenReference.RefreshScores(_player);
    }

}
