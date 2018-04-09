using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFloorPickupHandler : MonoBehaviour
{
    [SerializeField]
    float pickupDelay = 0.5f;
    [SerializeField]
    int maxPickupOnMap = 3;

    [HideInInspector]
    public static int pickupSpawned = 0;

    int mapSize = 0;
    int lineCount;
    int lineSize;

    [SerializeField]
    int maxPickupsSpawnedWithoutScore = 3;
    int pickupsSpawnedSinceLastScore = 0;

    bool isUsingScorePickups = true;

    IEnumerator Start()
    {
        pickupSpawned = 0;
        lineCount = transform.childCount;
        isUsingScorePickups = !((ColorFloorGameMode)GameManager.Instance.CurrentGameMode).squareToScoreMode;

        for (int i = 0; i < transform.childCount; i++)
            mapSize += transform.GetChild(i).childCount;

        yield return new WaitUntil(() => GameManager.CurrentState != GameState.ForcedPauseMGRules);

        while (true)
        {
            yield return new WaitForSeconds(pickupDelay);
            if (pickupSpawned < maxPickupOnMap)
            {
                // Spawn pickup
                int randChild = Random.Range(0, mapSize);
                lineSize = transform.GetChild(randChild / lineCount).childCount;

                // Makes sure we don't spawn twice at the same place
                while (transform.GetChild(randChild / lineCount).GetChild(randChild % lineSize).childCount > 1)
                    randChild = Random.Range(0, mapSize);

                int subpoolIndex = Random.Range(0, ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.ColorFloorPickUps).PoolParent.childCount);

                if (isUsingScorePickups)
                {
                    pickupsSpawnedSinceLastScore++;
                    if (pickupsSpawnedSinceLastScore >= maxPickupsSpawnedWithoutScore)
                    {
                        subpoolIndex = 0;
                        pickupsSpawnedSinceLastScore = 0;
                    }                  
                }

                ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.ColorFloorPickUps).GetItem(
                    transform.GetChild(randChild / lineCount).GetChild(randChild % lineSize),
                    Vector3.up * 1.5f,
                    Quaternion.identity,
                    true,
                    false,
                    subpoolIndex
                );
          
                pickupSpawned++;
            }
        }
    }

}
