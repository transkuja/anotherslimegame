using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnATrap : MonoBehaviour {
    [SerializeField]
    GameObject rabitePrefab;

    [SerializeField]
    GameObject bombPrefab;

    public void SpawnTraps()
    {
        int toSpawn = Random.Range(0, 3);
        GameObject toInstantiate = null;
        switch(toSpawn)
        {
            case 0:
                return;
            case 1:
                toInstantiate = rabitePrefab;
                break;
            case 2:
                toInstantiate = bombPrefab;
                break;
        }

        ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles)
            .GetItem(null, transform.position, Quaternion.identity, true, false, (int)HitParticles.BigHitStar);
        GameObject go = Instantiate(toInstantiate);
        if (go.GetComponent<TheBombPickup>())
            go.GetComponent<SphereCollider>().isTrigger = false;
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        //go.transform.localScale = Vector3.one;
        go.transform.SetParent(transform);
    }
}
