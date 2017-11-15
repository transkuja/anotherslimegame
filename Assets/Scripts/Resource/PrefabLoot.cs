using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoot : MonoBehaviour {

    [SerializeField]
    public GameObject prefabWingsEvolution1GameObject;
    [SerializeField]
    public GameObject prefabWingsEvolution2GameObject;
    [SerializeField]
    public GameObject prefabPointsGameObject;
    [SerializeField]
    public GameObject prefabStrengthEvolution1GameObject;
    [SerializeField]
    public GameObject prefabKeyGameObject;

    public GameObject prefabKeySprite;


    public GameObject prefabPointSprite;

    public GameObject SpawnCollectableInstance(Vector3 where, Quaternion direction, Transform parent, CollectableType myItemType)
    {
        switch (myItemType)
        {
            case CollectableType.WingsEvolution1:
                return Instantiate(prefabWingsEvolution1GameObject, where, direction, parent);
            case CollectableType.WingsEvolution2:
                return Instantiate(prefabWingsEvolution2GameObject, where, direction, parent);
            case CollectableType.StrengthEvolution1:
                return Instantiate(prefabStrengthEvolution1GameObject, where, direction, parent);
            case CollectableType.PlatformistEvolution1:
                return Instantiate(prefabStrengthEvolution1GameObject, where, direction, parent);
            case CollectableType.AgileEvolution1:
                return Instantiate(prefabStrengthEvolution1GameObject, where, direction, parent);
            case CollectableType.Points:
                return Instantiate(prefabPointsGameObject, where, direction, parent);
            case CollectableType.Key:
                return Instantiate(prefabKeyGameObject, where, direction, parent);
            default:
                Debug.Log("Unknown Item type");
                return null;
        }
    }
}
