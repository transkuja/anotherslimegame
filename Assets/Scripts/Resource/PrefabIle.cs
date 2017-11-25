﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabIle : MonoBehaviour
{
    [Header("Rune shelters")]
    public GameObject ghostRuneShelter;
    public GameObject platformistRuneShelter;
    public GameObject agileRuneShelter;
    public GameObject strengthRuneShelter;

    [Header("Islands")]
    public GameObject prefabIle1GameObject;

    public GameObject SpawnIleInstance(Vector3 where, Quaternion direction, Transform parent, bool useAlternativePrefab = false)
    {
        // USe random
        return Instantiate(prefabIle1GameObject, where, direction, parent);

    }

    public GameObject SpawnRuneShelterInstance(Vector3 where, Quaternion direction, Transform parent, CollectableType myItemType)
    {
        switch (myItemType)
        {
            case CollectableType.StrengthEvolution1:
                return Instantiate(strengthRuneShelter, where, direction, parent);
            case CollectableType.PlatformistEvolution1:
                return Instantiate(platformistRuneShelter, where, direction, parent);
            case CollectableType.AgileEvolution1:
                return Instantiate(agileRuneShelter, where, direction, parent);
            case CollectableType.GhostEvolution1:
                return Instantiate(ghostRuneShelter, where, direction, parent);
            default:
                Debug.Log("Unknown Item type");
                return null;
        }
    }
}