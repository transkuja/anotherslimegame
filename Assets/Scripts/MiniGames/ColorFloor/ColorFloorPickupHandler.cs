using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFloorPickupHandler : MonoBehaviour {

    [SerializeField]
    float pickupDelay = 0.5f;
    [SerializeField]
    int maxPickupOnMap = 3;

    [HideInInspector]
    public int pickupSpawned = 0;

    int mapSize = 0;
    int lineCount;
    int lineSize;

    IEnumerator Start () {
        lineCount = transform.childCount;
        for (int i = 0; i < transform.childCount; i++)
            mapSize += transform.GetChild(i).childCount;
        
		while (true)
        {
            yield return new WaitForSeconds(pickupDelay);
            if (pickupSpawned < maxPickupOnMap)
            {
                // Spawn pickup
                int randChild = Random.Range(0, mapSize);
                lineSize = transform.GetChild(randChild / lineCount).childCount;

                // Makes sure we don't spawn twice at the same place
                while (transform.GetChild(randChild/lineCount).GetChild(randChild%lineSize).childCount > 0)
                    randChild = Random.Range(0, mapSize);

                ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.ColorFloorScorePickUp).GetItem(
                    transform.GetChild(randChild / lineCount).GetChild(randChild % lineSize),
                    Vector3.up, 
                    Quaternion.identity,
                    true
                );

  //              ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.ColorFloorScorePickUp).GetItem(
  //                  transform.GetChild(randChild / lineCount).GetChild(randChild % lineSize),
  //                  Vector3.up, 
  //                  Quaternion.identity,
  //                  true
  //              );

  //              pickupSpawned++;
  //          }
  //      }
	//}

}
