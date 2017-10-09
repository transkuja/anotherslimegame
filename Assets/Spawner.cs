using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour {

    public Transform[] spawnLocations;
    private static int spawnedItemsCount = -1;
    private const int MAXSPAWNITEMSCOUNT = 10;

    private static int spawnedMonsterCount = -1;
    private const int MAXSPAWNMONSTERSCOUNT = 2;

    public static int SpawnedMonsterCount
    {
        get
        {
            return spawnedMonsterCount;
        }

        set
        {
            spawnedMonsterCount = value;
        }
    }

    public static int SpawnedItemsCount
    {
        get
        {
            if (spawnedItemsCount == -1) spawnedItemsCount = 0;
            return spawnedItemsCount;
        }

        set
        {
            spawnedItemsCount = value;
        }
    }

    // Use this for initialization
    void Start () {
    
    }

    public void computeSpawnItems()
    {
        for (int i = SpawnedItemsCount; i < MAXSPAWNITEMSCOUNT; i++ )
        {
            SpawnedItemsCount++;
            //RessourceUtils.Instance.refPrefabLoot.spawnCollectable1Instance();
        }

    }

    public void computeSpawnMonsters()
    {
        for (int i = SpawnedMonsterCount; i < MAXSPAWNMONSTERSCOUNT; i++)
        {
            SpawnedMonsterCount++;
            //Random.Range(0, 3);
            //RessourceUtils.Instance.refPrefabLoot.spawnMonster1Instance();
        }
    }

}
