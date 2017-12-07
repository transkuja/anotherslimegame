using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public List<GameObject> prefabs;
    public int poolSize = 50;
    List<GameObject> itemPool;

    Pool() { }

    public List<GameObject> ItemPool
    {
        get
        {
            if (itemPool == null)
                itemPool = new List<GameObject>();
            return itemPool;
        }
    }

    public GameObject GetItem()
    {
        for (int i = 0; i < ItemPool.Count; i++)
        {
            if (!ItemPool[i].activeInHierarchy)
                return itemPool[i];
        }

         // Ugly, the parent should be defined by a component PoolParent or directly link in the pool
        return CreateRandomPoolItem(itemPool[0].transform.parent); ;
    }

    public GameObject CreateRandomPoolItem(Transform _poolParent)
    {
        GameObject item = GameObject.Instantiate(prefabs[Random.Range(0, prefabs.Count)], _poolParent);
        item.AddComponent<PoolChild>();
        item.SetActive(false);
        ItemPool.Add(item);
        return item;
    }
}

public class PoolManager : MonoBehaviour {

    [SerializeField]
    public Pool breakablePiecesPool;

	void Start () {
        GameObject poolParent = new GameObject("BreakablePiecesPool");

        for (int i = 0; i < breakablePiecesPool.poolSize; i++)
        {
            breakablePiecesPool.CreateRandomPoolItem(poolParent.transform);
        }
	}

}
