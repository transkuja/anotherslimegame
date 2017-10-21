using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnManager : MonoBehaviour{

    // Spawned Items Location
    public Dictionary<int, Transform> dicSpawnItemsLocations = new Dictionary<int, Transform>();
    private int lastInsertedKeySpawnItems = 0;

    // Spawned Monsters Location
    public Dictionary<int, Transform> dicSpawnMonstersLocations = new Dictionary<int, Transform>();
    private int lastInsertedKeySpawnMonsters = 0;

    // Frame control on item
    private int spawnedItemsCountAtTheSameTime = 0;
    private const int MAXSPAWNITEMSCOUNTATTHESAMETIME = 10;

    // Frame control on monster
    private int spawnedMonsterCountAtTheSameTime = 0;
    private const int MAXSPAWNMONSTERSCOUNTATTHESAMETIME = 2;

    private static SpawnManager instance;

    public int SpawnedMonsterCount
    {
        get
        {
            return spawnedMonsterCountAtTheSameTime;
        }

        set
        {
            spawnedMonsterCountAtTheSameTime = value;
        }
    }

    public int SpawnedItemsCount
    {
        get
        {
            return spawnedItemsCountAtTheSameTime;
        }

        set
        {
            spawnedItemsCountAtTheSameTime = value;
        }
    }

    public void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    public static SpawnManager Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    private void SpawnItem(int idLocation, CollectableType myItemType, bool forceSpawn = false)
    {
        if(instance.dicSpawnItemsLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error  : invalid location");
            return;
        }
            
        if(!forceSpawn && SpawnedItemsCount == MAXSPAWNITEMSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max monster reach");
            return;
        }

        SpawnedItemsCount++;
        RessourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
            instance.dicSpawnItemsLocations[idLocation].transform.position,
            instance.dicSpawnItemsLocations[idLocation].transform.rotation,
            null,
            myItemType
        );
    }


    private void SpawnMonster(int idLocation, MonsterType myMonsterType, bool forceSpawn = false)
    {
        if (instance.dicSpawnMonstersLocations.ContainsKey(idLocation) == false)
        {
            Debug.Log("Error : invalid location");
            return;
        }

        if (!forceSpawn && SpawnedMonsterCount == MAXSPAWNMONSTERSCOUNTATTHESAMETIME)
        {
            Debug.Log("Error  : max monster reach");
            return;
        }

        SpawnedMonsterCount++;
        RessourceUtils.Instance.refPrefabMonster.SpawnMonsterInstance(
            instance.dicSpawnMonstersLocations[idLocation].transform.position,
            instance.dicSpawnMonstersLocations[idLocation].transform.rotation,
            null,
            myMonsterType
        );
    }

    // add a transformation to spawn manager, retrieve the id where the spawn location was inserted
    // call before everything
    public int RegisterSpawnItemLocation(Transform mySpawnLocation, CollectableType myItemType, bool needSpawn = false, bool forceSpawn = false)
    {
        instance.dicSpawnItemsLocations.Add(lastInsertedKeySpawnItems, mySpawnLocation);

        if (needSpawn)
        {
            SpawnItem(lastInsertedKeySpawnItems, myItemType, forceSpawn);
        }

        return lastInsertedKeySpawnItems++;
    }

    // call on destroy on a spawn item
    public void UnregisterSpawnItemLocation(int idToUnregister)
    {
        instance.dicSpawnItemsLocations.Remove(idToUnregister);
    }


    // add a transformation to spawn manager, retrieve the id where the spawn location was inserted
    // call before everything
    public int RegisterSpawnMonsterLocation(Transform mySpawnLocation, MonsterType myMonsterType, bool needSpawn = false, bool forceSpawn = false)
    {
        instance.dicSpawnMonstersLocations.Add(lastInsertedKeySpawnMonsters, mySpawnLocation);

        if (needSpawn)
        {
            SpawnMonster(lastInsertedKeySpawnMonsters, myMonsterType, forceSpawn);
        }

        return lastInsertedKeySpawnMonsters++;
    }

    // call on destroy on a spawn monster
    public void UnregisterSpawnMonsterLocation(int idToUnregister)
    {
        instance.dicSpawnMonstersLocations.Remove(idToUnregister);
    }
}
