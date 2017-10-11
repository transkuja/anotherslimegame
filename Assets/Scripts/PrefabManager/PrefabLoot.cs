using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Collectable1, Collectable2 }
public class PrefabLoot : MonoBehaviour {

    [SerializeField]
    public GameObject prefabCollectable1;


    [SerializeField]
    public GameObject prefabCollectable2;


    public GameObject SpawnCollectableInstance(Vector3 where, Quaternion direction, Transform parent, ItemType myItemType)
    {
        switch (myItemType)
        {
            case ItemType.Collectable1:
                return Instantiate(prefabCollectable1, where, direction, parent);
            case ItemType.Collectable2:
                return Instantiate(prefabCollectable2, where, direction, parent);
            default:
                Debug.Log("Unknown Item type");
                return null;
        }
    }
}
