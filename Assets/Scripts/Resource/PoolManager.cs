using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    Transform poolParent;
    List<GameObject> itemPool;
    public float timerReturnToPool = -1;

    public Pool(Transform _poolParent, float _timerReturnToPool) {
        poolParent = _poolParent;
        timerReturnToPool = _timerReturnToPool;
    }

    public List<GameObject> ItemPool
    {
        get
        {
            if (itemPool == null)
                itemPool = new List<GameObject>();
            return itemPool;
        }
    }

    public Transform PoolParent
    {
        get
        {
            return poolParent;
        }

        set
        {
            poolParent = value;
        }
    }
}

[System.Serializable]
public class PoolLeader
{
    public PoolName poolName;
    Transform poolParent;
    [SerializeField]
    List<GameObject> prefabs;

    [SerializeField]
    [Tooltip("Create a subpool for each prefab.")]
    bool separatePrefabsIntoDifferentPools = false;

    [SerializeField]
    int poolSize = 50;
    List<Pool> subPools;
    public float timerReturnToPool = -1;

    PoolLeader() { }

    List<Pool> SubPools
    {
        get
        {
            if (subPools == null)
                subPools = new List<Pool>();
            return subPools;
        }
    }

    public Transform PoolParent
    {
        get
        {
            return poolParent;
        }

        set
        {
            poolParent = value;
        }
    }

    public GameObject GetItem(bool activeObjectOnRetrieval = false, int subpoolNumber = 0)
    {
        GameObject returnGameObject;
        if (poolParent.GetChild(subpoolNumber).childCount == 0)
            returnGameObject = CreateRandomPoolItem(subpoolNumber);
        else
            returnGameObject = poolParent.GetChild(subpoolNumber).GetChild(0).gameObject;

        returnGameObject.transform.SetParent(null);
        if (activeObjectOnRetrieval) returnGameObject.SetActive(true);

        return returnGameObject;
    }

    public GameObject GetItem(Transform _newParent, Vector3 _newPosition, Quaternion _newRotation, bool activeObjectOnRetrieval = false, bool spawnInWorldspace = false, int subpoolNumber = 0)
    {
        GameObject returnGameObject;
        if (poolParent == null)
        {
            Debug.LogWarning("Pool parent is null (wtf?)");
            return null;
        }

        if (poolParent.GetChild(subpoolNumber).childCount == 0)
            returnGameObject = CreateRandomPoolItem(subpoolNumber);
        else
            returnGameObject = poolParent.GetChild(subpoolNumber).GetChild(0).gameObject;

        returnGameObject.transform.SetParent(_newParent);
        if (spawnInWorldspace)
        {
            returnGameObject.transform.position = _newPosition;
            returnGameObject.transform.rotation = _newRotation;
        }
        else
        {
            returnGameObject.transform.localPosition = _newPosition;
            returnGameObject.transform.localRotation = _newRotation;
        }

        if (activeObjectOnRetrieval) returnGameObject.SetActive(true);
        return returnGameObject;

    }

    public void InitializePool()
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogWarning("Cannot initialize pool " + poolParent.name + " because no prefabs are linked.");
            return;
        }

        if (poolSize <= 0)
        {
            Debug.LogWarning("Cannot initialize pool because pool size is null or negative.");
            return;
        }

        for (int i = 0; i < ((separatePrefabsIntoDifferentPools) ? prefabs.Count : 1); i++)
        {
            if (poolParent.childCount <= i)
            {
                GameObject poolContainer = new GameObject("Pool Container " + i);
                poolContainer.transform.parent = poolParent;
            }

            SubPools.Add(new Pool(poolParent.GetChild(i), timerReturnToPool));
            for (int j = 0; j < poolSize / ((separatePrefabsIntoDifferentPools) ? prefabs.Count : 1); j++)
            {
                CreateRandomPoolItem(i);
            }
        }
    }

    GameObject CreateRandomPoolItem(int _subpoolIndex)
    {
        int prefabIndex = (separatePrefabsIntoDifferentPools) ? _subpoolIndex : Random.Range(0, prefabs.Count);
        GameObject item = GameObject.Instantiate(prefabs[prefabIndex], poolParent.GetChild(_subpoolIndex));
        item.AddComponent<PoolChild>().Pool = SubPools[_subpoolIndex];
        item.SetActive(false);
        SubPools[_subpoolIndex].ItemPool.Add(item);
        return item;
    }
}

/*
 * Update this enum with your new pool's name 
 */
public enum PoolName { BreakablePieces, CollectablePoints, MonsterShots, GhostTrail, Money, ColorFloorPickUps, RunnerBloc, Fruits, HitParticles }
public class PoolManager : MonoBehaviour {

    [SerializeField]
    List<PoolLeader> poolLeaders;

    /// <summary>
    /// Return the pool flagged _poolName in inspector. If several are flagged _poolName, returns the one at the index _poolIndex.
    /// </summary>
    /// <param name="_poolName"></param>
    /// <param name="_poolIndex"></param>
    /// <returns></returns>
    public PoolLeader GetPoolByName(PoolName _poolName, int _poolIndex = 0)
    {
        int iteration = 0;
        foreach (PoolLeader leader in poolLeaders)
        {
            if (leader.poolName == _poolName)
            {
                if (iteration == _poolIndex)
                    return leader;
                else
                    iteration++;
            }
        }

        return null;
    }

    void Start () {

        if (poolLeaders == null || poolLeaders.Count == 0)
        {
            Debug.LogWarning("There are no pool leaders defined in Pool Manager.");
            return;
        }

        foreach (PoolLeader leader in poolLeaders)
        {
            GameObject poolParent = new GameObject(leader.poolName.ToString());
            poolParent.transform.SetParent(transform);
            leader.PoolParent = poolParent.transform;
            leader.InitializePool();
        }
    }
}

