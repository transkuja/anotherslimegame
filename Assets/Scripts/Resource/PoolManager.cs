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
    public float timerReturnToPool = -1;

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

    public GameObject GetItem(bool activeObjectOnRetrieval = false)
    {
        GameObject returnGameObject;
        if (poolParent.childCount == 0)
            returnGameObject = CreateRandomPoolItem();
        else
            returnGameObject = poolParent.GetChild(0).gameObject;

        returnGameObject.transform.SetParent(null);
        if (activeObjectOnRetrieval) returnGameObject.SetActive(true);

        return returnGameObject;
    }

    public GameObject GetItem(Transform _newParent, Vector3 _newPosition, Quaternion _newRotation, bool activeObjectOnRetrieval = false, bool spawnInWorldspace = false)
    {
        GameObject returnGameObject;
        if (poolParent.childCount == 0)
            returnGameObject = CreateRandomPoolItem();
        else
            returnGameObject = poolParent.GetChild(0).gameObject;

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

    public GameObject CreateRandomPoolItem()
    {
        GameObject item = GameObject.Instantiate(prefabs[Random.Range(0, prefabs.Count)], poolParent);
        item.AddComponent<PoolChild>().Pool = this;
        item.SetActive(false);
        ItemPool.Add(item);
        return item;
    }
}

public class PoolManager : MonoBehaviour {

    public Pool breakablePiecesPool;
    public Pool collectablePointsPool;
    public Pool monsterShotsPool;
    public Pool ghostTrailPool;
    public Pool moneyPool;
    public Pool colorFloorScorePickUpPool;
    public Pool runnerBlocPool;

    void Start () {

        /* =============================================================================== */
        GameObject breakablePiecesPoolParent = new GameObject("BreakablePiecesPool");
        breakablePiecesPoolParent.transform.SetParent(transform);
        breakablePiecesPool.PoolParent = breakablePiecesPoolParent.transform;

        for (int i = 0; i < breakablePiecesPool.poolSize; i++)
            breakablePiecesPool.CreateRandomPoolItem();
        /* =============================================================================== */

        /* =============================================================================== */
        GameObject collectablePointsPoolParent = new GameObject("CollectablePointsPool");
        collectablePointsPoolParent.transform.SetParent(transform);
        collectablePointsPool.PoolParent = collectablePointsPoolParent.transform;

        for (int i = 0; i < collectablePointsPool.poolSize; i++)
            collectablePointsPool.CreateRandomPoolItem();
        /* =============================================================================== */

        /* =============================================================================== */
        GameObject monsterShotsPoolParent = new GameObject("MonsterShotsPool");
        monsterShotsPoolParent.transform.SetParent(transform);
        monsterShotsPool.PoolParent = monsterShotsPoolParent.transform;

        for (int i = 0; i < monsterShotsPool.poolSize; i++)
            monsterShotsPool.CreateRandomPoolItem();
        /* =============================================================================== */

        /* =============================================================================== */
        GameObject ghostTrailPoolParent = new GameObject("GhostTrailPool");
        ghostTrailPoolParent.transform.SetParent(transform);
        ghostTrailPool.PoolParent = ghostTrailPoolParent.transform;

        for (int i = 0; i < ghostTrailPool.poolSize; i++)
            ghostTrailPool.CreateRandomPoolItem();
        /* =============================================================================== */

        /* =============================================================================== */
        GameObject moneyPoolParent = new GameObject("MoneyPool");
        moneyPoolParent.transform.SetParent(transform);
        moneyPool.PoolParent = moneyPoolParent.transform;

        for (int i = 0; i < moneyPool.poolSize; i++)
            moneyPool.CreateRandomPoolItem();
        /* =============================================================================== */

        /* =============================================================================== */
        GameObject colorFloorScorePickUpPoolParent = new GameObject("ColorFloorScorePickUpPool");
        colorFloorScorePickUpPoolParent.transform.SetParent(transform);
        colorFloorScorePickUpPool.PoolParent = colorFloorScorePickUpPoolParent.transform;

        for (int i = 0; i < colorFloorScorePickUpPool.poolSize; i++)
            colorFloorScorePickUpPool.CreateRandomPoolItem();
        /* =============================================================================== */

        /* =============================================================================== */
        GameObject runnerBlocPoolPoolParent = new GameObject("runnerBlocPool");
        runnerBlocPoolPoolParent.transform.SetParent(transform);
        runnerBlocPool.PoolParent = runnerBlocPoolPoolParent.transform;
        for (int i = 0; i < runnerBlocPool.poolSize; i++)
            runnerBlocPool.CreateRandomPoolItem();
        /* =============================================================================== */
    }
}

