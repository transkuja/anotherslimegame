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
    }

    public IEnumerator Spawner()
    {
        while (!GameManager.Instance.isTimeOver)
        {
            yield return new WaitForSeconds(fruitsSpawnDelay);

            Vector3 positionToSpawnPlane = new Vector3(Random.Range(minX, maxX), 18.15f, Random.Range(minZ, maxZ));


            if (GameManager.Instance.DataContainer != null)
            {
                switch (nbPlayer)
                {
                    case 1:
                        subPoolIndex = Random.Range(0, ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).PoolParent.childCount - 2);
                        fruit = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform, positionToSpawnPlane, Quaternion.identity, true, false, subPoolIndex);
                        fruit.GetComponent<BoxCollider>().enabled = true;
                        if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Clementine)
                        {
                            matClementine.color = GameManager.Instance.DataContainer.selectedColors[0];
                            fruit.GetComponent<Renderer>().material = matClementine;
                        }
                        else if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Pomme)
                        {
                            if(matPomme.color == GameManager.Instance.DataContainer.selectedColors[0])
                            {
                                matPomme.color = Color.black;
                                fruit.GetComponent<Renderer>().material = matPomme;
                            }
                            else
                                fruit.GetComponent<Renderer>().material = matPomme;
                        }
                        break;
                    case 2:
                        subPoolIndex = Random.Range(0, ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).PoolParent.childCount - 2);
                        fruit = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform, positionToSpawnPlane, Quaternion.identity, true, false, subPoolIndex);
                        fruit.GetComponent<BoxCollider>().enabled = true;
                        if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Clementine)
                        {
                            matClementine.color = GameManager.Instance.DataContainer.selectedColors[0];
                            fruit.GetComponent<Renderer>().material = matClementine;
                        }
                        else if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Pomme)
                        {
                            matPomme.color = GameManager.Instance.DataContainer.selectedColors[1];
                            fruit.GetComponent<Renderer>().material = matPomme;
                        }
                        break;
                    case 3:
                        subPoolIndex = Random.Range(0, ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).PoolParent.childCount - 1);
                        fruit = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform, positionToSpawnPlane, Quaternion.identity, true, false, subPoolIndex);
                        fruit.GetComponent<BoxCollider>().enabled = true;
                        if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Clementine)
                        {
                            matClementine.color = GameManager.Instance.DataContainer.selectedColors[0];
                            fruit.GetComponent<Renderer>().material = matClementine;
                        }
                        else if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Pomme)
                        {
                            matPomme.color = GameManager.Instance.DataContainer.selectedColors[1];
                            fruit.GetComponent<Renderer>().material = matPomme;
                        }
                        else if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Kiwi)
                        {
                            matKiwi.color = GameManager.Instance.DataContainer.selectedColors[2];
                            fruit.GetComponent<Renderer>().material = matKiwi;
                        }
                        break;
                    case 4:
                        subPoolIndex = Random.Range(0, ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).PoolParent.childCount);
                        fruit = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform, positionToSpawnPlane, Quaternion.identity, true, false, subPoolIndex);
                        fruit.GetComponent<BoxCollider>().enabled = true;
                        if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Clementine)
                        {
                            matClementine.color = GameManager.Instance.DataContainer.selectedColors[0];
                            fruit.GetComponent<Renderer>().material = matClementine;
                        }
                        else if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Pomme)
                        {
                            matPomme.color = GameManager.Instance.DataContainer.selectedColors[1];
                            fruit.GetComponent<Renderer>().material = matPomme;
                        }
                        else if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Kiwi)
                        {
                            matKiwi.color = GameManager.Instance.DataContainer.selectedColors[2];
                            fruit.GetComponent<Renderer>().material = matKiwi;
                        }
                        else if (fruit.GetComponent<FruitType>().typeFruit == Fruit.Fraise)
                        {
                            matFraise.color = GameManager.Instance.DataContainer.selectedColors[3];
                            fruit.GetComponent<Renderer>().material = matFraise;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                subPoolIndex = Random.Range(0, ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).PoolParent.childCount);
                fruit = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform, new Vector3(Random.Range(minX, maxX), 15, Random.Range(minZ, maxZ)), Quaternion.identity, true, false, subPoolIndex);
                fruit.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }
}
