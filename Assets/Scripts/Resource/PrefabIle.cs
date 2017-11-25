using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabIle : MonoBehaviour
{

    [SerializeField]
    public GameObject prefabIle1GameObject;


    public GameObject SpawnIleInstance(Vector3 where, Quaternion direction, Transform parent, bool useAlternativePrefab = false)
    {
        // USe random
        return Instantiate(prefabIle1GameObject, where, direction, parent);

    }
}