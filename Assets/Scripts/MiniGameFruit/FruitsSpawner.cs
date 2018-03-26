using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsSpawner : MonoBehaviour {

    [SerializeField]
    float fruitsSpawnDelay = 0.5f;

    /**
     * TEST 
     **/
    public BoxCollider boxColliderSpawn;
    public float minX;
    public float minZ;
    public float maxX;
    public float maxZ;

    public IEnumerator Spawner()
    {
        if (GameManager.CurrentState == GameState.Normal)
        {
            while (true)
            {
                yield return new WaitForSeconds(fruitsSpawnDelay);

                minX = -(boxColliderSpawn.transform.localScale.x / 2);
                maxX = boxColliderSpawn.transform.localScale.x / 2;

                minZ = -(boxColliderSpawn.transform.localScale.z / 2);
                maxZ = boxColliderSpawn.transform.localScale.z / 2;

                // Spawn fruit
                //int randChild = Random.Range(0, transform.childCount);

                GameObject go = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform, new Vector3(Random.Range(minX, maxX), 15, Random.Range(minZ, maxZ)), Quaternion.identity, true);
                go.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }
}
