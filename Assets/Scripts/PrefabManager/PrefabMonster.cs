using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabMonster : MonoBehaviour {

    [SerializeField]
    public GameObject prefabMonster1;

    public GameObject spawnMonster2Instance(Vector3 where, Quaternion direction, Transform parent)
    {
        return Instantiate(prefabMonster1, where, direction, parent);
    }
}
