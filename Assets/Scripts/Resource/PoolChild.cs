using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolChild : MonoBehaviour {

    private Pool pool;
    float currentTimer = 0.0f;
    bool noReturn = false;
    bool isReady = false;

    public Pool Pool
    {
        get
        {
            return pool;
        }

        set
        {
            pool = value;
            currentTimer = Pool.timerReturnToPool;
            isReady = true;
        }
    }

    private void OnEnable()
    {
        if (Pool != null)
        {
            currentTimer = Pool.timerReturnToPool;
            if (currentTimer == -1) noReturn = true;
        }
    }

    void Update () {
        if (isReady)
        {
            if (noReturn)
                return;

            currentTimer -= Time.deltaTime;
            if (currentTimer < 0.0f)
            {
                ReturnToPool();
            }
        }
	}

    public void ReturnToPool()
    {
        transform.SetParent(Pool.PoolParent);
        gameObject.SetActive(false);
    }
}
