using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyPlatform : MonoBehaviour {

    float timer;
    bool isInitialized = false;

    public void Init(float _timer)
    {
        timer = _timer;
        isInitialized = true;
    }

    private void Update()
    {
        if (isInitialized)
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                if (transform.childCount > 0)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Transform tr = transform.GetChild(i);
                        if (tr.GetComponent<Player>())
                        {
                            tr.SetParent(null);
                            tr.localScale = Vector3.one;
                        }
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}
