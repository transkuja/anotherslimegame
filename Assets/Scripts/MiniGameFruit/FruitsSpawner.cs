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
    IEnumerator Start()
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

            ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform/*.GetChild(randChild)*/, new Vector3(Random.Range(minX, maxX), 35, Random.Range(minZ, maxZ)), Quaternion.identity, true);
        }
    }
}
