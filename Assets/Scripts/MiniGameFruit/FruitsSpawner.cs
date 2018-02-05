using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsSpawner : MonoBehaviour {

    [SerializeField]
    float fruitsSpawnDelay = 0.5f;

    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(fruitsSpawnDelay);

            // Spawn fruit
            int randChild = Random.Range(0, transform.childCount);

            ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform.GetChild(randChild), Vector3.zero, Quaternion.identity, true);
        }
    }
}
