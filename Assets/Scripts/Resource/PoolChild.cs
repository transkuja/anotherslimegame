using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolChild : MonoBehaviour {

    public Pool pool;
    float currentTimer = 0.0f;
    bool noReturn = false;

    private void OnEnable()
    {
        currentTimer = pool.timerReturnToPool;
        if (currentTimer == -1) noReturn = true;
    }

    void Update () {
        if (noReturn)
            return;

        currentTimer -= Time.deltaTime;
        if (currentTimer < 0.0f)
        {
            ReturnToPool();
        }
	}

    public void ReturnToPool()
    {
        transform.SetParent(pool.PoolParent);
        gameObject.SetActive(false);
    }
}
