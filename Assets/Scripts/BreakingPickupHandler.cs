using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingPickupHandler : MonoBehaviour {

    bool hasPickups = false;
    bool hasBadPickups = false;

    [SerializeField]
    GameObject ballPrefab;
    [SerializeField]
    GameObject bombPrefab;

    GameObject spawnedBall;

    public void InitPickups(bool _hasPickups, bool _hasBadPickups)
    {
        hasPickups = _hasPickups;
        hasBadPickups = _hasBadPickups;

        if (hasPickups)
            StartCoroutine(PickupCoroutine());

        if (hasBadPickups)
            StartCoroutine(BadPickupCoroutine());
    }

    IEnumerator BadPickupCoroutine()
    {
        yield return new WaitUntil(() => GameManager.CurrentState != GameState.ForcedPauseMGRules);
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));

            int spawnFrom = Random.Range(0, transform.childCount);
            GameObject spawnedBomb = Instantiate(bombPrefab, transform.GetChild(spawnFrom).position + Vector3.up * 2.0f, transform.GetChild(spawnFrom).rotation);
            spawnedBomb.GetComponent<Rigidbody>().AddForce((spawnedBomb.transform.forward + Vector3.up) * 100, ForceMode.Impulse);
        }
    }

    IEnumerator PickupCoroutine()
    {
        yield return new WaitUntil(() => GameManager.CurrentState != GameState.ForcedPauseMGRules);
        yield return new WaitForSeconds(5.0f);

        while (true)
        {
            yield return new WaitForSeconds(5.0f);

            int spawnFrom = Random.Range(0, transform.childCount);
            spawnedBall = Instantiate(ballPrefab, transform.GetChild(spawnFrom).position + Vector3.up * 2.0f, transform.GetChild(spawnFrom).rotation);
            spawnedBall.GetComponent<Rigidbody>().AddForce((spawnedBall.transform.forward + Vector3.up) * 100, ForceMode.Impulse);

            yield return new WaitUntil(() => spawnedBall == null);
        }
    }
}
