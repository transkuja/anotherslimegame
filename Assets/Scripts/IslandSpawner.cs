using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class IslandSpawner : MonoBehaviour {

    private List<GameObject> evolutionIslands = new List<GameObject>();
    private List<GameObject> pointsIslands = new List<GameObject>();

    public void Awake()
    {
        FillEvolutionIslands();
    }

    void FillEvolutionIslands()
    {
        evolutionIslands.Clear();
        PrefabIle prefabIle = ResourceUtils.Instance.prefabIle;
        evolutionIslands.Add(prefabIle.prefabIle1GameObject);
        evolutionIslands.Add(prefabIle.prefabIle2GameObject);
        evolutionIslands.Add(prefabIle.prefabIle3GameObject);
        evolutionIslands.Add(prefabIle.prefabIle4GameObject);
        pointsIslands.Add(prefabIle.prefabPointIsland1GameObject);
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
                return Instantiate(HUBManager.instance.strengthRuneShelter, where, direction, parent);
            case CollectableType.PlatformistEvolution1:
                return Instantiate(HUBManager.instance.platformistRuneShelter, where, direction, parent);
            case CollectableType.AgileEvolution1:
                return Instantiate(HUBManager.instance.agileRuneShelter, where, direction, parent);
            case CollectableType.GhostEvolution1:
                return Instantiate(HUBManager.instance.ghostRuneShelter, where, direction, parent);
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
