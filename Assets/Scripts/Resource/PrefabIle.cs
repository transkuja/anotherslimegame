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

    [Header("Evolution Islands")]
    public GameObject prefabIle1GameObject;
    public GameObject prefabIle2GameObject;


    [Header("Point Islands")]
    public GameObject prefabPointIsland1GameObject;

    [Header("Shelter Feedbacks")]
    public GameObject prefabShelterPlatformistFeedback;
    public GameObject prefabShelterStrengthFeedback;
    public GameObject prefabShelterAgilityFeedback;
    public GameObject prefabShelterGhostFeedback;

    private List<GameObject> evolutionIslands = new List<GameObject>();
    private List<GameObject> pointsIslands = new List<GameObject>();


    public void Awake()
    {
        evolutionIslands.Add(prefabIle1GameObject);
        evolutionIslands.Add(prefabIle2GameObject);

        pointsIslands.Add(prefabPointIsland1GameObject);
    }

    public GameObject SpawnEvolutionIslandInstance(Vector3 where, Quaternion direction, Transform parent)
    {
        // Randomize islands spawn
        Utils.Shuffle(evolutionIslands);
        return Instantiate(evolutionIslands[0], where, direction, parent);
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
                Debug.Log("Unknown Shelter type");
                return null;
        }
    }

    public GameObject SpawnPointIslandInstance(Vector3 where, Quaternion direction, Transform parent)
    {
        // Randomize islands spawn
        Utils.Shuffle(pointsIslands);
        return Instantiate(pointsIslands[0], where, direction, parent);
    }
}