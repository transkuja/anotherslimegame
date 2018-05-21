using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingPickupHandler : MonoBehaviour {

    bool hasPickups = false;
    bool hasBadPickups = false;

    [SerializeField]
    GameObject ballPrefab;

    GameObject spawnedBall;

    public void InitPickups(bool _hasPickups, bool _hasBadPickups)
    {
        hasPickups = _hasPickups;
        hasBadPickups = _hasBadPickups;

        if (hasPickups)
            StartCoroutine(PickupCoroutine());
    }

    IEnumerator PickupCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);

            int spawnFrom = Random.Range(0, transform.childCount);
            spawnedBall = Instantiate(ballPrefab, transform.GetChild(spawnFrom).position + Vector3.up * 2.0f, transform.GetChild(spawnFrom).rotation);
            spawnedBall.GetComponent<Rigidbody>().AddForce((spawnedBall.transform.forward + Vector3.up) * 100, ForceMode.Impulse);

            yield return new WaitUntil(() => spawnedBall == null);
        }
    }
}
