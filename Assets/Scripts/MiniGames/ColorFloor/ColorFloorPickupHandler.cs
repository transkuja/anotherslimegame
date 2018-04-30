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
    [SerializeField]

    [Range(1,3)]
    int difficulty = 1;
    // Initialized with difficulty
    float initialSpawnRate;
    float currentSpawnRate;
    int minBadPickupSpawns;
    int maxBadPickupSpawns;
    //////////////////////////////////////

    int mapSize = 0;
    int lineCount;
    int lineSize;

    [SerializeField]
    int maxPickupsSpawnedWithoutScore = 3;
    int pickupsSpawnedSinceLastScore = 0;

    bool isUsingScorePickups = true;
    public static List<GameObject> spawnedPickups;

    public bool DEBUG_forceBadSpawns = false;

    void HandleDifficulty()
    {
        initialSpawnRate = 5.0f - difficulty;
        currentSpawnRate = initialSpawnRate;
        minBadPickupSpawns = (int)(difficulty * 1.5f);
        maxBadPickupSpawns = minBadPickupSpawns * 2;
    }

    IEnumerator Start()
    {
        HandleDifficulty();
        lineCount = transform.childCount;
        isUsingScorePickups = !((ColorFloorGameMode)GameManager.Instance.CurrentGameMode).squareToScoreMode;
        spawnedPickups = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
            mapSize += transform.GetChild(i).childCount;

        yield return new WaitUntil(() => GameManager.CurrentState != GameState.ForcedPauseMGRules);

        if (DEBUG_forceBadSpawns || (GameManager.Instance.DataContainer != null && !GameManager.Instance.DataContainer.launchedFromMinigameScreen))
        {
            StartCoroutine(BadPickupsSpawn());
        }

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
            while (transform.GetChild(randChild / lineCount).GetChild(randChild % lineSize).GetComponent<OnColoredFloorTrigger>().hasAnItem)
                randChild = Random.Range(0, mapSize);

            transform.GetChild(randChild / lineCount).GetChild(randChild % lineSize).GetComponent<OnColoredFloorTrigger>().hasAnItem = true;
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

            Debug.Log("spawn");
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

    IEnumerator BadPickupsSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnRate);
            currentSpawnRate *= Random.Range(0.8f, 1.1f);
            Debug.Log("spawnbad");

            // Spawn pickup, no pattern
            int[] randChild = new int[Random.Range(minBadPickupSpawns, maxBadPickupSpawns)];
            for (int i = 0; i < randChild.Length; ++i)
            {
                randChild[i] = Random.Range(0, mapSize);
                lineSize = transform.GetChild(randChild[i] / lineCount).childCount;

                // Makes sure we don't spawn twice at the same place
                while (transform.GetChild(randChild[i] / lineCount).GetChild(randChild[i] % lineSize).GetComponent<OnColoredFloorTrigger>().hasAnItem)
                    randChild[i] = Random.Range(0, mapSize);

                transform.GetChild(randChild[i] / lineCount).GetChild(randChild[i] % lineSize).GetComponent<OnColoredFloorTrigger>().hasAnItem = true;
                // Wait for x sec, OnEnable du feedback, start coroutine, after x sec, pop

                yield return new WaitForSeconds(0.33f);
                ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.ColorFloorPickUps, 1).GetItem(
                    transform.GetChild(randChild[i] / lineCount).GetChild(randChild[i] % lineSize),
                    Vector3.up * 1.5f,
                    Quaternion.identity,
                    true
                );
            }
        }
    }
}
