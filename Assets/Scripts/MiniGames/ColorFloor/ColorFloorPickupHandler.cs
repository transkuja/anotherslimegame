using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFloorPickupHandler : MonoBehaviour
{
    [SerializeField]
    float pickupDelayMin = 0.5f;
    [SerializeField]
    float pickupDelayMax = 0.5f;
    [SerializeField]
    int maxPickupOnMap = 3;

    int mapSize = 0;
    int lineCount;
    int lineSize;

    [SerializeField]
    int maxPickupsSpawnedWithoutScore = 3;
    int pickupsSpawnedSinceLastScore = 0;

    bool isUsingScorePickups = true;
    public static List<GameObject> spawnedPickups;

    IEnumerator Start()
    {
        lineCount = transform.childCount;
        isUsingScorePickups = !((ColorFloorGameMode)GameManager.Instance.CurrentGameMode).squareToScoreMode;
        spawnedPickups = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
            mapSize += transform.GetChild(i).childCount;

        yield return new WaitUntil(() => GameManager.CurrentState != GameState.ForcedPauseMGRules);

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(pickupDelayMin, pickupDelayMax));
            if (spawnedPickups.Count >= maxPickupOnMap)
            {
                GameObject pickupToDestroy = spawnedPickups[0];
                spawnedPickups.RemoveAt(0);
                Destroy(pickupToDestroy);
                yield return new WaitForSeconds(1.0f);
            }

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

            spawnedPickups.Add(
                ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.ColorFloorPickUps).GetItem(
                    transform.GetChild(randChild / lineCount).GetChild(randChild % lineSize),
                    Vector3.up * 1.5f,
                    Quaternion.identity,
                    true,
                    false,
                    subpoolIndex
                )
            );

        }
    }

}
