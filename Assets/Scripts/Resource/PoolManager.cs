using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    Transform poolParent;
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

    public GameObject GetItem()
    {
        GameObject returnGameObject;
        if (poolParent.childCount == 0)
            returnGameObject = CreateRandomPoolItem();
        else
            returnGameObject = poolParent.GetChild(0).gameObject;

        returnGameObject.transform.SetParent(null);

        return returnGameObject;
    }

    public GameObject CreateRandomPoolItem()
    {
        GameObject item = GameObject.Instantiate(prefabs[Random.Range(0, prefabs.Count)], poolParent);
        item.AddComponent<PoolChild>();
        item.GetComponent<PoolChild>().poolParent = poolParent;
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
        breakablePiecesPool.PoolParent = poolParent.transform;

        for (int i = 0; i < breakablePiecesPool.poolSize; i++)
        {
            breakablePiecesPool.CreateRandomPoolItem();
        }
	}

}
