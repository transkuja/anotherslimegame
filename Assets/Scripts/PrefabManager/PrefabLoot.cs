using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoot : MonoBehaviour {

    private static PrefabLoot instance;

    public PrefabLoot Instance
    {
        get
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            return instance;
        }

        private set { }
    }

    [SerializeField]
    public GameObject prefabCollectable1;

    public GameObject spawnCollectable1Instance(Vector3 where, Quaternion direction, Transform parent)
    {
        return Instantiate(Instance.prefabCollectable1, where, direction, parent);
    }


    [SerializeField]
    public GameObject prefabCollectable2;

    public GameObject spawnCollectable2Instance(Vector3 where, Quaternion direction, Transform parent)
    {
        return Instantiate(Instance.prefabCollectable2, where, direction, parent);
    }
}
