using System.Collections;
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
    public GameObject prefabIle3GameObject;
    public GameObject prefabIle4GameObject;

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
        evolutionIslands.Add(prefabIle3GameObject);
        evolutionIslands.Add(prefabIle4GameObject);

        pointsIslands.Add(prefabPointIsland1GameObject);
    }

    public GameObject SpawnEvolutionIslandInstance(Vector3 where, Quaternion direction, Transform parent)
    {
        // Randomize islands spawn
        Utils.Shuffle(evolutionIslands);
        GameObject go = Instantiate(evolutionIslands[0], where, direction, parent);
        evolutionIslands.RemoveAt(0);
        return go;
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

    public GameObject GetShelterFeedbackFromEvolutionName(CollectableType evolutionType)
    {
        switch (evolutionType)
        {
            case CollectableType.AgileEvolution1:
                return prefabShelterAgilityFeedback;
            case CollectableType.PlatformistEvolution1:
                return prefabShelterPlatformistFeedback;
            case CollectableType.StrengthEvolution1:
                return prefabShelterStrengthFeedback;
            case CollectableType.GhostEvolution1:
                return prefabShelterGhostFeedback;
            default:
                return null;
        }
    }
}