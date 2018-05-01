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
        initialSpawnRate = 5.0f - difficulty/2;
        currentSpawnRate = initialSpawnRate;
        minBadPickupSpawns = difficulty + 1;
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
            while (transform.GetChild(randChild / lineCount).GetChild(randChild % lineSize).GetComponent<OnColoredFloorTrigger>().HasAnItem)
                randChild = Random.Range(0, mapSize);

            transform.GetChild(randChild / lineCount).GetChild(randChild % lineSize).GetComponent<OnColoredFloorTrigger>().HasAnItem = true;
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

    IEnumerator BadPickupsSpawn()
    {
        while (true)
        {
            // Randomly ou autre
            int randomPatt = Random.Range(0, 3);
            int randomHowToSpawn = Random.Range(0, 2);

            yield return new WaitForSeconds(currentSpawnRate * ((randomPatt == 0) ? 2.0f : 1.0f)  + (randomHowToSpawn *2));
            currentSpawnRate *= Random.Range(0.8f, 1.1f);
            currentSpawnRate = Mathf.Max(currentSpawnRate, 2.0f);

            List<int> randChild = new List<int>();
            if (randomPatt == 0)
            {
                randChild.AddRange(RandomlySpawnBadPickup());
            }
            else if (randomPatt == 1)
            {
                int numberOfLines = Random.Range(1, 3);
                int numberOfColumns = Random.Range(1, 3);

                for (int i = 0; i < numberOfLines; i++)
                    randChild.AddRange(LineSpawnBadPickup());
                for (int i = 0; i < numberOfColumns; i++)
                    randChild.AddRange(ColumnSpawnBadPickup());
            }
            else
            {
                int numberOfAsc = Random.Range(1, 3);
                int numberOfDesc = Random.Range(1, 3);

                for (int i = 0; i < numberOfAsc; i++)
                    randChild.AddRange(AscendingDiagonalBadPickup());
                for (int i = 0; i < numberOfDesc; i++)
                    randChild.AddRange(DescendingDiagonalBadPickup());
            }


            if (randomHowToSpawn == 1)
                StartCoroutine(SpawnOneAfterAnother(randChild.ToArray()));
            else
                SpawnAtTheSameTime(randChild.ToArray());
        }
    }

    int[] AscendingDiagonalBadPickup(int _startingLine = -1, int _startingColumn = -1)
    {
        if (_startingLine == -1 && _startingColumn == -1)
        {
            int randStart = Random.Range(0, 15);
            _startingLine = Mathf.Min(randStart, 7);
            _startingColumn = Mathf.Max(0, randStart - 7);
        }
        else
        {
            _startingLine = Mathf.Clamp(_startingLine, 0, 7);
            _startingColumn = Mathf.Clamp(_startingColumn, 0, 7);
        }
        int[] result = new int[_startingLine + 1 - _startingColumn];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (_startingLine - i) * 8 + i + _startingColumn;
        }

        return result;
    }

    int[] DescendingDiagonalBadPickup(int _startingLine = -1, int _startingColumn = -1)
    {
        if (_startingLine == -1 && _startingColumn == -1)
        {
            int randStart = Random.Range(0, 15);
            _startingLine = (randStart > 7) ? 0 : randStart;
            _startingColumn = Mathf.Max(0, randStart - 7);
        }
        else
        {
            _startingLine = Mathf.Clamp(_startingLine, 0, 7);
            _startingColumn = Mathf.Clamp(_startingColumn, 0, 7);
        }
        int[] result = new int[8 - _startingLine - _startingColumn];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (_startingLine + i) * 8 + i + _startingColumn;
        }

        return result;
    }

    int[] LineSpawnBadPickup(int _lineIndex = -1)
    {
        int[] result = new int[8];
        if (_lineIndex == -1)
        {
            _lineIndex = Random.Range(0, 7);
        }

        for (int i = 0; i < 8; i++)
            result[i] = (_lineIndex * 8) + i;

        return result;
    }


    int[] ColumnSpawnBadPickup(int _columnIndex = -1)
    {
        int[] result = new int[8];
        if (_columnIndex == -1)
        {
            _columnIndex = Random.Range(0, 7);
        }

        for (int i = 0; i < 8; i++)
            result[i] = _columnIndex + 8*i;

        return result;
    }


    int[] RandomlySpawnBadPickup()
    {
        // Spawn pickup, no pattern
        int[] randChild = new int[Random.Range(minBadPickupSpawns, maxBadPickupSpawns + 1)];
        for (int i = 0; i < randChild.Length; ++i)
        {
            randChild[i] = Random.Range(0, mapSize);
            lineSize = transform.GetChild(randChild[i] / lineCount).childCount;

            // Makes sure we don't spawn twice at the same place
            while (transform.GetChild(randChild[i] / lineCount).GetChild(randChild[i] % lineSize).GetComponent<OnColoredFloorTrigger>().HasAnItem)
                randChild[i] = Random.Range(0, mapSize);
           
        }
        return randChild;
    }

    IEnumerator SpawnOneAfterAnother(int[] _randChild)
    {
        for (int i = 0; i < _randChild.Length; i++)
        {
            yield return new WaitForSeconds(0.33f);
            transform.GetChild(_randChild[i] / lineCount).GetChild(_randChild[i] % lineSize).GetComponent<OnColoredFloorTrigger>().WarnPlayerSmthgBadIsComing();
        }
    }

    void SpawnAtTheSameTime(int[] _randChild)
    {
        for (int i = 0; i < _randChild.Length; i++)
        {
            transform.GetChild(_randChild[i] / lineCount).GetChild(_randChild[i] % lineSize).GetComponent<OnColoredFloorTrigger>().WarnPlayerSmthgBadIsComing();
        }
    }
}
