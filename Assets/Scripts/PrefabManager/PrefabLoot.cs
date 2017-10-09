using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoot : MonoBehaviour {

    [SerializeField]
    public GameObject prefabCollectable1;

    public GameObject spawnCollectable1Instance(Vector3 where, Quaternion direction, Transform parent)
    {
        return Instantiate(prefabCollectable1, where, direction, parent);
    }


    [SerializeField]
    public GameObject prefabCollectable2;

    public GameObject spawnCollectable2Instance(Vector3 where, Quaternion direction, Transform parent)
    {
        return Instantiate(prefabCollectable2, where, direction, parent);
    }
}
