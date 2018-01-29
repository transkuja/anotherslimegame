using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoot : MonoBehaviour {

    [SerializeField]
    public GameObject prefabPointsGameObject;
    [SerializeField]
    public GameObject prefabPointsDroppedGameObject;
    [SerializeField]
    public GameObject prefabMoneyGameObject;
    [SerializeField]
    public GameObject prefabMoneyDroppedGameObject;
    [SerializeField]
    public GameObject prefabStrengthEvolution1GameObject;
    [SerializeField]
    public GameObject prefabAgileEvolution1GameObject;
    [SerializeField]
    public GameObject prefabPlatformistEvolution1GameObject;
    [SerializeField]
    public GameObject prefabGhostEvolution1GameObject;
    [SerializeField]
    public GameObject prefabKeyGameObject;


    [Header("UI")]

    public GameObject prefabTutoText;
    public GameObject prefabTutoTextForAll;

    public GameObject SpawnCollectableInstance(Vector3 where, Quaternion direction, Transform parent, CollectableType myItemType, bool useAlternativePrefab = false)
    {
        switch (myItemType)
        {
            case CollectableType.StrengthEvolution1:
                return Instantiate(prefabStrengthEvolution1GameObject, where, direction, parent);
            case CollectableType.PlatformistEvolution1:
                return Instantiate(prefabPlatformistEvolution1GameObject, where, direction, parent);
            case CollectableType.AgileEvolution1:
                return Instantiate(prefabAgileEvolution1GameObject, where, direction, parent);
            case CollectableType.GhostEvolution1:
                return Instantiate(prefabGhostEvolution1GameObject, where, direction, parent);
            case CollectableType.Points:
                if (!useAlternativePrefab)
                    return Instantiate(prefabPointsGameObject, where, direction, parent);
                else
                    return Instantiate(prefabPointsDroppedGameObject, where, direction, parent);
            case CollectableType.Money:
                if(!useAlternativePrefab)
                    return Instantiate(prefabMoneyGameObject, where, direction, parent);
                else
                    return Instantiate(prefabMoneyDroppedGameObject, where, direction, parent);
            case CollectableType.Rune:
                return Instantiate(prefabKeyGameObject, where, direction, parent);
            default:
                Debug.Log("Unknown Item type");
                return null;
        }
    }
}
