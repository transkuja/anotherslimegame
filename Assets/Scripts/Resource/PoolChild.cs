using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            CurrentTimer = Pool.timerReturnToPool;
            isReady = true;
        }
    }

    public float CurrentTimer
    {
        get
        {
            return currentTimer;
        }

        set
        {
            currentTimer = value;
        }
    }

    private void OnEnable()
    {
        if (Pool != null)
        {
            CurrentTimer = Pool.timerReturnToPool;
            if (CurrentTimer == -1) noReturn = true;
        }
    }
   
    void Update () {
        if (isReady)
        {
            if (noReturn)
                return;

            CurrentTimer -= Time.deltaTime;
            if (CurrentTimer < 0.0f)
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
