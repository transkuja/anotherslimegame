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

    IEnumerator Start () {
		while (true)
        {
            yield return new WaitForSeconds(pickupDelay);
            if (pickupSpawned < maxPickupOnMap)
            {
                // Spawn pickup
                int randChild = Random.Range(0, transform.childCount);

                // Makes sure we don't spawn twice at the same place
                while (transform.GetChild(randChild).childCount > 0)
                    randChild = Random.Range(0, transform.childCount);

                GameObject go = Instantiate(ResourceUtils.Instance.poolManager.colorFloorScorePickUpPool.GetItem(), transform.GetChild(randChild));
                go.transform.localPosition = Vector3.up;
                go.SetActive(true);

                pickupSpawned++;
            }
        }
	}

}
