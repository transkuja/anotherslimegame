using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            other.GetComponent<PlayerController>().enabled = false;

            // Making the player to stop in the air 
            other.GetComponent<Rigidbody>().Sleep(); // Quelque part là, il y a un sleep

            for (int i = 0; i < Utils.GetMaxValueForCollectable(CollectableType.Key); i++)
            {
                ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                    other.GetComponent<Player>().KeysInitialPosition[i], 
                    other.GetComponent<Player>().KeysInitialRotation[i], 
                    null, 
                    CollectableType.Key)
                .GetComponent<Collectable>().Init();
            }

            GameManager.Instance.ScoreScreenReference.rank++;
            GameManager.Instance.ScoreScreenReference.RefreshScores(other.GetComponent<Player>());
        }
    }
}
