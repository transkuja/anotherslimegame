using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoot : MonoBehaviour {

    [SerializeField]
    public GameObject prefabCollectable1;
    [SerializeField]
    public GameObject prefabCollectable2;
    [SerializeField]
    public GameObject prefabCollectable3;

    public GameObject prefabKeySprite;


    public GameObject prefabPointSprite;

    public GameObject SpawnCollectableInstance(Vector3 where, Quaternion direction, Transform parent, CollectableType myItemType, bool isCollectableFromPlayer)
    {
        switch (myItemType)
        {
            case CollectableType.Evolution1:
                return Instantiate(prefabCollectable1, where, direction, parent);
            case CollectableType.Evolution2:
                return Instantiate(prefabCollectable2, where, direction, parent);
            case CollectableType.Points:
                return Instantiate(prefabCollectable3, where, direction, parent);
            default:
                Debug.Log("Unknown Item type");
                return null;
        }

        //prefabCollectable1.GetComponent<Collectable>()
    }
}
