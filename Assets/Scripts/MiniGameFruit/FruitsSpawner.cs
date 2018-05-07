using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FruitsSpawner : MonoBehaviour {


    [SerializeField]
    float fruitsSpawnDelay = 0.5f;

    public BoxCollider boxColliderSpawn;
    public float minX;
    public float minZ;
    public float maxX;
    public float maxZ;

    public uint nbPlayer;

    public Material matClementine;
    public Material matPomme;
    public Material matKiwi;
    public Material matFraise;



    GameObject fruit;
    int subPoolIndex;

    public void Start()
    {
        nbPlayer = GameManager.Instance.PlayerStart.ActivePlayersAtStart;
        minX = -(boxColliderSpawn.transform.localScale.x / 2);
        maxX = boxColliderSpawn.transform.localScale.x / 2;

        minZ = -(boxColliderSpawn.transform.localScale.z / 2);
        maxZ = boxColliderSpawn.transform.localScale.z / 2;

        matClementine.color = GameManager.Instance.PlayerStart.colorPlayer[0];
        matPomme.color = GameManager.Instance.PlayerStart.colorPlayer[1];
        matKiwi.color = GameManager.Instance.PlayerStart.colorPlayer[2];
        matFraise.color = GameManager.Instance.PlayerStart.colorPlayer[3];
    }

    public IEnumerator Spawner()
    {
        while (!GameManager.Instance.isTimeOver)
        {
            yield return new WaitForSeconds(fruitsSpawnDelay);

            Vector3 positionToSpawnPlane = new Vector3(Random.Range(minX, maxX), 18.15f, Random.Range(minZ, maxZ));

            if (nbPlayer == 1)
            {
                subPoolIndex = Random.Range(0, (int)nbPlayer + 1);
                fruit = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform, positionToSpawnPlane, Quaternion.identity, true, false, subPoolIndex);
                fruit.GetComponent<BoxCollider>().enabled = true;
                fruit.GetComponent<FruitType>().state = StateFruit.Safe;
            }
            else 
            {
                subPoolIndex = Random.Range(0, (int)nbPlayer);
                fruit = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform, positionToSpawnPlane, Quaternion.identity, true, false, subPoolIndex);
                fruit.GetComponent<BoxCollider>().enabled = true;
                fruit.GetComponent<FruitType>().state = StateFruit.Safe;
            }
        }
    }
}